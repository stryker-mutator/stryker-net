using System;

namespace Stryker.Core.TestRunners
{
    [Flags]
    public enum TestFramework
    {
        msTest = 1,
        xUnit = 2,
        nUnit = 4
    }
}
