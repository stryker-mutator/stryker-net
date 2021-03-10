using ExampleProject;
using Xunit;

namespace NetCoreTestProject.XUnit
{
    public class EndlessLoopTests
    {
        [Fact]
        public void Loop()
        {
            var target = new EndlessLoop();
            target.SomeLoop();
        }
    }
}
