using System;

namespace Stryker.Abstractions.Reporting;

public interface IPosition : IEquatable<IPosition>
{
    int Column { get; set; }
    int Line { get; set; }
}
