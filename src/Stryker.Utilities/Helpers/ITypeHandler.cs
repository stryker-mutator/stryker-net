using System;

namespace Stryker.Utilities.Helpers;

// describe a strategy that is specialized in a given type
// and support sub-specialization 
public interface ITypeHandler<in T>
{
    // supported type
    Type ManagedType { get; }
    // return true to confirm it can handle the give instance
    bool CanHandle(T t);
}
