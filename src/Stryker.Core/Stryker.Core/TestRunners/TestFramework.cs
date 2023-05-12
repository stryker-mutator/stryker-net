namespace Stryker.Core.TestRunners;
using System;

[Flags]
public enum TestFramework
{
    MsTest = 1,
    xUnit = 2,
    NUnit = 4
}
