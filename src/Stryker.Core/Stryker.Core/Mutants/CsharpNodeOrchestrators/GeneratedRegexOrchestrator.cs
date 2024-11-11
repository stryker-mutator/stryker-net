using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Logging;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.Mutators;
using Stryker.RegexMutators;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Stryker.Core.Mutants.CsharpNodeOrchestrators;

internal sealed class GeneratedRegexOrchestrator : MemberDefinitionOrchestrator<TypeDeclarationSyntax>
{
    private ILogger Logger { get; } = ApplicationLogging.LoggerFactory.CreateLogger<GeneratedRegexOrchestrator>();

    private static readonly SyntaxAnnotation _tempAnnotationMarker = new (Guid.NewGuid().ToString());

    /// <inheritdoc />
    protected override bool CanHandle(TypeDeclarationSyntax t) =>
        t is ClassDeclarationSyntax or StructDeclarationSyntax or RecordDeclarationSyntax &&
        t.Modifiers.Any(static a => a.IsKind(SyntaxKind.PartialKeyword));

    /// <inheritdoc />
    public override SyntaxNode Mutate(SyntaxNode node, SemanticModel semanticModel, MutationContext context)
    {
        var toProcess = ((TypeDeclarationSyntax)node).Members
                                                  .Select(a => GenerateNewMethods(a, semanticModel))
                                                  .Where(static a => a.HasValue)
                                                  .Select(static a => a.Value)
                                                  .ToImmutableArray();

        if (toProcess.Length == 0)
        {
            return base.Mutate(node, semanticModel, context);
        }

        var originalNode = node;
        var correctSemanticModel = semanticModel;

        node = node.TrackNodes(toProcess.Select(static SyntaxNode (a) => a.OriginalNode)).WithAdditionalAnnotations(_tempAnnotationMarker);

        if (semanticModel is not null)
        {
            // Obtain new SemanticModel for modified syntax tree
            var newSyntaxTree = originalNode.SyntaxTree.GetRoot().ReplaceNode(originalNode, node).SyntaxTree;
            correctSemanticModel = semanticModel.Compilation.AddSyntaxTrees(newSyntaxTree).GetSemanticModel(newSyntaxTree);
            node = newSyntaxTree.GetRoot().GetAnnotatedNodes(_tempAnnotationMarker).First();
        }

        return InnerMutate(base.Mutate(node, correctSemanticModel, context), context, toProcess);
    }

    private static SyntaxNode InnerMutate(SyntaxNode node, MutationContext context, ImmutableArray<RegexMutationBatch> toProcess) {
        context = context.Enter(MutationControl.Expression);

        foreach (var (oldNode, marker, (proxyMethod, renamedMethod, _), mutations2) in toProcess)
        {
            node = node.ReplaceNode(node.GetCurrentNode<MemberDeclarationSyntax>(oldNode)!, [
                proxyMethod,
                renamedMethod,
                ..mutations2.OrderBy(static a => a.Name).Select(static a => a.NewMethod)
            ]);

            var l = new List<(Mutant, ExpressionSyntax)>(mutations2.Length);

            foreach (var mutation in mutations2.OrderBy(static a => a.Name))
            {
                if (context.GenerateMutant(context, mutation.Mutation, typeof(GeneratedRegexOrchestrator)) is { } m)
                {
                    l.Add((m, (ExpressionSyntax)mutation.Mutation.ReplacementNode));
                }
            }

            var nodeToMutate = node.GetAnnotatedNodes(marker).OfType<ExpressionSyntax>().First();
            node = node.ReplaceNode(nodeToMutate, context.Placer.PlaceExpressionControlledMutations(nodeToMutate, l));
        }

        context.Leave();
        return node;
    }

    private RegexMutationBatch? GenerateNewMethods(MemberDeclarationSyntax method, SemanticModel semanticModel)
    {
        var mpds = method switch
        {
            MethodDeclarationSyntax mds => new MethodOrPropertyDeclarationSyntax(mds),
            PropertyDeclarationSyntax pds => new MethodOrPropertyDeclarationSyntax(pds),
            _ => null
        };

        var regexAttribute = mpds?.GetRegexAttribute();

        if (regexAttribute is null)
        {
            return null;
        }

        var marker = new SyntaxAnnotation(Guid.NewGuid().ToString());
        var proxyInfo = MutatePartialRegexMethod(mpds, marker);

        var regexMutations = GenerateNewMethods(mpds, regexAttribute, semanticModel, proxyInfo.NodeToMutate).ToArray();

        if (regexMutations.Length == 0)
        {
            return null;
        }

        return new RegexMutationBatch(mpds, marker, proxyInfo, regexMutations);
    }

