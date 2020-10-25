using Stryker.Core.Exceptions;
using Stryker.Core.TestRunners;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stryker.Core.Options.Inputs
{
    // Deprecated, might be removed soon
    public class TestRunnerInput : ComplexStrykerInput<string, TestRunner>
    {
        static TestRunnerInput()
        {
            Description = $"Choose which testrunner should be used to run your tests. | { FormatOptions(DefaultInput, ((IEnumerable<TestRunner>)Enum.GetValues(DefaultValue.GetType())).Select(x => x.ToString())) }";
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
