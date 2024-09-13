using TargetProject.StrykerFeatures;

namespace NetCoreTestProject.MSTest
{
    [TestClass]
    public class Constructs
    {

        [TestMethod]
        [DataRow(29, false)]
        [DataRow(31, true)]
        public void TestAgeExplicit(int age, bool expired)
        {
            var sut = new KilledMutants { Age = age };
            Assert.IsTrue(expired == sut.IsExpiredBool());
        }
    }
}
