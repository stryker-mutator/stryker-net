using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Stryker.Core.Exceptions;

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
        private const int _dummyId = int.MinValue;

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
            foreach(var syntaxTreeMap in syntaxTreeMapping.Where(x => x.Value.Any()))
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
                var updatedSyntaxTree = RemoveMutantIfStatements(originalTree, syntaxTreeMap.Value, devMode);

                if (updatedSyntaxTree == originalTree && (lastAttempt || devMode))
                {
                    Logger.LogCritical(
                        "Stryker.NET could not compile the project after mutation. This is probably an error for Stryker.NET and not your project. Please report this issue on github with the previous error message.");
                    throw new StrykerCompilationException("Internal error due to compile error.");
                }

                Logger.LogTrace("RolledBack to {0}", updatedSyntaxTree.ToString());

                // update the compiler object with the new syntax tree
                compiler = compiler.ReplaceSyntaxTree(originalTree, updatedSyntaxTree);
            }

            // by returning the same compiler object (with different syntax trees) the next compilation will use Roslyn's incremental compilation
            return new RollbackProcessResult() {
                Compilation = compiler,
                RollbackedIds = RolledBackIds
            };
        }

        private (SyntaxNode, int) FindMutationIfAndId(SyntaxNode startNode)
        {
            for (var node = startNode; node != null; node = node.Parent)
            {
                var id = ExtractMutationIfAndId(node);
                if (id != null) 
                {
                    return (node, id.Value);
                }
            }

            return (null, 0);
        }

        private int? ExtractMutationIfAndId(SyntaxNode node)
        {
            var (engine, id) = MutantPlacer.FindEngine(node);

            if (engine == null)
            {
                return null;
            }

            Logger.LogDebug(id == -1 ? $"Found a helper: {engine}." : $"Found id {id} in {engine} annotation.");

            return id;
        }

        private static SyntaxNode FindEnclosingMember(SyntaxNode node)
        {
            for(var currentNode = node; currentNode != null; currentNode = currentNode.Parent)
            {
                if (currentNode.Kind() == SyntaxKind.MethodDeclaration || currentNode.Kind() == SyntaxKind.GetAccessorDeclaration || currentNode.Kind() == SyntaxKind.SetAccessorDeclaration)
                {
                    return currentNode;
                }
            }

            return null;
        }

        private void ScanAllMutationsIfsAndIds(SyntaxNode node,  IList<(SyntaxNode, int)> scan)
        {
            var id = ExtractMutationIfAndId(node);
            if (id != null)
            {
                scan.Add((node, id.Value));
            }

            foreach (var childNode in node.ChildNodes())
            {
                ScanAllMutationsIfsAndIds(childNode, scan);
            }
        }

        private void DumpBuildErrors(KeyValuePair<SyntaxTree, ICollection<Diagnostic>> syntaxTreeMap)
        {
            Logger.LogInformation($"Roll backing mutations from {syntaxTreeMap.Key.FilePath}.");
            var sourceLines = syntaxTreeMap.Key.ToString().Split("\n");
            foreach (var diagnostic in syntaxTreeMap.Value)
            {
                var fileLinePositionSpan = diagnostic.Location.GetMappedLineSpan();
                Logger.LogInformation($"Error :{diagnostic.GetMessage()}, {fileLinePositionSpan.ToString()}");
                for (var i = Math.Max(0, fileLinePositionSpan.StartLinePosition.Line - 1);
                    i <= Math.Min(fileLinePositionSpan.EndLinePosition.Line + 1, sourceLines.Length - 1);
                    i++)
                {
                    Logger.LogInformation($"{i + 1}: {sourceLines[i]}");
                }
            }

            Logger.LogInformation(Environment.NewLine);
        }

        private SyntaxTree RemoveMutantIfStatements(SyntaxTree originalTree, IEnumerable<Diagnostic> diagnosticInfo, bool devMode)
        {
            var rollbackRoot = originalTree.GetRoot();
            // find all if statements to remove
            var brokenMutations = new Collection<SyntaxNode>();
            var diagnostics = diagnosticInfo as Diagnostic[] ?? diagnosticInfo.ToArray();
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

            if (brokenMutations.Count == 0)
            {
                // we were unable to identify any mutation that could have caused the build issue(s)
                foreach (var diagnostic in diagnostics)
                {
                    var brokenMutation = rollbackRoot.FindNode(diagnostic.Location.SourceSpan);
                    var errorLocation = diagnostic.Location.GetMappedLineSpan();
                    Logger.LogWarning(
                        "Stryker.NET encountered an compile error in {0} (at {1}:{2}) with message: {3} (Source code: {4})",
                        errorLocation.Path, errorLocation.StartLinePosition.Line,
                        errorLocation.StartLinePosition.Character, diagnostic.GetMessage(), brokenMutation);
                    if (devMode)
                    {
                        Logger.LogCritical("Stryker.NET will stop (due to dev-mode option sets to true)");
                        return originalTree;
                    }

                    Logger.LogWarning(
                        "Safe Mode! Stryker will try to continue by rolling back all mutations in method. This should not happen, please report this as an issue on github with the previous error message.");
                    // backup, remove all mutations in the node
                    var scan = new List<(SyntaxNode, int)>();
                    var initNode = FindEnclosingMember(brokenMutation) ?? brokenMutation;
                    ScanAllMutationsIfsAndIds(initNode, scan);

                    foreach (var (key, value) in scan)
                    {
                        if (!brokenMutations.Contains(key))
                        {
                            brokenMutations.Add(key);
                            if (value != -1)
                            {
                                RolledBackIds.Add(value);
                            }
                        }
                    }
                }
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
    }
}
