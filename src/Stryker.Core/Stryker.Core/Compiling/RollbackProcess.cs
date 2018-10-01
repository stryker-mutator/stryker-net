﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;
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

            // match the diagnotics with their syntaxtrees
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

            // by returning the same compiler object (with different syntax trees) the next compilation will use Roslyns incremental compilation
            return new RollbackProcessResult() {
                Compilation = compiler,
                RollbackedIds = _rollbackedIds
            };
        }

        private IfStatementSyntax FindMutationIf(SyntaxNode node)
        {
            var annotation = node.GetAnnotations("MutantIf");
            if (annotation.Any() && node is IfStatementSyntax mutantIf)
            {
                string data = annotation.First().Data;
                int mutantId = int.Parse(data);
                _rollbackedIds.Add(mutantId);
                _logger.LogDebug("Found id {0} in MutantIf annotation", mutantId);
                return mutantIf;
            }
            else
            {
                if(node.Parent == null)
                {
                    return null; 
                }
                return FindMutationIf(node.Parent);
            }
        }

        private SyntaxTree RemoveMutantIfStatements(SyntaxTree originalTree, ICollection<Diagnostic> diagnosticInfo)
        {
            var rollbackRoot = originalTree.GetRoot();
            // find all if statements to remove
            var brokenMutations = new Collection<IfStatementSyntax>();
            foreach (var diagnostic in diagnosticInfo)
            {
                var brokenMutation = rollbackRoot.FindNode(diagnostic.Location.SourceSpan);
                var mutationIf = FindMutationIf(brokenMutation);
                if(mutationIf == null)
                {
                    _logger.LogError("Unable to rollback mutation for node {0} with diagnosticmessage {1}", brokenMutation, diagnostic.GetMessage());
                }
                brokenMutations.Add(mutationIf);
            }
            // mark the if statements to track
            var trackedTree = rollbackRoot.TrackNodes(brokenMutations);
            foreach (var brokenMutation in brokenMutations)
            {
                // find the ifstatement in the new tree
                var nodeToRemove = trackedTree.GetCurrentNode(brokenMutation);
                // remove the ifstatement and update the tree
                trackedTree = trackedTree.ReplaceNode(nodeToRemove, nodeToRemove.Else.Statement);
            }
            return trackedTree.SyntaxTree;
        }
    }
}