    private IEnumerable<MutationInfo> GenerateNewMethods(MethodOrPropertyDeclarationSyntax method,
                                                                      AttributeSyntax                   regexAttribute,
                                                                      SemanticModel                     model,
                                                                      ExpressionSyntax                  nodeToMutate)
    {
        var arguments = regexAttribute.ArgumentList?.Arguments;

        var namedArgument = arguments?.FirstOrDefault(static argument =>
                                                          argument.NameColon?.Name.Identifier.ValueText ==
                                                          "pattern");
        var patternArgument = namedArgument ?? arguments?.FirstOrDefault();
        var patternExpression = patternArgument?.Expression;

        string currentValue = null;

        if (patternExpression.IsKind(SyntaxKind.IdentifierName) && model is not null)
        {
            var constantValue = model.GetConstantValue(patternExpression);

            if (constantValue.HasValue)
            {
                currentValue = constantValue.Value as string;
            }
            else
            {
                currentValue = (model.GetSymbolInfo(patternExpression).Symbol as IFieldSymbol)?.OriginalDefinition
                           .ConstantValue as string;
            }
        }

        if (patternExpression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
        {
            currentValue = model?.GetConstantValue(patternExpression).Value as string;
        }

        if (patternExpression.IsKind(SyntaxKind.StringLiteralExpression))
        {
            currentValue = ((LiteralExpressionSyntax)patternExpression).Token.ValueText;
        }

        if (currentValue is null)
        {
            yield break;
        }

        var patternExpressionLineSpan = patternExpression.GetLocation().GetLineSpan();
        var regexMutantOrchestrator = new RegexMutantOrchestrator(currentValue);
        var replacementValues = regexMutantOrchestrator.Mutate();

        foreach (var regexMutation in replacementValues)
        {
            try
            {
                _ = new Regex(regexMutation.ReplacementPattern);
            }
            catch (ArgumentException exception)
            {
                Logger.LogDebug(
                                "RegexMutator created mutation {CurrentValue} -> {ReplacementPattern} which is an invalid regular expression:\n{Message}",
                                currentValue, regexMutation.ReplacementPattern, exception.Message);
                continue;
            }

            var hashData = SHA1.HashData(Encoding.UTF8.GetBytes(regexMutation.ReplacementPattern));
            var hash = Convert.ToBase64String(hashData).Replace('+', 'A').Replace('/', 'B').Replace('=', 'C');

            var newName =
                $"{method.Identifier.ValueText}_{CultureInfo.InvariantCulture.TextInfo.ToTitleCase(regexMutation.DisplayName.Replace("\"[\\w\\W]\"", "AnyChar").Replace("-", "")).Replace(" ", "")}_{hash}";

            SyntaxNode replacementNode = nodeToMutate is InvocationExpressionSyntax
                                             ? InvocationExpression(IdentifierName(newName), ArgumentList())
                                             : IdentifierName(newName);

