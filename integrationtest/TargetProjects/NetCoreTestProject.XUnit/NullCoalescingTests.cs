using Xunit;

namespace ExampleProject.XUnit
{
    public class NullCoalescingTests
    {
        [Fact]
        public void Test1()
        {
            var sut = new NullCoalescing();
            var a = "X";
            var b = "X"; // This test is incorrect. This should've been 'Y'. We expect Stryker to detect this.
            var result = sut.DoIt(a, b);
            Assert.Equal("X", result);
        }

        [Fact]
        public void Test2()
        {
            var sut = new NullCoalescing();
            var b = "Y";
            var result = sut.DoIt(null, b);
            Assert.Equal("Y", result);
        }

        [Fact]
        public void Test3()
        {
            var sut = new NullCoalescing();
            var a = "X";
            var result = sut.DoIt(a, null);
            Assert.Equal("X", result);
        }
    }
}
