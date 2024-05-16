using System;

namespace Stryker.Core.Helpers;

// describe a strategy that is specialized in a given type
// and support sub-specialization 
internal interface ITypeHandler<in T>
{
    // supported type
    Type ManagedType { get; }
    // return true to confirm it can handle the give instance
    bool CanHandle(T t);
}