            yield return
                new MutationInfo(newName,
                                              new Mutation
                                              {
                                                  OriginalNode     = nodeToMutate,
                                                  ReplacementNode  = replacementNode,
                                                  DisplayName      = regexMutation.DisplayName,
                                                  Type             = Mutator.Regex,
                                                  Description      = regexMutation.Description,
                                                  ReplacementText  = $"\"{regexMutation.ReplacementPattern}\"",
                                                  OriginalLocation = patternExpressionLineSpan
                                              },
                                              method.ReplaceNode(patternExpression,
                                                                 LiteralExpression(SyntaxKind.StringLiteralExpression,
                                                                  Literal(regexMutation.ReplacementPattern)))
                                                 .WithIdentifier(Identifier(newName))
                                                 .RemoveIfDirectives());
        }
    }

    private static ProxyInfo MutatePartialRegexMethod(MethodOrPropertyDeclarationSyntax originalMethod,
                                                      SyntaxAnnotation                  marker)
    {
        var newName = Identifier($"{originalMethod.Identifier.ValueText}_Original");

        var regexAttribute = originalMethod.GetRegexAttribute();

        var nodeToMutate = originalMethod.CreateCall(newName).WithAdditionalAnnotations(marker);

        var proxyMethod = originalMethod
                      .RemoveNode(regexAttribute, SyntaxRemoveOptions.KeepNoTrivia)!
                      .WithExpressionBody(ArrowExpressionClause(Token(TriviaList(Space), SyntaxKind.EqualsGreaterThanToken, TriviaList(Space)),
                                                                nodeToMutate))
                      .WithSemicolonToken(Token(SyntaxTriviaList.Empty, SyntaxKind.SemicolonToken, ";", ";",
                                                TriviaList(LineFeed)))
                      .WithModifiers(originalMethod.Modifiers.Remove(originalMethod.Modifiers.First(static a =>
                                                                         a.IsKind(SyntaxKind.PartialKeyword))));

        proxyMethod = proxyMethod.RemoveNodes(proxyMethod.AttributeLists.Where(static a => a.Attributes.Count == 0),
                                              SyntaxRemoveOptions.KeepNoTrivia)!.WithTriviaFrom(regexAttribute.Parent);
        var newMethod = originalMethod.WithIdentifier(newName).RemoveIfDirectives();

        return new ProxyInfo(proxyMethod, newMethod, nodeToMutate);
    }

    /// <summary>
    ///     Contains info about the modification of the original regex method/property that is now proxied through a method
    ///     that can be mutated.
    /// </summary>
    /// <param name="ProxyMethod">
    ///     The method that the original code calls, which was a method marked with a
    ///     <see cref="GeneratedRegexAttribute" /> that has now been removed. Body is now an invocation of
    ///     <see cref="RenamedMethod" />
    /// </param>
    /// <param name="RenamedMethod">A copy of the original method, with a new name</param>
    /// <param name="NodeToMutate">
    ///     The call from <see cref="ProxyMethod" /> to <see cref="RenamedMethod" /> that will be the
    ///     code that is mutated
    /// </param>
    private record struct ProxyInfo(
        MethodOrPropertyDeclarationSyntax ProxyMethod,
        MethodOrPropertyDeclarationSyntax RenamedMethod,
        ExpressionSyntax                  NodeToMutate);

    /// <summary>
    ///     Contains info about a mutation to be applied, along with the method that the mutation will use.
    /// </summary>
    /// <param name="Name">The full name of the generated method</param>
    /// <param name="Mutation">The <see cref="Abstractions.Mutants.Mutation" /> to apply</param>
    /// <param name="NewMethod">A copy of the original regex method with a <see cref="RegexMutation" /> applied</param>
    private record struct MutationInfo(
        string                            Name,
        Mutation                          Mutation,
        MethodOrPropertyDeclarationSyntax NewMethod);

    /// <summary>
    ///     All info needed about the mutations for a single <see cref="GeneratedRegexAttribute" />.
    /// </summary>
    /// <param name="OriginalNode">The original method or property marked with a <see cref="GeneratedRegexAttribute" /></param>
    /// <param name="Marker">The marker of the node to mutate</param>
    /// <param name="ProxyInfo">The <see cref="GeneratedRegexOrchestrator.ProxyInfo"/> for this batch of mutations</param>
    /// <param name="Mutations">All of the <see cref="MutationInfo"/>s for this batch</param>
    private record struct RegexMutationBatch(
        MethodOrPropertyDeclarationSyntax OriginalNode,
        SyntaxAnnotation                  Marker,
        ProxyInfo                         ProxyInfo,
        MutationInfo[]                    Mutations);
}

internal sealed class MethodOrPropertyDeclarationSyntax
{
    private readonly MemberDeclarationSyntax _memberDeclaration;

    public SyntaxToken Identifier => _memberDeclaration switch
    {
        MethodDeclarationSyntax mds => mds.Identifier,
        PropertyDeclarationSyntax pds => pds.Identifier,
        _ => throw new UnreachableException()
    };

    public SyntaxTokenList Modifiers => _memberDeclaration switch
    {
        MethodDeclarationSyntax mds => mds.Modifiers,
        PropertyDeclarationSyntax pds => pds.Modifiers,
        _ => throw new UnreachableException()
    };

    public SyntaxList<AttributeListSyntax> AttributeLists => _memberDeclaration switch
    {
        MethodDeclarationSyntax mds => mds.AttributeLists,
        PropertyDeclarationSyntax pds => pds.AttributeLists,
        _ => throw new UnreachableException()
    };

    public MethodOrPropertyDeclarationSyntax(MethodDeclarationSyntax memberDeclaration) =>
        _memberDeclaration = memberDeclaration;

    public MethodOrPropertyDeclarationSyntax(PropertyDeclarationSyntax memberDeclaration) =>
        _memberDeclaration = memberDeclaration;

    public MethodOrPropertyDeclarationSyntax WithIdentifier(SyntaxToken identifier) => _memberDeclaration switch
    {
        MethodDeclarationSyntax mds => mds.WithIdentifier(identifier),
        PropertyDeclarationSyntax pds => pds.WithIdentifier(identifier),
        _ => throw new UnreachableException()
    };

