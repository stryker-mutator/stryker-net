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

            var result = sut.IsExpiredBool();

            Assert.IsTrue(expired == result);
        }

        [TestMethod]
        public void GetHelloUtf8_ShouldReturnHello()
        {
            var result = TargetProject.Constructs.CSharp11.GetHelloUtf8();
            var expected = "Hello"u8.ToArray();
            Assert.IsTrue(System.Linq.Enumerable.SequenceEqual(result.ToArray(), expected));
        }
    }
}
