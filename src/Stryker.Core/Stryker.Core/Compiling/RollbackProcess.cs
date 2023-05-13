using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;

namespace Stryker.Core.Compiling
{
    public interface IRollbackProcess
    {
        RollbackProcessResult Start(CSharpCompilation compiler, ImmutableArray<Diagnostic> diagnostics, bool lastAttempt, bool devMode);
    }

    /// <summary>
    /// Responsible for rolling back all mutations that prevent compiling the mutated assembly
    /// </summary>
    public class RollbackProcess : IRollbackProcess
    {
        private List<int> RolledBackIds { get; }
        private ILogger Logger { get; }

        public RollbackProcess()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<RollbackProcess>();
            RolledBackIds = new List<int>();
        }

        public RollbackProcessResult Start(CSharpCompilation compiler, ImmutableArray<Diagnostic> diagnostics, bool lastAttempt, bool devMode)
        {
            // match the diagnostics with their syntax trees
            var syntaxTreeMapping = compiler.SyntaxTrees.ToDictionary<SyntaxTree, SyntaxTree, ICollection<Diagnostic>>(syntaxTree => syntaxTree, _ => new Collection<Diagnostic>());

            foreach (var diagnostic in diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error))
            {
                syntaxTreeMapping[diagnostic.Location.SourceTree].Add(diagnostic);
            }

            // remove the broken mutations from the syntax trees
            foreach (var syntaxTreeMap in syntaxTreeMapping.Where(x => x.Value.Any()))
            {
                var originalTree = syntaxTreeMap.Key;
                if (devMode)
                {
                    DumpBuildErrors(syntaxTreeMap);
                }
                else
                {
                    Logger.LogTrace($"RollBacking mutations from {originalTree.FilePath}.");
                }

                Logger.LogTrace("source {1}", originalTree);
                var updatedSyntaxTree = RemoveMutations(originalTree, syntaxTreeMap.Value, devMode);

                if (updatedSyntaxTree == originalTree && (lastAttempt || devMode))
                {
                    Logger.LogCritical(
                        "Stryker.NET could not compile the project after mutation. This is probably an error for Stryker.NET and not your project. Please report this issue on github with the previous error message.");
                    throw new CompilationException("Internal error due to compile error.");
                }

                Logger.LogTrace("RolledBack to {0}", updatedSyntaxTree.ToString());

                // update the compiler object with the new syntax tree
                compiler = compiler.ReplaceSyntaxTree(originalTree, updatedSyntaxTree);
            }

