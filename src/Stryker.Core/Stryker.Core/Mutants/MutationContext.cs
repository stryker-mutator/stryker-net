using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Mutants
{
    /// <summary>
    /// Describe the (syntax tree) context during mutation
    /// </summary>
    public class MutationContext: IDisposable
    {
        private static readonly ILogger Logger;
        private readonly CsharpMutantOrchestrator _mainOrchestrator;
        private readonly MutationContext _ancestor;
        public readonly List<Mutant> ExpressionLevelMutations = new List<Mutant>();
        public readonly List<Mutant> BlockLevelControlledMutations = new List<Mutant>();
        public readonly List<Mutant> StatementLevelControlledMutations = new List<Mutant>();

        static MutationContext()
        {
            Logger = ApplicationLogging.LoggerFactory.CreateLogger<MutationContext>();
        }

        public MutationContext(CsharpMutantOrchestrator mutantOrchestrator)
        {
            _mainOrchestrator = mutantOrchestrator;
        }

        private MutationContext(MutationContext parent)
        {
            _ancestor = parent;
            _mainOrchestrator = parent._mainOrchestrator;
            InStaticValue = parent.InStaticValue;
        }

        /// <summary>
        ///  True when inside a static initializer, fields or accessor.
        /// </summary>
        public bool InStaticValue { get; set; }

        public bool MustInjectCoverageLogic => _mainOrchestrator.MustInjectCoverageLogic;

        public bool HasBlockLevelMutant => BlockLevelControlledMutations.Count > 0;
        
        public bool HasStatementLevelMutant => StatementLevelControlledMutations.Count > 0 || HasBlockLevelMutant;

        public MutationContext EnterStatic()
        {
            return new MutationContext(this) {InStaticValue = true};
        }

        public MutationContext Clone()
        {
            return new MutationContext(this);
        }

        public void Discard()
        {
            if (HasStatementLevelMutant)
            {
                // some mutants 
                Logger.LogInformation($"{BlockLevelControlledMutations.Count+StatementLevelControlledMutations.Count} mutations were not injected.");
                foreach (var mutant in BlockLevelControlledMutations.Union(StatementLevelControlledMutations))
                {
                    mutant.ResultStatus = MutantStatus.CompileError;
                    mutant.ResultStatusReason = "Stryker was not able to inject mutation in code.";
                }
                BlockLevelControlledMutations.Clear();
                StatementLevelControlledMutations.Clear();
            }

        }

        public void Dispose()
        {
            if (_ancestor == null)
            {
                Discard();
                return;
            }
            // copy the pending mutation to the enclosing context
            _ancestor.StatementLevelControlledMutations.AddRange(StatementLevelControlledMutations);
            _ancestor.BlockLevelControlledMutations.AddRange(BlockLevelControlledMutations);
        }
    }
}
