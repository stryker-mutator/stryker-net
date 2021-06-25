using Xunit;

namespace ExampleProject.XUnit
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
