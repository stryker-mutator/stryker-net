using ExampleProject.Math;
using Xunit;

namespace NetCoreTestProject.XUnit
{
    public class DummyMathTests
    {
        [Fact]
        public void DummyMathTestAdd()
        {
            var target = new DummyMath();
            var result = target.Add(1, 4);

            Assert.Equal(5, result);
        }
    }
}
