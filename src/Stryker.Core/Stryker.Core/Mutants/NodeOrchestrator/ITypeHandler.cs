using System;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    internal interface ITypeHandler<in T>
    {
        Type ManagedType { get; }

        bool CanHandle(T t);
    }
}