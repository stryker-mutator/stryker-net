using System;
using Shouldly;
using Xunit;

namespace Stryker.CLI.UnitTest
{
    public class CLIOptionsTests
    {
        [Fact]
        public void ShouldHaveDefaultValues()
        {
            CLIOptions.MaxConcurrentTestRunners.DefaultValue.ShouldBe(Math.Max(Environment.ProcessorCount / 2, 1));
            CLIOptions.IgnoreMethods.DefaultValue.ShouldBeEmpty();
            CLIOptions.LogToFile.DefaultValue.ShouldBeFalse();
            CLIOptions.LogLevel.DefaultValue.ShouldBe("info");
            CLIOptions.MutationLevel.DefaultValue.ShouldBe("Standard");
            CLIOptions.Diff.DefaultValue.ShouldBeFalse();
            CLIOptions.DevMode.DefaultValue.ShouldBeFalse();
            CLIOptions.ThresholdBreak.DefaultValue.ShouldBe(0);
            CLIOptions.ThresholdLow.DefaultValue.ShouldBe(60);
            CLIOptions.ThresholdHigh.DefaultValue.ShouldBe(80);
            CLIOptions.AdditionalTimeoutMS.DefaultValue.ShouldBe(5000);
        }
    }
}
