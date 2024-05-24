using NUnit.Framework;
using NUnit.Framework.Legacy;
using TargetProject.StrykerFeatures;

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
            var target = new StrykerComments
            {
                Age = 29
            };
            var result = target.IsExpired();
            ClassicAssert.AreEqual("No", result);
        }
    }
}
