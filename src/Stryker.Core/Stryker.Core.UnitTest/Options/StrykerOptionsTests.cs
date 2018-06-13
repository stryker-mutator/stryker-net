using Serilog.Events;
using Shouldly;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class StrykerOptionsTests
    {
        [Fact]
        public void StrykerOptions_ShouldHaveDefaultLoggingOptions()
        {
            var target = new StrykerOptions("", "", "");

            target.LogOptions.LogToFile.ShouldBe(false);
            target.LogOptions.LogLevel.ShouldBe(LogEventLevel.Warning);
        }
    }
}
