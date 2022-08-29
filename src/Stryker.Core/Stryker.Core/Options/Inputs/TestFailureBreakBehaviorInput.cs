using System.Collections.Generic;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs
{
    public class TestFailureBreakBehaviorInput : Input<string>
    {
        public override string Default => "when-half";

        protected override string Description => "Initial test run failure behavior.";
        protected override IEnumerable<string> AllowedOptions => new[] { "never", "when-any", "when-half" };

        public TestFailureBreakBehavior Validate()
        {
            if (SuppliedInput is not null)
            {
                return SuppliedInput.ToLower() switch
                {
                    "never" => TestFailureBreakBehavior.Never,
                    "when-any" => TestFailureBreakBehavior.WhenAny,
                    "when-half" => TestFailureBreakBehavior.WhenHalf,
                    _ => throw new InputException($"Incorrect initial test run behavior ({SuppliedInput}). The behaviors are [Never, When-Any, When-Half]")
                };
            }

            return TestFailureBreakBehavior.WhenHalf;
        }
    }
}
