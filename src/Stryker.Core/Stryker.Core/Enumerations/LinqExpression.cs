using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Enumerations
{
    /// <summary>
    ///     Enumeration for the different kinds of linq expressions
    /// </summary>
    public enum LinqExpression
    {
        FirstOrDefault,
        SingleOrDefault,
        First,
        Last,
        All,
        Any,
        Skip,
        Take,
        SkipWhile,
        TakeWhile,
        Min,
        Max,
        Sum,
        Count
    }
}
