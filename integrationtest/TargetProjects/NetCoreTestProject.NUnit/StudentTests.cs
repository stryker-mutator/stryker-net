using NUnit.Framework;
using TargetProject;

namespace NetCoreTestProject.NUnit
{
    public class StudentTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var target = new Student();
            target.Age = 29;
            var result = target.IsExpired();
            Assert.AreEqual("No", result);
        }
    }
}