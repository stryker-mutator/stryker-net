using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Core.Helpers;
using Stryker.Core.Mutants;
using Stryker.Utilities.Logging;

namespace Stryker.Core.Compiling;

public interface ICSharpRollbackProcess
{
    CSharpRollbackProcessResult Start(Compilation compiler, ImmutableArray<Diagnostic> diagnostics,
        Mode mode, bool devMode);

    SyntaxTree CleanUpFile(SyntaxTree file);

    enum Mode
    {
        Normal,
        Aggressive,
        LastChance
    }
}

/// <summary>
/// Responsible for rolling back all mutations that prevent compiling the mutated assembly
/// </summary>
public class CSharpRollbackProcess : ICSharpRollbackProcess
{
    private List<int> RollBackedIds { get; } = [];
    private ILogger Logger { get; } = ApplicationLogging.LoggerFactory.CreateLogger<CSharpRollbackProcess>();

    public CSharpRollbackProcessResult Start(Compilation compiler, ImmutableArray<Diagnostic> diagnostics,
        ICSharpRollbackProcess.Mode mode, bool devMode)
    {
        // match the diagnostics with their syntax trees
        var syntaxTreeMapping =
            compiler.SyntaxTrees.ToDictionary<SyntaxTree, SyntaxTree, ICollection<Diagnostic>>(
                syntaxTree => syntaxTree, _ => new Collection<Diagnostic>());

        foreach (var diagnostic in diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error))
        {
            if (diagnostic.Location.SourceTree == null)
            {
                Logger.LogWarning("General compilation error: {Message}", diagnostic.GetMessage());
                continue;
            }

            syntaxTreeMapping[diagnostic.Location.SourceTree].Add(diagnostic);
        }

        // remove the broken mutations from the syntax trees
        foreach (var syntaxTreeMap in syntaxTreeMapping.Where(x => x.Value.Count != 0))
        {
            var originalTree = syntaxTreeMap.Key;
            Logger.LogDebug("RollBacking mutations from {FilePath}.", originalTree.FilePath);
            if (devMode)
            {
                DumpBuildErrors(syntaxTreeMap);
                Logger.LogTrace("source {OriginalTree}", originalTree);
            }

            var updatedSyntaxTree = RemoveCompileErrorMutations(originalTree, syntaxTreeMap.Value, mode).SyntaxTree;

            if (updatedSyntaxTree == originalTree || mode == ICSharpRollbackProcess.Mode.LastChance)
            {
                Logger.LogCritical(
                    "Stryker.NET could not compile the project after mutation. This is probably an error for Stryker.NET and not your project. Please report this issue on github with the previous error message.");
                throw new CompilationException("Internal error due to compile error.");
            }

            Logger.LogTrace("RolledBack to {UpdatedSyntaxTree}", updatedSyntaxTree.ToString());

            // update the compiler object with the new syntax tree
            compiler = compiler.ReplaceSyntaxTree(originalTree, updatedSyntaxTree);
        }

