namespace Stryker.Core.Options.Inputs;

public class TestCaseFilterInput : Input<string>
{
    public override string Default => string.Empty;

    protected override string Description => @"Filters out tests in the project using the given expression.
Uses the syntax for dotnet test --filter option and vstest.console.exe --testcasefilter option.
For more information on running selective tests, see https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests.";

    public string Validate()
        => string.IsNullOrWhiteSpace(SuppliedInput) ? Default : SuppliedInput;
}
