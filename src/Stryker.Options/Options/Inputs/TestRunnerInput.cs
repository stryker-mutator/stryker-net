using System;
using System.Collections.Generic;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Testing;

namespace Stryker.Abstractions.Options.Inputs;

public class TestRunnerInput : Input<string>
{
    public override string Default => Testing.TestRunner.VsTest.ToString();

    protected override string Description => "Specify which test runner to use for running tests. VsTest is the default and currently the only fully supported option.";
    protected override IEnumerable<string> AllowedOptions => EnumToStrings(typeof(Testing.TestRunner));

    public Testing.TestRunner Validate()
    {
        if (SuppliedInput is null)
        {
            return Testing.TestRunner.VsTest;
        }
        else if (Enum.TryParse(SuppliedInput, true, out Testing.TestRunner runner))
        {
            return runner;
        }
        else
        {
            throw new InputException($"The given test runner ({SuppliedInput}) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<Testing.TestRunner>)Enum.GetValues(typeof(Testing.TestRunner))))}]");
        }
    }
}