        // by returning the same compiler object (with different syntax trees) the next compilation will use Roslyn's incremental compilation
        return new CSharpRollbackProcessResult(
            compiler,
            RollBackedIds);
    }

    // search is this node contains or is within a mutation
    private SyntaxNode FindMutationWithNode(SyntaxNode startNode)
    {
        var info = ExtractMutationInfo(startNode);
        if (info.Id != null)
        {
            return startNode;
        }

        for (var node = startNode; node != null; node = node.Parent)
        {
            info = ExtractMutationInfo(node);
            if (info.Id != null)
            {
                return node;
            }
        }

        // scan within the expression
        return startNode is ExpressionSyntax ? FindMutationInChildren(startNode) : null;
    }

    // search the first mutation within the node
    private static SyntaxNode FindMutationInChildren(SyntaxNode startNode) => MutantPlacer.GetAllMutations(startNode).FirstOrDefault().node;

    private MutantInfo ExtractMutationInfo(SyntaxNode node)
    {
        var info = MutantPlacer.FindAnnotations(node);

        if (info.Engine == null)
        {
            return new MutantInfo();
        }

        Logger.LogDebug("Found mutant {id} of type '{type}' controlled by '{engine}'.", info.Id, info.Type,
            info.Engine);

        return info;
    }

    private static SyntaxNode FindEnclosingMember(SyntaxNode node)
    {
        for (var currentNode = node; currentNode != null; currentNode = currentNode.Parent)
        {
            if (currentNode.IsKind(SyntaxKind.MethodDeclaration) ||
                currentNode.IsKind(SyntaxKind.GetAccessorDeclaration) ||
                currentNode.IsKind(SyntaxKind.SetAccessorDeclaration) ||
                currentNode.IsKind(SyntaxKind.ConstructorDeclaration))
            {
                return currentNode;
            }
        }

        // return the whole file if not found
        return node.SyntaxTree.GetRoot();
    }

    private List<MutantInfo> ScanAllMutationsIfsAndIds(SyntaxNode node)
    {
        var scan = new List<MutantInfo>();
        foreach (var childNode in node.ChildNodes())
        {
            scan.AddRange(ScanAllMutationsIfsAndIds(childNode));
        }

        var info = ExtractMutationInfo(node);
        if (info.Id != null)
        {
            scan.Add(info);
        }

        return scan;
    }

    private SyntaxNode RemoveCompileErrorMutations(SyntaxTree originalTree, IEnumerable<Diagnostic> diagnosticInfo,
        ICSharpRollbackProcess.Mode mode)
    {
        var rollbackRoot = originalTree.GetRoot();
        // find all if statements to remove
        var brokenMutations =
            IdentifyMutationsAndFlagForRollback(diagnosticInfo, rollbackRoot, mode, out var diagnostics);

        if (brokenMutations.Count == 0)
        {
            // we were unable to identify any mutation that could have caused the build issue(s)
            brokenMutations = ScanForSuspiciousMutations(diagnostics, rollbackRoot);
        }

        return RollTheseMutationsBack(rollbackRoot, brokenMutations);
    }

    // roll back mutations
    private SyntaxNode RollTheseMutationsBack(SyntaxNode rollbackRoot, Collection<SyntaxNode> brokenMutations)
    {
        // mark the broken mutation nodes to track
        var trackedTree = rollbackRoot.TrackNodes(brokenMutations);
        foreach (var brokenMutation in brokenMutations)
        {
            // find the mutated node in the new tree
            var nodeToRemove = trackedTree.GetCurrentNode(brokenMutation);
            if (nodeToRemove == null)
            {
                // already removed
                continue;
            }

            var info = MutantPlacer.FindAnnotations(nodeToRemove);
            if (info.Id.HasValue && info.Id != -1)
            {
                RollBackedIds.Add(info.Id.Value);
            }
            // remove the mutated node using its MutantPlacer remove method and update the tree
            trackedTree = trackedTree.ReplaceNode(nodeToRemove, MutantPlacer.RemoveMutant(nodeToRemove));
        }

        return trackedTree;
    }

    // identify mutations that may have caused compilation errors
    private Collection<SyntaxNode> ScanForSuspiciousMutations(Diagnostic[] diagnostics, SyntaxNode rollbackRoot)
    {
        var suspiciousMutations = new Collection<SyntaxNode>();
        foreach (var diagnostic in diagnostics)
        {
            var brokenMutation = rollbackRoot.FindNode(diagnostic.Location.SourceSpan);
            var initNode = FindEnclosingMember(brokenMutation);
            var scan = ScanAllMutationsIfsAndIds(initNode);

            if (scan.Any(x => x.Type == nameof(Mutator.Block)))
            {
                // we remove all block mutation on first attempt
                foreach (var mutant in scan.Where(x =>
                             x.Type == nameof(Mutator.Block) && !suspiciousMutations.Contains(x.Node)))
                {
                    suspiciousMutations.Add(mutant.Node);
                }
            }
            else
            {
                // we have to remove every mutation
                var errorLocation = diagnostic.Location.GetMappedLineSpan();
                Logger.LogWarning(
                    "An unidentified mutation in {Path} resulted in a compile error (at {Line}:{StartCharacter}) with id: {DiagnosticId}, message: {Message} (Source code: {BrokenMutation})",
                    errorLocation.Path, errorLocation.StartLinePosition.Line,
                    errorLocation.StartLinePosition.Character, diagnostic.Id,
                    diagnostic.GetMessage(), brokenMutation);

                Logger.LogInformation(
                    "Safe Mode! Stryker will remove all mutations in {DisplayName} and mark them as 'compile error'.",
                    DisplayName(initNode));
                // backup, remove all mutations in the node
                foreach (var mutant in scan.Where(mutant => !suspiciousMutations.Contains(mutant.Node)))
                {
                    suspiciousMutations.Add(mutant.Node);
                }
            }
        }

        return suspiciousMutations;
    }

    // removes all mutation from a file
    public SyntaxTree CleanUpFile(SyntaxTree file)
    {
        var rollbackRoot = file.GetRoot();
        var scan = ScanAllMutationsIfsAndIds(rollbackRoot);
        var suspiciousMutations = new Collection<SyntaxNode>();
        foreach (var mutant in scan.Where(mutant => !suspiciousMutations.Contains(mutant.Node)))
        {
            suspiciousMutations.Add(mutant.Node);
        }
        return file.WithRootAndOptions(RollTheseMutationsBack(rollbackRoot, suspiciousMutations), file.Options);
    }

    private static string DisplayName(SyntaxNode initNode) =>
        initNode switch
        {
            MethodDeclarationSyntax method => $"{method.Identifier}",
            ConstructorDeclarationSyntax constructor => $"{constructor.Identifier}",
            AccessorDeclarationSyntax accessor => $"{accessor.Keyword} {accessor.Keyword}",
            not null => initNode.Parent == null ? "whole file" : "the current node",
        };

    private Collection<SyntaxNode> IdentifyMutationsAndFlagForRollback(IEnumerable<Diagnostic> diagnosticInfo,
        SyntaxNode rollbackRoot, ICSharpRollbackProcess.Mode mode, out Diagnostic[] diagnostics)
    {
        var brokenMutations = new Collection<SyntaxNode>();
        diagnostics = diagnosticInfo as Diagnostic[] ?? diagnosticInfo.ToArray();
        foreach (var diagnostic in diagnostics)
        {
            var brokenMutation = rollbackRoot.FindNode(diagnostic.Location.SourceSpan);
            // handles uninitialized variables
            if (diagnostic.Id is "CS0165" or "CS0177")
            {
                var identifierText = ExtractIdentifier(diagnostic, brokenMutation);
                if (!string.IsNullOrEmpty(identifierText)
                    && ScanErasingMutation(
                        x => x.AssignsThis(identifierText), brokenMutation, brokenMutations, mode))
                {
                    continue;
                }
            }
            // handles missing return statement
            else if (diagnostic.Id is "CS0161")
            {
                if (brokenMutation is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    // CS0161 implies a block body
                    brokenMutation = methodDeclarationSyntax.Body!.Statements.Last();
                }
                if (ScanErasingMutation(x => x is ReturnStatementSyntax, brokenMutation, brokenMutations, mode))
                {
                    continue;
                }
            }
            // general case, assume the diagnostic location is within the mutation.
            RegisterMutationForRollback(FindMutationWithNode(brokenMutation), brokenMutations);
        }

        return brokenMutations;
    }

    private bool ScanErasingMutation(Func<SyntaxNode, bool> predicate,
        SyntaxNode brokenMutation, Collection<SyntaxNode> brokenMutations, ICSharpRollbackProcess.Mode mode)
    {
        var count = brokenMutations.Count;
        do
        {
            // scanning previous siblings
            foreach (var previous in brokenMutation.GetPreviousSiblings().Reverse())
            {
                var mutations = MutantPlacer.GetMutationsEngines(previous);
                // we check if a mutation hides
                foreach (var node in mutations.Where(entry => !brokenMutations.Contains(entry.node)
                                                              && entry.engine.Erases(entry.node, predicate)).
                             Select(entry => entry.node))
                {
                    RegisterMutationForRollback(node, brokenMutations);
                    if (mode == ICSharpRollbackProcess.Mode.Normal)
                    {
                        // remove only one in normal mode
                        return true;
                    }
                }
            }
            // we reached where we are
            brokenMutation = brokenMutation.Parent;

        } while (brokenMutation is not null && brokenMutation is not MemberDeclarationSyntax);
        return count < brokenMutations.Count;
    }

    private string ExtractIdentifier(Diagnostic diagnostic, SyntaxNode brokenMutation)
    {
        var arguments = diagnostic.GetType().GetProperty("Arguments",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) // NOSONAR
            ?.GetValue(diagnostic) as object[];
        var identifierText = arguments?[0] as string;
        if (identifierText != null)
        {
            return identifierText;
        }

        if (diagnostic.Id == "CS0165" && brokenMutation is IdentifierNameSyntax identifierNameSyntax)
        {
            identifierText = identifierNameSyntax.Identifier.Text;
        }
        else
        {
            Logger.LogInformation(
                "Unable to extract the identifier for uninitialized variable error, fallback to default rollback logic. Diagnostic: {DiagnosticId}, message: {Message}",
                diagnostic.Id, diagnostic.GetMessage());
        }

        return identifierText;
    }

    private void RegisterMutationForRollback(SyntaxNode mutation, Collection<SyntaxNode> brokenMutations)
    {
        if (mutation == null || brokenMutations.Contains(mutation))
        {
            return;
        }
        if (MutantPlacer.RequiresRemovingChildMutations(mutation))
        {
            var scan = ScanAllMutationsIfsAndIds(mutation);
            foreach (var mutant in scan.Where(mutant => !brokenMutations.Contains(mutant.Node)))
            {
                brokenMutations.Add(mutant.Node);
            }
        }
        else
        {
            brokenMutations.Add(mutation);
        }
    }

    private void DumpBuildErrors(KeyValuePair<SyntaxTree, ICollection<Diagnostic>> syntaxTreeMap)
    {
        Logger.LogDebug("Dumping build error in file");
        var sourceLines = syntaxTreeMap.Key.ToString().Split("\n");
        foreach (var diagnostic in syntaxTreeMap.Value)
        {
            var fileLinePositionSpan = diagnostic.Location.GetMappedLineSpan();
            Logger.LogDebug("Error :{Message}, {fileLinePositionSpan}",
                diagnostic.GetMessage(), fileLinePositionSpan);
            for (var i = Math.Max(0, fileLinePositionSpan.StartLinePosition.Line - 1);
                 i <= Math.Min(fileLinePositionSpan.EndLinePosition.Line + 1, sourceLines.Length - 1);
                 i++)
            {
                Logger.LogDebug("{index}: {sourceLine}", i + 1, sourceLines[i]);
            }
        }

        Logger.LogDebug(Environment.NewLine);
    }
}
