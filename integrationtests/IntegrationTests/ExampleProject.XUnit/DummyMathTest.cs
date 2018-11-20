using Xunit;

namespace ExampleProject.XUnit
{
    public class DummyMathTest
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
