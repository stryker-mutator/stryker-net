using Xunit;

namespace ExampleProject.XUnit
{
    public class BadTestExample
    {
        [Fact]
        public void DummyCalc_ShouldDoSomeCalc()
        {
            var target = new DummyCalc();
            var result = target.SomeCalc(1, 2);

            // this test contains no assert statement
        }
    }
}
