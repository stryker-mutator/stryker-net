using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Utilities.Helpers;

// type based strategy pattern implementation: finds the proper implementation according the type of a given object
// keeping a cache for faster resolution
public class TypeBasedStrategy<T, THandler> where T : class where THandler : class, ITypeHandler<T>
{
    private readonly IDictionary<Type, IList<THandler>> _handlerMapping = new Dictionary<Type, IList<THandler>>();

    public void RegisterHandler(THandler handler)
    {
        if (!_handlerMapping.TryGetValue(handler.ManagedType, out var value))
        {
            value = new List<THandler>();
            _handlerMapping.Add(handler.ManagedType, value);
        }

        value.Add(handler);
    }

    public void RegisterHandlers(List<THandler> handlers)
    {
        foreach (var handler in handlers)
        {
            RegisterHandler(handler);
        }
    }

    public THandler? FindHandler(T item) => FindHandler(item, item.GetType());

    private THandler? FindHandler(T? item, Type type)
    {
        if (item == null)
        {
            return null;
        }
        for (Type? currentType = type; currentType != null; currentType = currentType.BaseType)
        {
            if (!_handlerMapping.TryGetValue(currentType, out var handlers))
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
