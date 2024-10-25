using System;

namespace Stryker.Core.TestRunners;

[Flags]
public enum TestFrameworks
{
    MsTest = 1,
    xUnit = 2,
    NUnit = 4
}
