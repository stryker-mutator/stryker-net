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
        bool lastAttempt, bool devMode);

    SyntaxTree CleanUpFile(SyntaxTree file);
}

/// <summary>
/// Responsible for rolling back all mutations that prevent compiling the mutated assembly
/// </summary>
public class CSharpRollbackProcess : ICSharpRollbackProcess
{
    private List<int> RollBackedIds { get; } = [];
    private ILogger Logger { get; } = ApplicationLogging.LoggerFactory.CreateLogger<CSharpRollbackProcess>();

    public CSharpRollbackProcessResult Start(Compilation compiler, ImmutableArray<Diagnostic> diagnostics,
        bool lastAttempt, bool devMode)
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
        foreach (var syntaxTreeMap in syntaxTreeMapping.Where(x => x.Value.Any()))
        {
            var originalTree = syntaxTreeMap.Key;
            Logger.LogDebug("RollBacking mutations from {FilePath}.", originalTree.FilePath);
            if (devMode)
            {
                DumpBuildErrors(syntaxTreeMap);
                Logger.LogTrace("source {OriginalTree}", originalTree);
            }

            var updatedSyntaxTree = RemoveCompileErrorMutations(originalTree, syntaxTreeMap.Value).SyntaxTree;

            if (updatedSyntaxTree == originalTree || lastAttempt)
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
    private static SyntaxNode FindMutationInChildren(SyntaxNode startNode)
    {
        var mutation = MutantPlacer.GetAllMutations(startNode).FirstOrDefault();
        return mutation.node;
    }

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

    private SyntaxNode RemoveCompileErrorMutations(SyntaxTree originalTree, IEnumerable<Diagnostic> diagnosticInfo)
    {
        var rollbackRoot = originalTree.GetRoot();
        // find all if statements to remove
        var brokenMutations =
            IdentifyMutationsAndFlagForRollback(diagnosticInfo, rollbackRoot, out var diagnostics);

        if (brokenMutations.Count == 0)
        {
            // we were unable to identify any mutation that could have caused the build issue(s)
            brokenMutations = ScanForSuspiciousMutations(diagnostics, rollbackRoot);
        }

        return RollTheseMutationsBack(rollbackRoot, brokenMutations);
    }

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
        SyntaxNode rollbackRoot, out Diagnostic[] diagnostics)
    {
        var brokenMutations = new Collection<SyntaxNode>();
        diagnostics = diagnosticInfo as Diagnostic[] ?? diagnosticInfo.ToArray();
        foreach (var diagnostic in diagnostics)
        {
            var brokenMutation = rollbackRoot.FindNode(diagnostic.Location.SourceSpan);
            if (diagnostic.Id is "CS0165" or "CS0177" && ScanUninitializedVariable(diagnostic, brokenMutation, brokenMutations))
            {
                continue;
            }

            // find mutation around node in error
            var mutation = FindMutationWithNode(brokenMutation);
            if (mutation == null || brokenMutations.Contains(mutation))
            {
                continue;
            }

            // does it require to remove any child mutation ?
            if (MutantPlacer.RequiresRemovingChildMutations(mutation))
            {
                FlagChildrenMutationsForRollback(mutation, brokenMutations);
            }
            else
            {
                brokenMutations.Add(mutation);
            }
        }

        return brokenMutations;
    }

    private bool ScanUninitializedVariable(Diagnostic diagnostic, SyntaxNode brokenMutation,
        Collection<SyntaxNode> brokenMutations)
    {
        var identifierText = ExtractIdentifier(diagnostic, brokenMutation);
        if (string.IsNullOrEmpty(identifierText))
        {
            return false;
        }
        var count = brokenMutations.Count;
        do
        {
            foreach (var previous in brokenMutation.GetPreviousSiblings().Reverse())
            {
                var mutations = MutantPlacer.GetMutationsEngines(previous);
                // we check if a mutation hides an assignment
                foreach (var node in mutations.Where(entry => !brokenMutations.Contains(entry.node) && entry.engine.ErasesAssignment(entry.node, identifierText)).Select(entry => entry.node))
                {
                    brokenMutations.Add(node);
                }
            }
            // we have a mutation, check if it erases an assignment
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
        if (identifierText == null)
        {
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
        }

        return identifierText;
    }

    private void FlagChildrenMutationsForRollback(SyntaxNode mutation, Collection<SyntaxNode> brokenMutations)
    {
        var scan = ScanAllMutationsIfsAndIds(mutation);
        foreach (var mutant in scan.Where(mutant => !brokenMutations.Contains(mutant.Node)))
        {
            brokenMutations.Add(mutant.Node);
        }
    }
}