    public MethodOrPropertyDeclarationSyntax ReplaceNode(SyntaxNode oldNode, SyntaxNode newNode) =>
        _memberDeclaration switch
        {
            MethodDeclarationSyntax mds => mds.ReplaceNode(oldNode, newNode),
            PropertyDeclarationSyntax pds => pds.ReplaceNode(oldNode, newNode),
            _ => throw new UnreachableException()
        };

    public MethodOrPropertyDeclarationSyntax RemoveNodes(IEnumerable<SyntaxNode> nodes, SyntaxRemoveOptions options) =>
        _memberDeclaration switch
        {
            MethodDeclarationSyntax mds => mds.RemoveNodes(nodes, options),
            PropertyDeclarationSyntax pds => pds.RemoveNodes(nodes, options),
            _ => throw new UnreachableException()
        };

    public MethodOrPropertyDeclarationSyntax RemoveNode(SyntaxNode node, SyntaxRemoveOptions options) =>
        _memberDeclaration switch
        {
            MethodDeclarationSyntax mds => mds.RemoveNode(node, options),
            PropertyDeclarationSyntax pds => pds.RemoveNode(node, options),
            _ => throw new UnreachableException()
        };

    public MethodOrPropertyDeclarationSyntax
        WithExpressionBody(ArrowExpressionClauseSyntax arrowExpressionClauseSyntax) => _memberDeclaration switch
    {
        MethodDeclarationSyntax mds => mds.WithExpressionBody(arrowExpressionClauseSyntax),
        PropertyDeclarationSyntax pds => pds.WithExpressionBody(arrowExpressionClauseSyntax).WithAccessorList(default),
        _ => throw new UnreachableException()
    };

    public MethodOrPropertyDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken) => _memberDeclaration switch
    {
        MethodDeclarationSyntax mds => mds.WithSemicolonToken(semicolonToken),
        PropertyDeclarationSyntax pds => pds.WithSemicolonToken(semicolonToken),
        _ => throw new UnreachableException()
    };

    public MethodOrPropertyDeclarationSyntax WithModifiers(SyntaxTokenList modifiers) => _memberDeclaration switch
    {
        MethodDeclarationSyntax mds => mds.WithModifiers(modifiers),
        PropertyDeclarationSyntax pds => pds.WithModifiers(modifiers),
        _ => throw new UnreachableException()
    };

    public MethodOrPropertyDeclarationSyntax WithTriviaFrom(SyntaxNode node) =>
        _memberDeclaration switch
        {
            MethodDeclarationSyntax mds => mds.WithTriviaFrom(node),
            PropertyDeclarationSyntax pds => pds.WithTriviaFrom(node),
            _ => throw new UnreachableException()
        };

    public ExpressionSyntax CreateCall(SyntaxToken newName) =>
        _memberDeclaration switch
        {
            MethodDeclarationSyntax => InvocationExpression(IdentifierName(newName), ArgumentList()),
            PropertyDeclarationSyntax => IdentifierName(newName),
            _ => throw new UnreachableException()
        };

    public MethodOrPropertyDeclarationSyntax RemoveIfDirectives() => IfDirectiveRemover.Instance.Visit(this);

    public AttributeSyntax GetRegexAttribute()
    {
        if (!Modifiers.Any(static a => a.IsKind(SyntaxKind.PartialKeyword)))
        {
            return null;
        }

        return AttributeLists.SelectMany(static a => a.Attributes)
                          .FirstOrDefault(static a => a.Name is IdentifierNameSyntax
                              {
                                  Identifier.Value: "GeneratedRegex" or "GeneratedRegexAttribute"
                              });
    }

    public static implicit operator MethodOrPropertyDeclarationSyntax(MethodDeclarationSyntax mds) => new(mds);

    public static implicit operator MethodOrPropertyDeclarationSyntax(PropertyDeclarationSyntax mds) => new(mds);

    public static implicit operator MemberDeclarationSyntax(MethodOrPropertyDeclarationSyntax mpds) =>
        mpds._memberDeclaration;

    private sealed class IfDirectiveRemover() : CSharpSyntaxRewriter(true)
    {
        public static IfDirectiveRemover Instance { get; } = new();

        /// <inheritdoc />
        public override SyntaxNode VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node) => null;

        /// <inheritdoc />
        public override SyntaxNode VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node) => null;

        /// <inheritdoc />
        public override SyntaxNode VisitElifDirectiveTrivia(ElifDirectiveTriviaSyntax node) => null;

        public MethodOrPropertyDeclarationSyntax Visit(MethodOrPropertyDeclarationSyntax mpds) =>
            base.Visit(mpds) switch
            {
                MethodDeclarationSyntax mds => mds,
                PropertyDeclarationSyntax pds => pds,
                _ => throw new UnreachableException()
            };
    }
}
