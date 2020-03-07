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
        private List<int> _rollbackedIds { get; set; }
        private ILogger _logger { get; set; }

        public RollbackProcess()
        {
            _logger = ApplicationLogging.LoggerFactory.CreateLogger<RollbackProcess>();
            _rollbackedIds = new List<int>();
        }

        public RollbackProcessResult Start(CSharpCompilation compiler, ImmutableArray<Diagnostic> diagnostics, bool lastAttempt, bool devMode)
        {
            // match the diagnostics with their syntaxtrees
            var syntaxTreeMapping = new Dictionary<SyntaxTree, ICollection<Diagnostic>>();
            foreach (var syntaxTree in compiler.SyntaxTrees)
            {
                syntaxTreeMapping.Add(syntaxTree, new Collection<Diagnostic>());
            }
            foreach (var diagnostic in diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error))
            {
                syntaxTreeMapping[diagnostic.Location.SourceTree].Add(diagnostic);
            }

            // remove the broken mutations from the syntax trees
            foreach(var syntaxTreeMap in syntaxTreeMapping.Where(x => x.Value.Any()))
            {
                if (devMode)
                {
                    DumpBuildErrors(syntaxTreeMap);
                }
                else
                {
                    _logger.LogTrace($"Roll backing mutations from {syntaxTreeMap.Key.FilePath}.");
                }

                _logger.LogTrace("source {1}", syntaxTreeMap.Key.ToString());
                var updatedSyntaxTree = RemoveMutantIfStatements(syntaxTreeMap.Key, syntaxTreeMap.Value, lastAttempt, devMode);

                _logger.LogTrace("Rollbacked to {0}", updatedSyntaxTree.ToString());

                // update the compiler object with the new syntaxtree
                compiler = compiler.ReplaceSyntaxTree(syntaxTreeMap.Key, updatedSyntaxTree);
            }

            // by returning the same compiler object (with different syntax trees) the next compilation will use Roslyn's incremental compilation
            return new RollbackProcessResult() {
                Compilation = compiler,
                RollbackedIds = _rollbackedIds
            };
        }

        private (SyntaxNode, int) FindMutationIfAndId(SyntaxNode node)
        {
            var id = ExtractMutationIfAndId(node);
            if (id == null)
            {
                if (node.Parent == null)
                {
                    return (null, 0);
                }

                return FindMutationIfAndId(node.Parent);
            }

            return (node, id.Value);
        }

        private int? ExtractMutationIfAndId(SyntaxNode node)
        {
            var annotation = node.GetAnnotations(MutantPlacer.MutationMarkers);
            using (var scan= annotation.GetEnumerator())
            {
                if (scan.MoveNext())
                {
                    var data = scan.Current.Data;
                    var mutantId = int.Parse(data);
                    _logger.LogDebug("Found id {0} in MutantIf annotation", mutantId);
                    return mutantId;
                }

                return null;
            }
        }

        private static SyntaxNode FindEnclosingMember(SyntaxNode node)
        {
            if (node.Kind() == SyntaxKind.MethodDeclaration || node.Kind() == SyntaxKind.GetAccessorDeclaration || node.Kind() == SyntaxKind.SetAccessorDeclaration)
            {
                return node;
            }

            if (node.Parent == null)
            {
                return null;
            }

            return FindEnclosingMember(node.Parent);
        }

        private void ScanAllMutationsIfsAndIds(SyntaxNode node,  IDictionary<SyntaxNode, int> scan)
        {
            var id = ExtractMutationIfAndId(node);
            if (id != null)
            {
                scan[node] = id.Value;
            }

            foreach (var childNode in node.ChildNodes())
            {
                ScanAllMutationsIfsAndIds(childNode, scan);
            }
        }

        private void DumpBuildErrors(KeyValuePair<SyntaxTree, ICollection<Diagnostic>> syntaxTreeMap)
        {
            _logger.LogInformation($"Roll backing mutations from {syntaxTreeMap.Key.FilePath}.");
            var sourceLines = syntaxTreeMap.Key.ToString().Split("\n");
            foreach (var diagnostic in syntaxTreeMap.Value)
            {
                var fileLinePositionSpan = diagnostic.Location.GetMappedLineSpan();
                _logger.LogInformation($"Error :{diagnostic.GetMessage()}, {fileLinePositionSpan.ToString()}");
                for (var i = Math.Max(0, fileLinePositionSpan.StartLinePosition.Line - 1);
                    i <= Math.Min(fileLinePositionSpan.EndLinePosition.Line + 1, sourceLines.Length - 1);
                    i++)
                {
                    _logger.LogInformation($"{i + 1}: {sourceLines[i]}");
                }
            }

            _logger.LogInformation(Environment.NewLine);
        }

        private SyntaxTree RemoveMutantIfStatements(SyntaxTree originalTree, ICollection<Diagnostic> diagnosticInfo,
            bool lastAttempt, bool devMode)
        {
            var rollbackRoot = originalTree.GetRoot();
            // find all if statements to remove
            var brokenMutations = new Collection<SyntaxNode>();
            foreach (var diagnostic in diagnosticInfo)
            {
                var brokenMutation = rollbackRoot.FindNode(diagnostic.Location.SourceSpan);
                var (mutationIf, mutantId) = FindMutationIfAndId(brokenMutation);
                if (mutationIf == null)
                {
                    var errorLocation = diagnostic.Location.GetMappedLineSpan();
                    _logger.LogWarning(
                        "Stryker.NET encountered an compile error in {0} (at {1}:{2}) with message: {3} (Source code: {4})",
                        errorLocation.Path, errorLocation.StartLinePosition.Line,
                        errorLocation.StartLinePosition.Character, diagnostic.GetMessage(), brokenMutation);
                    if (devMode)
                    {
                        _logger.LogCritical("Stryker.NET will stop (due to dev-mode option sets to true)");
                        throw new StrykerCompilationException("Internal error due to compile error.");
                    }
                    _logger.LogWarning("Safe Mode! Stryker will try to continue by rolling back all mutations in method. This should not happen, please report this as an issue on github with the previous error message.");
                    // backup, remove all mutants in the node

                    var scan = new Dictionary<SyntaxNode, int>();
                    var initNode = FindEnclosingMember(brokenMutation) ?? brokenMutation;
                    ScanAllMutationsIfsAndIds(initNode, scan);

                    if (!scan.Any() && lastAttempt)
                    {
                        // Not able to rollback anything
                        _logger.LogCritical("Stryker.NET could not compile the project after mutation. This is probably an error for Stryker.NET and not your project. Please report this issue on github with the previous error message.");
                        throw new StrykerCompilationException("Internal error due to compile error.");
                    }

                    foreach (var (key, value) in scan)
                    {
                        if (!brokenMutations.Contains(key))
                        {
                            brokenMutations.Add(key);
                            _rollbackedIds.Add(value);
                        }
                    }
                }
                else
                {
                    if (!brokenMutations.Contains(mutationIf))
                    {
                        brokenMutations.Add(mutationIf);
                        _rollbackedIds.Add(mutantId);
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
