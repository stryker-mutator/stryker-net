using System.Collections.Generic;
using Stryker.Abstractions.Exceptions;

namespace Stryker.Abstractions.Options.Inputs;

public class TestRunnerInput : Input<string>
{
    public override string Default => "vstest";
    protected override string Description => "Specify the test runner to use.";
    protected override IEnumerable<string> AllowedOptions => new[] { "vstest", "mtp" };

    public TestRunner Validate()
    {
        if (SuppliedInput is null)
        {
            return TestRunner.VsTest;
        }

        return SuppliedInput.ToLowerInvariant() switch
        {
            "vstest" => TestRunner.VsTest,
            "mtp" => TestRunner.MicrosoftTestPlatform,
            _ => throw new InputException($"Invalid test runner '{SuppliedInput}'. Valid options are: {string.Join(", ", AllowedOptions)}")
        };
    }
}

