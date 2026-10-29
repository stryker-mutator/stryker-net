using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Abstractions;

namespace Stryker.Core.MutantFilters;

/// <summary>
/// Finds mutants whose only possible effect is on whether, or with what, ignored methods are called:
/// the condition of an <c>if</c> whose branches only call ignored methods, and locals that are only
/// read by ignored calls (a counter that is logged and nothing else).
/// Anything that can change other state, throw, or change control flow is never reported.
/// Assumes property getters are pure, that a receiver the code does not null-test is not null, and that
/// integer arithmetic outside a <c>checked</c> block does not throw (no project-wide overflow checking).
/// </summary>
internal sealed class IgnoredCallOnlyEffect
{
    private static readonly HashSet<string> NoneTested = new(StringComparer.Ordinal);

    private readonly Func<SyntaxNode, bool> _isIgnoredCall;
    private readonly Dictionary<SyntaxNode, Dictionary<string, Local>> _deadLocalsByMember = new();
    private readonly Dictionary<SyntaxNode, HashSet<string>> _nullTestCarriersByMember = new();
    private readonly Dictionary<SyntaxNode, Dictionary<string, HashSet<string>>> _checkedByWriteOfByMember = new();

    public IgnoredCallOnlyEffect(Func<SyntaxNode, bool> isIgnoredCall) => _isIgnoredCall = isIgnoredCall;

    public bool IsOnlyEffect(Mutation mutation)
    {
        var node = mutation.OriginalNode;
        var member = node?.FirstAncestorOrSelf<MemberDeclarationSyntax>();
        if (member is null)
        {
            return false;
        }

        var dead = DeadLocals(member);
        switch (node)
        {
            case StatementSyntax statement when mutation.Type is Mutator.Block or Mutator.Statement:
                return IsInert(statement, dead, null);
            case PostfixUnaryExpressionSyntax or PrefixUnaryExpressionSyntax when mutation.Type == Mutator.Update:
                return IsDeadWrite(node, dead);
            case AssignmentExpressionSyntax assignment when mutation.Type == Mutator.Assignment:
                return IsDeadWrite(node, dead)
                    && mutation.ReplacementNode is AssignmentExpressionSyntax replacement
                    && IsSafeWrite(replacement.Kind(), assignment.Left, assignment, dead);
        }

        var nullTested = NullTested(node.FirstAncestorOrSelf<StatementSyntax>());
        return KeepsEvaluation(mutation, nullTested) && FlowsOnlyToIgnoredCalls(node, dead, nullTested);
    }

    // The mutated expression still evaluates exactly what the original did, and cannot throw where it did not.
    private bool KeepsEvaluation(Mutation mutation, HashSet<string> nullTested) => mutation.Type switch
    {
        Mutator.Equality or Mutator.Boolean => true,
        Mutator.Unary => mutation.OriginalNode.IsKind(SyntaxKind.BitwiseNotExpression),
        Mutator.Logical => mutation.OriginalNode is not BinaryExpressionSyntax
        {
            RawKind: (int)SyntaxKind.LogicalAndExpression or (int)SyntaxKind.LogicalOrExpression
        } binary || IsSafe(binary.Right, With(nullTested, GuardedByIncludingVariables(binary.Left))),
        Mutator.Conditional => mutation.OriginalNode is ConditionalExpressionSyntax conditional
            && IsSafe(conditional.Condition, nullTested)
            && IsSafe(conditional.WhenTrue, With(nullTested, GuardedByIncludingVariables(conditional.Condition)))
            && IsSafe(conditional.WhenFalse, With(nullTested, GuardedByIncludingVariables(conditional.Condition))),
        _ => false,
    };

