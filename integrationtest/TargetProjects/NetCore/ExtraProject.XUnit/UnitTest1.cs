using Xunit;

namespace ExtraProject.XUnit
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var target = new Teacher();

            target.AddLesson();

            Assert.Equal(1, target.Lessons);
        }
    }
}
