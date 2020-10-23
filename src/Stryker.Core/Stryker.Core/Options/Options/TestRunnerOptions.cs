using Stryker.Core.Exceptions;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.Options.Options
{
    class TestRunnerOptions : BaseStrykerOption<TestRunner>
    {
        public TestRunnerOptions(string testRunner)
        {
            if (Enum.TryParse(testRunner, true, out TestRunner result))
            {
                Value = result;
            }
            else
            {
                throw new StrykerInputException(ErrorMessage,
                    $"The given test runner ({testRunner}) is invalid. Valid options are: [{string.Join(", ", (IEnumerable<TestRunner>)Enum.GetValues(typeof(TestRunner)))}]");
            }
        }

        public override StrykerOption Type => StrykerOption.TestRunner;
        public override string HelpText => "Choose which testrunner should be used to run your tests.";
        public override TestRunner DefaultValue => TestRunner.VsTest;
    }
}
