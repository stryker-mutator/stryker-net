using System;
using Stryker.Abstractions.Reporting;

namespace Stryker.Core.Reporters.Json;

public class Position : IPosition
{
    private int _line;
    public int Line
    {
        get => _line;
        set => _line = value > 0 ? value : throw new ArgumentException("Line number must be higher than 0");
    }

    private int _column;
    public int Column
    {
        get => _column;
        set => _column = value > 0 ? value : throw new ArgumentException("Column number must be higher than 0");
    }
}
