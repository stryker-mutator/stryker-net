using Microsoft.VisualStudio.TestTools.UnitTesting;
using TargetProject.Constructs;
using TargetProject.Defects;

namespace TargetProject.MTP;

[TestClass]
public class SampleTests
{
    [TestMethod]
    public void ExampleChain_ReturnsExpectedChar()
    {
        var c = new CSharp1().ExampleChain();
        Assert.AreEqual('S', c);
    }

    [TestMethod]
    public void Employee_Methods_DoNotThrow()
    {
        var e = new CSharp2.Employee();
        e.DoWork();
        e.GoToLunch();
        Assert.IsTrue(true); // If the methods threw, the test would fail.
    }

    [TestMethod]
    public void WhileTrue_Loop_ReturnsTrueWhenStopTrue()
    {
        var result = WhileTrue.Loop(true);
        Assert.IsTrue(result);
    }
}
