using ExampleProject.Math;

namespace NetCoreTestProject.MSTest;

[TestClass]
public class DummyMathTests
{
    [TestMethod]
    public void DummyMathTestAdd()
    {
        var target = new DummyMath();
        var result = target.Add(1, 4);

        Assert.AreEqual(5, result);
    }

    [TestMethod]
    [DataRow(5, 5, 10)]
    [DataRow(0, 0, 0)]
    public void DummyMathTestAdd(int first, int second, int expected)
    {
        var target = new DummyMath();
        var result = target.Add(first, second);

        Assert.AreEqual(expected, result);
    }
}
