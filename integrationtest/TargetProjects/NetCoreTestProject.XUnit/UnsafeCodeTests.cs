using ExampleProject;
using Xunit;

namespace NetCoreTestProject.XUnit
{
    public class UnsafeCodeTests
    {
        [Fact]
        public void GetIndexOfArray()
        {
            int[] dummy = new int[] { 10, 20, 30 };
            UnsafeCode unsafeCode = new UnsafeCode();
            Assert.Equal(20, unsafeCode.GetElement(dummy, 1));
            Assert.Equal(30, unsafeCode.GetElement(dummy, 2));
        }
    }
}
