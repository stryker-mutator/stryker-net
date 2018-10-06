using NUnit.Framework;

namespace ExampleProject.NUnit
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void TestMethod1()
        {
            var target = new DummyMath();
            var result = target.Add(1, 4);
            Assert.That(result == 5);
        }
    }
}
