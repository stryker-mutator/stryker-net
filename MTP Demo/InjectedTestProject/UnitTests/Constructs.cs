using TargetProject;

namespace UnitTests
{
    [TestClass]
    public class Constructs
    {

        [TestMethod]
        [DataRow(99, false)]
        [DataRow(100, true)]
        public void TestAgeExplicit(int age, bool expired)
        {
            var sut = new Class { Age = age };
            Assert.IsTrue(expired == sut.IsExpiredBool());
        }
    }
}
