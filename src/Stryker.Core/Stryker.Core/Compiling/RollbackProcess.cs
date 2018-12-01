using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace Stryker.Core.Compiling
{
    public interface IRollbackProcess
    {
        RollbackProcessResult Start(CSharpCompilation compiler, ImmutableArray<Diagnostic> diagnostics);
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
        }

        public RollbackProcessResult Start(CSharpCompilation compiler, ImmutableArray<Diagnostic> diagnostics)
        {
            _rollbackedIds = new List<int>();

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

            // remove the broken mutations from the syntaxtrees
            foreach(var syntaxTreeMap in syntaxTreeMapping.Where(x => x.Value.Any()))
            {
                _logger.LogDebug("Rollbacking mutations from {0}", syntaxTreeMap.Key.FilePath);
                _logger.LogTrace("source {1}", syntaxTreeMap.Key.ToString());
                var updatedSyntaxTree = RemoveMutantIfStatements(syntaxTreeMap.Key, syntaxTreeMap.Value);

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
            var annotation = node.GetAnnotations(MutantPlacer.MutationMakers);
            using (var scan= annotation.GetEnumerator())
            {
                if (scan.MoveNext())
                {
                    var data = scan.Current.Data;
                    var mutantId = int.Parse(data);
                    _logger.LogDebug("Found id {0} in MutantIf annotation", mutantId);
                    return mutantId;
                }
                else
                {
                    return null;
                }
            }
        }

        private static SyntaxNode FindEnclosingMember(SyntaxNode node)
        {
            if (node.Kind() == SyntaxKind.MethodDeclaration)
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

        private SyntaxTree RemoveMutantIfStatements(SyntaxTree originalTree, ICollection<Diagnostic> diagnosticInfo)
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
                    _logger.LogError("Unable to rollback mutation for node {0} with diagnostic message {1}", brokenMutation, diagnostic.GetMessage());
                    _logger.LogWarning("Rolling back all mutations in method.", brokenMutation, diagnostic.GetMessage());
                    // backup, remove all mutants in the node

                    var scan = new Dictionary<SyntaxNode, int>();
                    var initNode = FindEnclosingMember(brokenMutation) ?? brokenMutation;
                    ScanAllMutationsIfsAndIds(initNode, scan);
                    foreach (var entry in scan)
                    {
                        if (!brokenMutations.Contains(entry.Key))
                        {
                            brokenMutations.Add(entry.Key);
                            _rollbackedIds.Add(entry.Value);
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
                if (nodeToRemove is IfStatementSyntax)
                {
                    trackedTree = trackedTree.ReplaceNode(nodeToRemove, MutantPlacer.RemoveByIfStatement(nodeToRemove));
                } else if (nodeToRemove is ConditionalExpressionSyntax)
                {
                    trackedTree = trackedTree.ReplaceNode(nodeToRemove, MutantPlacer.RemoveByConditionalExpression(nodeToRemove));
                }
            }
            return trackedTree.SyntaxTree;
        }
    }
}
