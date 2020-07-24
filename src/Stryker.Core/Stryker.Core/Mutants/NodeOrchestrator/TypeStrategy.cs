using System;
using System.Collections.Generic;

namespace Stryker.Core.Mutants.NodeOrchestrator
{
    // 
    internal class TypeBasedStrategy<T, THandler> where THandler: class, ITypeHandler<T>
    {
        private readonly IDictionary<Type, THandler> _mappingCache = new Dictionary<Type, THandler>();

        public void RegisterHandler(THandler handler)
        {
            _mappingCache[handler.ManagedType] = handler;
        }

        public THandler FindHandler(Type type)
        {
            if (type == null)
            {
                return null;
            }

            if (_mappingCache.TryGetValue(type, out var result))
            {
                return result;
            }

            result = FindHandler(type.BaseType);
            _mappingCache[type] = result;

            return result;
        }
    }
}
