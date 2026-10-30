using Target.NetFx;

namespace MSTest.NetFx;

[TestClass]
public class NumberTests
{
    [TestMethod]
    [DataRow(0, false)]
    [DataRow(1, true)]
    public void IsPositive(int value, bool expected)
    {
        Assert.AreEqual(expected, Number.IsPositive(value));
    }
}
