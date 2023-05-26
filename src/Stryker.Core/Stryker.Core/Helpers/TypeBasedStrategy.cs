using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Helpers
{
    // type based strategy pattern implementation: finds the proper implementation according the type of a given object
    // keeping a cache for faster resolution
    internal class TypeBasedStrategy<T, THandler> where THandler : class, ITypeHandler<T>
    {
        private readonly IDictionary<Type, IList<THandler>> _handlerMapping = new Dictionary<Type, IList<THandler>>();

        public void RegisterHandler(THandler handler)
        {
            if (!_handlerMapping.ContainsKey(handler.ManagedType))
            {
                _handlerMapping.Add(handler.ManagedType, new List<THandler>());
            }
            _handlerMapping[handler.ManagedType].Add(handler);
        }

        public void RegisterHandlers(List<THandler> handlers)
        {
            foreach (var handler in handlers)
            {
                RegisterHandler(handler);
            }
        }

        public THandler FindHandler(T item) => FindHandler(item, item.GetType());

        private THandler FindHandler(T item, Type type)
        {
            for (; item != null && type != null; type = type.BaseType)
            {
                if (!_handlerMapping.TryGetValue(type, out var handlers))
                {
                    continue;
                }
                var match =  handlers.FirstOrDefault( th => th.CanHandle(item));
                if (match != null)
                {
                    return match;
                }
            }

            return null;
        }
    }
}
