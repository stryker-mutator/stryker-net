using System;

namespace Stryker.Abstractions.Testing;

[Flags]
public enum TestFrameworks
{
    MsTest = 1,
    xUnit = 2,
    NUnit = 4
}
