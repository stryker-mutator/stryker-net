using System;
using System.Collections.Generic;

namespace Stryker.Core.Mutants.FsharpOrchestrators;

public class OrchestratorFinder<T>
{
    private readonly IDictionary<Type, IFsharpTypeHandler<T>> _handlerMapping = new Dictionary<Type, IFsharpTypeHandler<T>>();

    public void Add(Type type, IFsharpTypeHandler<T> handler)
    {
        _handlerMapping.Add(type, handler);
    }

    public IFsharpTypeHandler<T> FindHandler(Type type)
    {
        IFsharpTypeHandler<T> returnable;
        if (_handlerMapping.TryGetValue(type, out returnable))
        {
            return returnable;
        }
        return new DefaultOrchestrator<T>();
    }
}
