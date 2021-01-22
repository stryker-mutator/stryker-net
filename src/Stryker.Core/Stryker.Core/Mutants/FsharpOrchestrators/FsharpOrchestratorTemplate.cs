using System;
using System.Collections.Generic;

namespace Stryker.Core.Mutants.FsharpOrchestrators
{
    public interface IFsharpTypehandle<T>
    {
        public T Mutate(T input, FsharpCoreOrchestrator iterator);
    }

    public class OrchestratorFinder<T>
    {
        private readonly IDictionary<Type, IFsharpTypehandle<T>> _handlerMapping = new Dictionary<Type, IFsharpTypehandle<T>>();

        public void Add(Type type, IFsharpTypehandle<T> handler)
        {
            _handlerMapping.Add(type, handler);
        }

        public IFsharpTypehandle<T> FindHandler(Type type)
        {
            IFsharpTypehandle<T> returnable;
            if (_handlerMapping.TryGetValue(type, out returnable))
            {
                return returnable;
            }
            return new DefaultOrchestrator<T>();
        }
    }

    public class DefaultOrchestrator<T> : IFsharpTypehandle<T>
    {
        public T Mutate(T input, FsharpCoreOrchestrator iterator)
        {
            return input;
        }
    }
}
