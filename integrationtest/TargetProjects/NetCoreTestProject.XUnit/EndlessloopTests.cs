using TargetProject.StrykerFeatures;
using Xunit;

namespace ExampleProject.XUnit
{
    public class EndlessLoopTests
    {
        [Fact]
        public void Loop()
        {
            var target = new Timeout();
            target.SomeLoop();
        }
    }
}