            // by returning the same compiler object (with different syntax trees) the next compilation will use Roslyn's incremental compilation
            return new RollbackProcessResult()
            {
                Compilation = compiler,
                RollbackedIds = RolledBackIds
            };
        }

        // search is this node contains or is within a mutation
        private (SyntaxNode, int) FindMutationIfAndId(SyntaxNode startNode)
        {
            var info = ExtractMutationInfo(startNode);
            if (info.Id != null)
            {
                return (startNode, info.Id.Value);
            }
            for (var node = startNode; node != null; node = node.Parent)
            {
                info = ExtractMutationInfo(node);
                if (info.Id != null)
                {
                    return (node, info.Id.Value);
                }
            }

            // scan within the expression
            return startNode is ExpressionSyntax ? FindMutationInChildren(startNode) : (null, -1);
        }

        // search the first mutation within the node
        private (SyntaxNode, int) FindMutationInChildren(SyntaxNode startNode)
        {
            foreach (var node in startNode.ChildNodes())
            {
                var info = ExtractMutationInfo(node);
                if (info.Id != null)
                {
                    return (node, info.Id.Value);
                }
            }

            foreach (var node in startNode.ChildNodes())
            {
                var (subNode, mutantId) = FindMutationInChildren(node);
                if (subNode != null)
                {
                    return (subNode, mutantId);
                }
            }

            return (null, -1);
        }

        private MutantInfo ExtractMutationInfo(SyntaxNode node)
        {
            var info = MutantPlacer.FindAnnotations(node);

            if (info.Engine == null)
            {
                return new MutantInfo();
            }

            Logger.LogDebug("Found mutant {id} of type '{type}' controlled by '{engine}'.", info.Id, info.Type, info.Engine);

            return info;
        }

        private static SyntaxNode FindEnclosingMember(SyntaxNode node)
        {
            for (var currentNode = node; currentNode != null; currentNode = currentNode.Parent)
            {
                if (currentNode.IsKind(SyntaxKind.MethodDeclaration) || currentNode.IsKind(SyntaxKind.GetAccessorDeclaration) || currentNode.IsKind(SyntaxKind.SetAccessorDeclaration) || currentNode.IsKind(SyntaxKind.ConstructorDeclaration))
                {
                    return currentNode;
                }
            }
            // return the all file if not found
            return node.SyntaxTree.GetRoot();
        }

        private IList<MutantInfo> ScanAllMutationsIfsAndIds(SyntaxNode node)
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
            Logger.LogInformation($"Roll backing mutations from {syntaxTreeMap.Key.FilePath}.");
            var sourceLines = syntaxTreeMap.Key.ToString().Split("\n");
            foreach (var diagnostic in syntaxTreeMap.Value)
            {
                var fileLinePositionSpan = diagnostic.Location.GetMappedLineSpan();
                Logger.LogInformation($"Error :{diagnostic.GetMessage()}, {fileLinePositionSpan}");
                for (var i = Math.Max(0, fileLinePositionSpan.StartLinePosition.Line - 1);
                    i <= Math.Min(fileLinePositionSpan.EndLinePosition.Line + 1, sourceLines.Length - 1);
                    i++)
                {
                    Logger.LogInformation($"{i + 1}: {sourceLines[i]}");
                }
            }

            Logger.LogInformation(Environment.NewLine);
        }

        private SyntaxTree RemoveMutations(SyntaxTree originalTree, IEnumerable<Diagnostic> diagnosticInfo, bool devMode)
        {
            var rollbackRoot = originalTree.GetRoot();
            // find all if statements to remove
            var brokenMutations = IdentifyMutations(diagnosticInfo, rollbackRoot, out var diagnostics);

            if (brokenMutations.Count == 0)
            {
                if (devMode)
                {
                    Logger.LogCritical("Stryker.NET will stop (due to dev-mode option sets to true)");
                    return originalTree;
                }

                // we were unable to identify any mutation that could have caused the build issue(s)
                brokenMutations = ScanForSuspiciousMutations(diagnostics, rollbackRoot);
            }

            // mark the broken mutation nodes to track
            var trackedTree = rollbackRoot.TrackNodes(brokenMutations);
            foreach (var brokenMutation in brokenMutations)
            {
                // find the mutated node in the new tree
                var nodeToRemove = trackedTree.GetCurrentNode(brokenMutation);
                // remove the mutated node using its MutantPlacer remove method and update the tree
                trackedTree = trackedTree.ReplaceNode(nodeToRemove, MutantPlacer.RemoveMutant(nodeToRemove));
            }
            return trackedTree.SyntaxTree;
        }

        private Collection<SyntaxNode> ScanForSuspiciousMutations(Diagnostic[] diagnostics, SyntaxNode rollbackRoot)
        {
            var suspiciousMutations = new Collection<SyntaxNode>();
            foreach (var diagnostic in diagnostics)
            {
                var brokenMutation = rollbackRoot.FindNode(diagnostic.Location.SourceSpan);
                var initNode = FindEnclosingMember(brokenMutation);
                var scan = ScanAllMutationsIfsAndIds(initNode);

                if (scan.Any(x => x.Type == Mutator.Block.ToString()))
                {
                    // we remove all block mutation first
                    foreach (var mutant in scan.Where(x =>
                                 x.Type == Mutator.Block.ToString() && !suspiciousMutations.Contains(x.Node)))
                    {
                        suspiciousMutations.Add(mutant.Node);
                        RolledBackIds.Add(mutant.Id.Value);
                    }
                }
                else
                {
                    // we have to remove every mutation
                    var errorLocation = diagnostic.Location.GetMappedLineSpan();
                    Logger.LogWarning(
                        "Stryker.NET encountered a compile error in {0} (at {1}:{2}) with message: {3} (Source code: {4})",
                        errorLocation.Path, errorLocation.StartLinePosition.Line,
                        errorLocation.StartLinePosition.Character, diagnostic.GetMessage(), brokenMutation);

                    Logger.LogWarning(
                        "Safe Mode! Stryker will try to continue by rolling back all mutations in method. This should not happen, please report this as an issue on github with the previous error message.");
                    // backup, remove all mutations in the node

                    foreach (var mutant in scan.Where(mutant => !suspiciousMutations.Contains(mutant.Node)))
                    {
                        suspiciousMutations.Add(mutant.Node);
                        if (mutant.Id != -1)
                        {
                            RolledBackIds.Add(mutant.Id.Value);
                        }
                    }
                }
            }

            return suspiciousMutations;
        }

        private Collection<SyntaxNode> IdentifyMutations(IEnumerable<Diagnostic> diagnosticInfo, SyntaxNode rollbackRoot, out Diagnostic[] diagnostics)
        {
            var brokenMutations = new Collection<SyntaxNode>();
            diagnostics = diagnosticInfo as Diagnostic[] ?? diagnosticInfo.ToArray();
            foreach (var diagnostic in diagnostics)
            {
                var brokenMutation = rollbackRoot.FindNode(diagnostic.Location.SourceSpan);
                var (mutationIf, mutantId) = FindMutationIfAndId(brokenMutation);
                if (mutationIf == null || brokenMutations.Contains(mutationIf))
                {
                    continue;
                }

                brokenMutations.Add(mutationIf);
                if (mutantId >= 0)
                {
                    RolledBackIds.Add(mutantId);
                }
            }

            return brokenMutations;
        }
    }
}