    // Follows the value of node upwards until it lands somewhere only ignored calls observe.
    private bool FlowsOnlyToIgnoredCalls(SyntaxNode node, Dictionary<string, Local> dead, HashSet<string> nullTested)
    {
        var current = node;
        while (true)
        {
            var parent = current.Parent;
            switch (parent)
            {
                case ParenthesizedExpressionSyntax:
                case PrefixUnaryExpressionSyntax
                {
                    RawKind: (int)SyntaxKind.LogicalNotExpression or (int)SyntaxKind.BitwiseNotExpression
                }:
                case IsPatternExpressionSyntax:
                case PatternSyntax and not RecursivePatternSyntax { PositionalPatternClause: not null }:
                case SubpatternSyntax:
                case PropertyPatternClauseSyntax:
                    break;
                case BinaryExpressionSyntax binary:
                    switch (binary.Kind())
                    {
                        case SyntaxKind.LogicalAndExpression or SyntaxKind.LogicalOrExpression or SyntaxKind.CoalesceExpression:
                            if (current == binary.Left && !IsSafe(binary.Right, With(nullTested, GuardedByIncludingVariables(binary.Left))))
                            {
                                return false;
                            }
                            break;
                        case SyntaxKind.EqualsExpression or SyntaxKind.NotEqualsExpression
                            or SyntaxKind.LessThanExpression or SyntaxKind.LessThanOrEqualExpression
                            or SyntaxKind.GreaterThanExpression or SyntaxKind.GreaterThanOrEqualExpression
                            or SyntaxKind.BitwiseAndExpression or SyntaxKind.BitwiseOrExpression or SyntaxKind.ExclusiveOrExpression
                            or SyntaxKind.IsExpression or SyntaxKind.AsExpression:
                            break;
                        default:
                            // arithmetic can overflow or divide by zero on the mutated value
                            return false;
                    }
                    break;
                case ConditionalExpressionSyntax conditional:
                    var branchTested = With(nullTested, GuardedByIncludingVariables(conditional.Condition));
                    if (current == conditional.Condition
                        && !(IsSafe(conditional.WhenTrue, branchTested) && IsSafe(conditional.WhenFalse, branchTested)))
                    {
                        return false;
                    }
                    break;
                case IfStatementSyntax ifStatement when current == ifStatement.Condition:
                    // a guard that reads a null test carrier may be all that stands between a log call and a null
                    if (ReadsNullTestCarrier(ifStatement.Condition, node))
                    {
                        return false;
                    }

                    // a member read in a branch may only be valid because the condition held (`TryGetValue(.., out var v)`
                    // then `v.Length`, `IsSuccess` then `.Value`): the then-branch distrusts what the mutated conjunct
                    // reads or binds, and the else-branch everything the condition does
                    var thenTested = new HashSet<string>(nullTested, StringComparer.Ordinal);
                    thenTested.ExceptWith(ProvenNotNull(ifStatement.Condition, node));
                    thenTested.UnionWith(Conjuncts(ifStatement.Condition).Where(conjunct => conjunct.Span.OverlapsWith(node.Span)).SelectMany(GuardedByIncludingVariables));
                    return IsInert(ifStatement.Statement, dead, thenTested)
                        && (ifStatement.Else is null
                            || IsInert(ifStatement.Else.Statement, dead, With(nullTested, GuardedByIncludingVariables(ifStatement.Condition))));
                case EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax declarator }:
                    return declarator.Parent?.Parent is LocalDeclarationStatementSyntax
                        && dead.ContainsKey(declarator.Identifier.ValueText);
                case AssignmentExpressionSyntax assignment when current == assignment.Right:
                    return IsDeadWrite(assignment, dead);
                default:
                    return false;
            }

            current = parent;
        }
    }

    // A statement that, when it runs (or no longer runs), can only change ignored calls.
    // nullTested is null for removal: an ignored call that no longer runs dereferences nothing.
    private bool IsInert(StatementSyntax statement, Dictionary<string, Local> dead, HashSet<string> nullTested) => statement switch
    {
        BlockSyntax block => block.Statements.All(s => IsInert(s, dead, nullTested)),
        EmptyStatementSyntax => true,
        // an awaited ignored call is not inert: awaiting can throw or yield
        ExpressionStatementSyntax { Expression: var expression } when IsIgnoredCall(expression) =>
            IsSafe(expression, nullTested ?? NoneTested),
        ExpressionStatementSyntax { Expression: PostfixUnaryExpressionSyntax or PrefixUnaryExpressionSyntax } expressionStatement =>
            IsDeadWrite(expressionStatement.Expression, dead),
        ExpressionStatementSyntax { Expression: AssignmentExpressionSyntax assignment } =>
            IsDeadWrite(assignment, dead) && IsSafe(assignment.Right, nullTested ?? NullTested(assignment)),
        IfStatementSyntax ifStatement => IsInertIf(ifStatement, dead, nullTested),
        _ => false,
    };

    private bool IsInertIf(IfStatementSyntax ifStatement, Dictionary<string, Local> dead, HashSet<string> nullTested)
    {
        var tested = NullTested(ifStatement);
        if (nullTested is not null)
        {
            tested.UnionWith(nullTested);
        }

        return IsSafe(ifStatement.Condition, tested)
            && IsInert(ifStatement.Statement, dead, nullTested is null ? null : tested)
            && (ifStatement.Else is null || IsInert(ifStatement.Else.Statement, dead, nullTested is null ? null : tested));
    }

    private bool IsIgnoredCall(ExpressionSyntax expression) =>
        expression is InvocationExpressionSyntax or ConditionalAccessExpressionSyntax && _isIgnoredCall(expression);

    // `x++;`, `x += y;` or `x = y;` on a local that only ignored calls read.
    private static bool IsDeadWrite(SyntaxNode write, Dictionary<string, Local> dead) =>
        write.Parent is ExpressionStatementSyntax && write switch
        {
            PostfixUnaryExpressionSyntax postfix => IsSafeWrite(postfix.Kind(), postfix.Operand, write, dead),
            PrefixUnaryExpressionSyntax prefix => IsSafeWrite(prefix.Kind(), prefix.Operand, write, dead),
            AssignmentExpressionSyntax assignment => IsSafeWrite(assignment.Kind(), assignment.Left, write, dead),
            _ => false,
        };

    // A write of this kind to target cannot throw: arithmetic only on an integer local outside a checked
    // context, and never division, remainder or a conditional assignment.
    private static bool IsSafeWrite(SyntaxKind kind, ExpressionSyntax target, SyntaxNode write, Dictionary<string, Local> dead) => kind switch
    {
        SyntaxKind.SimpleAssignmentExpression or SyntaxKind.AndAssignmentExpression
            or SyntaxKind.OrAssignmentExpression or SyntaxKind.ExclusiveOrAssignmentExpression => IsDeadName(target, dead),
        SyntaxKind.PostIncrementExpression or SyntaxKind.PostDecrementExpression
            or SyntaxKind.PreIncrementExpression or SyntaxKind.PreDecrementExpression
            or SyntaxKind.AddAssignmentExpression or SyntaxKind.SubtractAssignmentExpression or SyntaxKind.MultiplyAssignmentExpression =>
            IsDeadName(target, dead) && dead[((IdentifierNameSyntax)target).Identifier.ValueText].IsInteger && !IsChecked(write),
        _ => false,
    };

    private static bool IsDeadName(ExpressionSyntax expression, Dictionary<string, Local> dead) =>
        expression is IdentifierNameSyntax identifier
        && dead.TryGetValue(identifier.Identifier.ValueText, out var local)
        && local.Scope.Span.Contains(identifier.Span);

    private static bool IsChecked(SyntaxNode node) =>
        node.Ancestors().Any(n => n.IsKind(SyntaxKind.CheckedStatement) || n.IsKind(SyntaxKind.CheckedExpression));

    // Evaluating this can neither change state nor throw.
    private bool IsSafe(ExpressionSyntax expression, HashSet<string> nullTested) =>
        expression is null || expression.DescendantNodesAndSelf(n => !IsNameOf(n)).All(n => n switch
        {
            InvocationExpressionSyntax => _isIgnoredCall(n) || IsNameOf(n),
            BaseObjectCreationExpressionSyntax or ArrayCreationExpressionSyntax or ImplicitArrayCreationExpressionSyntax
                or AssignmentExpressionSyntax or AwaitExpressionSyntax or ThrowExpressionSyntax
                or ElementAccessExpressionSyntax or ElementBindingExpressionSyntax or CastExpressionSyntax
                or QueryExpressionSyntax or WithExpressionSyntax or PositionalPatternClauseSyntax
                or CheckedExpressionSyntax => false,
            PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.PreIncrementExpression or (int)SyntaxKind.PreDecrementExpression } => false,
            PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.UnaryMinusExpression } negation => negation.Operand is LiteralExpressionSyntax,
            PostfixUnaryExpressionSyntax
            {
                RawKind: (int)SyntaxKind.PostIncrementExpression or (int)SyntaxKind.PostDecrementExpression or (int)SyntaxKind.SuppressNullableWarningExpression
            } => false,
            BinaryExpressionSyntax binary => binary.Kind() switch
            {
                SyntaxKind.AddExpression => IsStringConcatenation(binary),
                SyntaxKind.SubtractExpression or SyntaxKind.MultiplyExpression or SyntaxKind.DivideExpression or SyntaxKind.ModuloExpression
                    or SyntaxKind.LeftShiftExpression or SyntaxKind.RightShiftExpression or SyntaxKind.UnsignedRightShiftExpression => false,
                _ => true,
            },
            MemberAccessExpressionSyntax access => !nullTested.Contains(Key(access.Expression)),
            _ => true,
        });

    // nameof(...) parses as an invocation but is a compile-time constant; nothing inside it is evaluated.
    private static bool IsNameOf(SyntaxNode node) =>
        node is InvocationExpressionSyntax { Expression: IdentifierNameSyntax { Identifier.ValueText: "nameof" }, ArgumentList.Arguments.Count: 1 };

    // Without types, only a concatenation with a string literal is known not to be arithmetic.
    private static bool IsStringConcatenation(BinaryExpressionSyntax binary) =>
        binary.IsKind(SyntaxKind.AddExpression) && (IsString(binary.Left) || IsString(binary.Right));

    private static bool IsString(ExpressionSyntax expression) => expression switch
    {
        LiteralExpressionSyntax literal => literal.IsKind(SyntaxKind.StringLiteralExpression),
        InterpolatedStringExpressionSyntax => true,
        ParenthesizedExpressionSyntax parenthesized => IsString(parenthesized.Expression),
        BinaryExpressionSyntax binary => IsStringConcatenation(binary),
        _ => false,
    };

    // Expressions the statement's own expressions test for null; a flipped test dereferences null.
    private static HashSet<string> NullTested(SyntaxNode root)
    {
        var tested = new HashSet<string>(StringComparer.Ordinal);
        if (root is null)
        {
            return tested;
        }

        foreach (var node in root.DescendantNodesAndSelf(n => n == root || n is not StatementSyntax))
        {
            switch (node)
            {
                case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.EqualsExpression or (int)SyntaxKind.NotEqualsExpression } binary:
                    if (binary.Right.IsKind(SyntaxKind.NullLiteralExpression))
                    {
                        tested.Add(Key(binary.Left));
                    }
                    else if (binary.Left.IsKind(SyntaxKind.NullLiteralExpression))
                    {
                        tested.Add(Key(binary.Right));
                    }
                    break;
                case IsPatternExpressionSyntax pattern:
                    tested.Add(Key(pattern.Expression));
                    break;
                case ConditionalAccessExpressionSyntax conditional:
                    tested.Add(Key(conditional.Expression));
                    break;
                case MemberAccessExpressionSyntax { Name.Identifier.ValueText: "HasValue" } access:
                    tested.Add(Key(access.Expression));
                    break;
                case InvocationExpressionSyntax { ArgumentList.Arguments.Count: > 0 } invocation
                    when invocation.Expression.ToString().Split('.').Last().StartsWith("IsNullOr", StringComparison.Ordinal):
                    tested.Add(Key(invocation.ArgumentList.Arguments[0].Expression));
                    break;
            }
        }

        return tested;
    }

    // The then-branch only runs when every top-level conjunct holds, so a conjunct the mutant leaves
    // untouched that proves a value non-null keeps proving it there.
    private static IEnumerable<string> ProvenNotNull(ExpressionSyntax condition, SyntaxNode mutated)
    {
        foreach (var conjunct in Conjuncts(condition))
        {
            if (conjunct.Span.OverlapsWith(mutated.Span))
            {
                continue;
            }

            switch (conjunct)
            {
                case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.NotEqualsExpression } binary
                    when binary.Right.IsKind(SyntaxKind.NullLiteralExpression):
                    yield return Key(binary.Left);
                    break;
                case BinaryExpressionSyntax { RawKind: (int)SyntaxKind.NotEqualsExpression } binary
                    when binary.Left.IsKind(SyntaxKind.NullLiteralExpression):
                    yield return Key(binary.Right);
                    break;
                case IsPatternExpressionSyntax { Pattern: UnaryPatternSyntax { Pattern: ConstantPatternSyntax constant } } pattern
                    when constant.Expression.IsKind(SyntaxKind.NullLiteralExpression):
                    yield return Key(pattern.Expression);
                    break;
                case IsPatternExpressionSyntax { Pattern: RecursivePatternSyntax or DeclarationPatternSyntax or TypePatternSyntax } pattern:
                    yield return Key(pattern.Expression);
                    break;
                case MemberAccessExpressionSyntax { Name.Identifier.ValueText: "HasValue" } access:
                    yield return Key(access.Expression);
                    break;
            }
        }
    }

    private static IEnumerable<ExpressionSyntax> Conjuncts(ExpressionSyntax expression) => expression switch
    {
        ParenthesizedExpressionSyntax parenthesized => Conjuncts(parenthesized.Expression),
        BinaryExpressionSyntax { RawKind: (int)SyntaxKind.LogicalAndExpression } and =>
            Conjuncts(and.Left).Concat(Conjuncts(and.Right)),
        _ => [expression],
    };

    private static string Key(ExpressionSyntax expression)
    {
        while (true)
        {
            switch (expression)
            {
                case ParenthesizedExpressionSyntax parenthesized:
                    expression = parenthesized.Expression;
                    continue;
                case PostfixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.SuppressNullableWarningExpression } suppressed:
                    expression = suppressed.Operand;
                    continue;
                case MemberAccessExpressionSyntax { Expression: ThisExpressionSyntax } access:
                    return access.Name.Identifier.ValueText;
                default:
                    return string.Concat(expression.DescendantTokens().Select(t => t.ValueText));
            }
        }
    }

    // A local and the block that declares it; only references inside that block are the local.
    private sealed record Local(SyntaxNode Scope, bool IsInteger, List<IdentifierNameSyntax> References);

    // Locals whose value only ever reaches ignored calls: the greatest set where every reference
    // is a statement-level write, an argument of an ignored call, or flows into such a local or
    // into the condition of an if whose branches are inert.
    private Dictionary<string, Local> DeadLocals(SyntaxNode member)
    {
        if (_deadLocalsByMember.TryGetValue(member, out var cached))
        {
            return cached;
        }

        var declared = member.DescendantNodes()
            .Select(DeclaredName)
            .Where(name => name is not null)
            .GroupBy(name => name)
            .Where(group => group.Count() == 1)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.Ordinal);
        var dead = member.DescendantNodes()
            .OfType<LocalDeclarationStatementSyntax>()
            .Where(local => local.UsingKeyword.IsKind(SyntaxKind.None) && !local.IsConst
                && local.Declaration.Type is not RefTypeSyntax && local.Parent is BlockSyntax or SwitchSectionSyntax)
            .SelectMany(local => local.Declaration.Variables.Select(variable => (
                Name: variable.Identifier.ValueText,
                Local: new Local(local.Parent, IsInteger(local.Declaration.Type, variable),
                    References(local.Parent, variable.Identifier.ValueText).ToList()))))
            .Where(local => declared.Contains(local.Name))
            .ToDictionary(local => local.Name, local => local.Local, StringComparer.Ordinal);

        // a variable whose value carries a null test, because it holds one, is written under one, or is
        // computed from such a variable, keeps guarding a dereference after the test is mutated: it is
        // never treated as dead, and a guard that reads it is never filtered. Every variable the member
        // writes counts (parameters, fields and repeated names too), matched by name across the member.
        var writes = member.DescendantNodes().Select(WriteSite).Where(write => write.Name is not null).ToList();
        var nullTestCarriers = writes
            .Where(write => DependsOnNullTest(write.Site, member))
            .Select(write => write.Name)
            .ToHashSet(StringComparer.Ordinal);
        bool changed;
        do
        {
            changed = false;
            foreach (var (name, site) in writes)
            {
                if (!nullTestCarriers.Contains(name)
                    && site.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                        .Any(identifier => identifier != site && nullTestCarriers.Contains(identifier.Identifier.ValueText)))
                {
                    nullTestCarriers.Add(name);
                    changed = true;
                }
            }
        }
        while (changed);

        foreach (var name in nullTestCarriers)
        {
            dead.Remove(name);
        }

        _nullTestCarriersByMember[member] = nullTestCarriers;
        _checkedByWriteOfByMember[member] = CheckedByWrites(writes, member);
        _deadLocalsByMember[member] = dead;
        do
        {
            var notDead = dead.Where(local => !local.Value.References.All(reference => IsDeadReference(reference, dead))).Select(local => local.Key).ToList();
            notDead.ForEach(name => dead.Remove(name));
            changed = notDead.Count > 0;
        }
        while (changed);

        return dead;
    }

    // Where a variable gets a value: an initializer, the right side of an assignment, or the operand of `++`/`--`.
    private static (string Name, ExpressionSyntax Site) WriteSite(SyntaxNode node) => node switch
    {
        VariableDeclaratorSyntax { Initializer: { } initializer } declarator => (declarator.Identifier.ValueText, initializer.Value),
        AssignmentExpressionSyntax { Left: IdentifierNameSyntax target } assignment => (target.Identifier.ValueText, assignment.Right),
        PrefixUnaryExpressionSyntax { Operand: IdentifierNameSyntax target, RawKind: (int)SyntaxKind.PreIncrementExpression or (int)SyntaxKind.PreDecrementExpression } =>
            (target.Identifier.ValueText, target),
        PostfixUnaryExpressionSyntax { Operand: IdentifierNameSyntax target, RawKind: (int)SyntaxKind.PostIncrementExpression or (int)SyntaxKind.PostDecrementExpression } =>
            (target.Identifier.ValueText, target),
        _ => (null, null),
    };

    // What a condition reads or binds; a member read on any of them may depend on the condition holding.
    private static IEnumerable<string> GuardedBy(SyntaxNode condition) =>
        condition.DescendantNodesAndSelf().Select(node => node switch
        {
            SingleVariableDesignationSyntax designation => designation.Identifier.ValueText,
            ExpressionSyntax expression => Key(expression),
            _ => null,
        }).Where(key => key is not null);

    // GuardedBy, plus what any variable the condition reads was computed from or written under:
    // `var found = d.TryGetValue(k, out var v); if (found) ..v.Length` or `var ok = r.IsSuccess; if (ok) ..r.Value`.
    private IEnumerable<string> GuardedByIncludingVariables(SyntaxNode condition)
    {
        var keys = GuardedBy(condition);
        return _checkedByWriteOfByMember.TryGetValue(condition.FirstAncestorOrSelf<MemberDeclarationSyntax>(), out var checkedBy)
            ? keys.Concat(condition.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>()
                .SelectMany(identifier => checkedBy.TryGetValue(identifier.Identifier.ValueText, out var more) ? more : []))
            : keys;
    }

    // For every variable the member writes: the receivers its written values read a member of and the
    // variables they bind (`r` in `r.IsSuccess`, `v` in `TryGetValue(k, out var v)`), what the conditions
    // the writes sit under read or bind, and the same for every variable the written values copy.
    private static Dictionary<string, HashSet<string>> CheckedByWrites(List<(string Name, ExpressionSyntax Site)> writes, SyntaxNode member)
    {
        var checkedBy = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        var copies = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        foreach (var (name, site) in writes)
        {
            if (!checkedBy.TryGetValue(name, out var keys))
            {
                checkedBy[name] = keys = new HashSet<string>(StringComparer.Ordinal);
                copies[name] = new HashSet<string>(StringComparer.Ordinal);
            }

            if (site.Parent is not (PrefixUnaryExpressionSyntax or PostfixUnaryExpressionSyntax))
            {
                foreach (var (copy, key) in ValidatedBy(site))
                {
                    (copy ? copies[name] : keys).Add(key);
                }
            }

            SyntaxNode onPath = site;
            foreach (var ancestor in site.Ancestors().TakeWhile(ancestor => ancestor != member))
            {
                var condition = ancestor switch
                {
                    IfStatementSyntax ifStatement when onPath != ifStatement.Condition => ifStatement.Condition,
                    WhileStatementSyntax whileStatement when onPath != whileStatement.Condition => whileStatement.Condition,
                    ConditionalExpressionSyntax conditional when onPath != conditional.Condition => conditional.Condition,
                    BinaryExpressionSyntax
                    {
                        RawKind: (int)SyntaxKind.LogicalAndExpression or (int)SyntaxKind.LogicalOrExpression or (int)SyntaxKind.CoalesceExpression
                    } binary when onPath == binary.Right => binary.Left,
                    _ => null,
                };
                if (condition is not null)
                {
                    keys.UnionWith(GuardedBy(condition));
                }

                onPath = ancestor;
            }
        }

        bool changed;
        do
        {
            changed = false;
            foreach (var (name, keys) in checkedBy)
            {
                var through = copies[name].Where(checkedBy.ContainsKey).SelectMany(copied => checkedBy[copied]).ToList();
                changed |= through.Count(keys.Add) > 0;
            }
        }
        while (changed);

        return checkedBy;
    }

    // What a written value checks: the receivers it reads a member of, what a call it makes reads or binds in
    // its arguments (`IsValid(r)`, `TryGetValue(k, out var v)`), and (Copy) the variables it copies.
    private static IEnumerable<(bool Copy, string Key)> ValidatedBy(ExpressionSyntax value) => value switch
    {
        IdentifierNameSyntax identifier => [(true, identifier.Identifier.ValueText)],
        ParenthesizedExpressionSyntax parenthesized => ValidatedBy(parenthesized.Expression),
        PrefixUnaryExpressionSyntax prefix => ValidatedBy(prefix.Operand),
        AwaitExpressionSyntax awaited => ValidatedBy(awaited.Expression),
        BinaryExpressionSyntax binary => ValidatedBy(binary.Left).Concat(ValidatedBy(binary.Right)),
        MemberAccessExpressionSyntax access => ValidatedBy(access.Expression).Append((false, Key(access.Expression))),
        ConditionalAccessExpressionSyntax conditional => ValidatedBy(conditional.Expression).Append((false, Key(conditional.Expression))),
        InvocationExpressionSyntax invocation => ValidatedBy(invocation.Expression).Concat(invocation.ArgumentList.Arguments
            .SelectMany(argument => GuardedBy(argument.Expression))
            .Select(key => (false, key))),
        IsPatternExpressionSyntax pattern => ValidatedBy(pattern.Expression).Append((false, Key(pattern.Expression)))
            .Concat(pattern.Pattern.DescendantNodesAndSelf().OfType<SingleVariableDesignationSyntax>()
                .Select(designation => (false, designation.Identifier.ValueText))),
        _ => [],
    };

    private static HashSet<string> With(HashSet<string> tested, IEnumerable<string> more)
    {
        var combined = new HashSet<string>(tested, StringComparer.Ordinal);
        combined.UnionWith(more);
        return combined;
    }

    // The written value holds a null test, or the write is conditional on an expression that tests for null.
    private static bool DependsOnNullTest(ExpressionSyntax site, SyntaxNode member)
    {
        if (NullTested(site).Count > 0)
        {
            return true;
        }

        SyntaxNode onPath = site;
        foreach (var ancestor in site.Ancestors().TakeWhile(ancestor => ancestor != member))
        {
            if (ancestor.ChildNodes().Any(guard => guard != onPath && guard is not StatementSyntax
                && (NullTested(guard).Count > 0 || guard.DescendantNodesAndSelf().Any(n => n.IsKind(SyntaxKind.NullLiteralExpression)))))
            {
                return true;
            }

            onPath = ancestor;
        }

        return false;
    }

    private bool ReadsNullTestCarrier(ExpressionSyntax condition, SyntaxNode mutated) =>
        _nullTestCarriersByMember.TryGetValue(mutated.FirstAncestorOrSelf<MemberDeclarationSyntax>(), out var carriers)
        && condition.DescendantNodesAndSelf().OfType<IdentifierNameSyntax>().Any(identifier => carriers.Contains(identifier.Identifier.ValueText));

    // The ignored call's result is thrown away, or only disposed by a using that nothing else reads.
    private static bool IsResultDiscarded(InvocationExpressionSyntax invocation, SyntaxNode member)
    {
        SyntaxNode call = invocation.Parent is ConditionalAccessExpressionSyntax conditional && conditional.WhenNotNull == invocation
            ? conditional
            : invocation;
        return call.Parent switch
        {
            ExpressionStatementSyntax => true,
            UsingStatementSyntax usingStatement => usingStatement.Expression == call,
            EqualsValueClauseSyntax { Parent: VariableDeclaratorSyntax declarator } =>
                declarator.Parent?.Parent is LocalDeclarationStatementSyntax { UsingKeyword.RawKind: (int)SyntaxKind.UsingKeyword }
                    or UsingStatementSyntax
                && !member.DescendantNodes().OfType<IdentifierNameSyntax>().Any(identifier => identifier.Identifier.ValueText == declarator.Identifier.ValueText),
            _ => false,
        };
    }

    private static bool IsInteger(TypeSyntax type, VariableDeclaratorSyntax variable) => type switch
    {
        PredefinedTypeSyntax predefined => predefined.Keyword.Kind() is SyntaxKind.IntKeyword or SyntaxKind.LongKeyword
            or SyntaxKind.ShortKeyword or SyntaxKind.ByteKeyword or SyntaxKind.SByteKeyword
            or SyntaxKind.UIntKeyword or SyntaxKind.ULongKeyword or SyntaxKind.UShortKeyword,
        IdentifierNameSyntax { IsVar: true } => variable.Initializer?.Value is LiteralExpressionSyntax literal
            && literal.IsKind(SyntaxKind.NumericLiteralExpression) && literal.Token.Value is int or long or uint or ulong,
        _ => false,
    };

    private static string DeclaredName(SyntaxNode node) => node switch
    {
        VariableDeclaratorSyntax declarator => declarator.Identifier.ValueText,
        ParameterSyntax parameter => parameter.Identifier.ValueText,
        SingleVariableDesignationSyntax designation => designation.Identifier.ValueText,
        ForEachStatementSyntax forEach => forEach.Identifier.ValueText,
        CatchDeclarationSyntax catchDeclaration => catchDeclaration.Identifier.ValueText,
        LocalFunctionStatementSyntax localFunction => localFunction.Identifier.ValueText,
        QueryClauseSyntax or FromClauseSyntax or QueryContinuationSyntax => node.ChildTokens()
            .FirstOrDefault(t => t.IsKind(SyntaxKind.IdentifierToken)).ValueText,
        _ => null,
    };

    private static IEnumerable<IdentifierNameSyntax> References(SyntaxNode scope, string name) =>
        scope.DescendantNodes().OfType<IdentifierNameSyntax>().Where(identifier =>
            identifier.Identifier.ValueText == name
            && identifier.Parent switch
            {
                MemberAccessExpressionSyntax access => access.Name != identifier,
                MemberBindingExpressionSyntax or NameColonSyntax or NameEqualsSyntax or QualifiedNameSyntax => false,
                AssignmentExpressionSyntax { Parent: InitializerExpressionSyntax } assignment => assignment.Left != identifier,
                _ => true,
            });

    private bool IsDeadReference(IdentifierNameSyntax reference, Dictionary<string, Local> dead)
    {
        if (IsDeadWrite(reference.Parent, dead) && reference.Parent switch
            {
                AssignmentExpressionSyntax assignment => assignment.Left == reference,
                _ => true,
            })
        {
            return true;
        }

        // an argument of an ignored call whose result is discarded, passed by value, whose evaluation does nothing else
        var argument = reference.FirstAncestorOrSelf<ArgumentSyntax>();
        while (argument is not null)
        {
            if (argument.Parent?.Parent is InvocationExpressionSyntax invocation && _isIgnoredCall(invocation))
            {
                return reference.FirstAncestorOrSelf<ArgumentSyntax>().RefKindKeyword.IsKind(SyntaxKind.None)
                    && IsSafe(argument.Expression, NoneTested)
                    && IsResultDiscarded(invocation, reference.FirstAncestorOrSelf<MemberDeclarationSyntax>());
            }

            argument = argument.Parent?.FirstAncestorOrSelf<ArgumentSyntax>();
        }

        var statement = reference.FirstAncestorOrSelf<StatementSyntax>();
        return FlowsOnlyToIgnoredCalls(reference, dead, NullTested(statement));
    }
}
