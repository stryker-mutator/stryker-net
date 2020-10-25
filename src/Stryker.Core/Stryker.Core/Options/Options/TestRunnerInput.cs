using Stryker.Core.Exceptions;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;

namespace Stryker.Core.Options.Options
{
    // Deprecated, might be removed soon
    public class TestRunnerInput : ComplexStrykerInput<TestRunner, string>
    {
        static TestRunnerInput()
        {
            HelpText = $"Choose which testrunner should be used to run your tests. | { FormatOptionsString(DefaultInput, (IEnumerable<TestRunner>)Enum.GetValues(DefaultValue.GetType())) }";
            DefaultInput = "vstest";
            DefaultValue = new TestRunnerInput(DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.TestRunner;

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
