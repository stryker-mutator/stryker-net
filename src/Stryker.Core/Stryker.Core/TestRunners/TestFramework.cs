using System;

namespace Stryker.Core.TestRunners
{
    [Flags]
    public enum TestFramework
    {
        MsTest = 1,
        xUnit = 2,
        NUnit = 4
    }
}
