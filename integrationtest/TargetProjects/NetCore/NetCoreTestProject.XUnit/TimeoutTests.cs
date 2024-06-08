using TargetProject.StrykerFeatures;
using Xunit;

namespace ExampleProject.XUnit
{
    public class TimeoutTests
    {
        [Fact]
        public void ThisShouldTimeout()
        {
            var target = new Timeout();
            target.SomeLoop();
        }
    }
}
