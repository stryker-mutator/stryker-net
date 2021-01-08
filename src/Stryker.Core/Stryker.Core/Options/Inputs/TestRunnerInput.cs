using Stryker.Core.Exceptions;
using Stryker.Core.TestRunners;
using System;

namespace Stryker.Core.Options.Inputs
{
    // Deprecated, might be removed soon
    public class TestRunnerInput : OptionDefinition<string, TestRunner>
    {
        public override StrykerOption Type => StrykerOption.TestRunner;
        public override string DefaultInput => "vstest";
        public override TestRunner DefaultValue => new TestRunnerInput(DefaultInput).Value;

        protected override string Description => "Choose which testrunner should be used to run your tests.";
        protected override string HelpOptions => FormatEnumHelpOptions();

        public TestRunnerInput() { }
        public TestRunnerInput(string testRunner)
        {
            if (testRunner is { })
            {
                if (Enum.TryParse(testRunner, true, out TestRunner result))
                {
                    Value = result;
                }
                else
                {
                    throw new StrykerInputException($"The given test runner ({testRunner}) is invalid.");
                }
            }
        }
    }
}
