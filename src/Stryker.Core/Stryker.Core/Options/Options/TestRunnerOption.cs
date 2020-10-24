using Stryker.Core.Exceptions;
using Stryker.Core.TestRunners;
using System;

namespace Stryker.Core.Options.Options
{
    // Deprecated, might be removed soon
    public class TestRunnerOption : BaseStrykerOption<TestRunner>
    {
        public TestRunnerOption(string testRunner)
        {
            if (testRunner is { })
            {
                if (Enum.TryParse(testRunner, true, out TestRunner result))
                {
                    Value = result;
                    return;
                }
                else
                {
                    throw new StrykerInputException($"The given test runner ({testRunner}) is invalid.");
                }
            }
        }

        public override StrykerOption Type => StrykerOption.TestRunner;
        public override string HelpText => "Choose which testrunner should be used to run your tests.";
        public override TestRunner DefaultValue => TestRunner.VsTest;
    }
}
