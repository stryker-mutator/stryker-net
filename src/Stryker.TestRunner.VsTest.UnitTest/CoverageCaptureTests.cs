using System.Xml;
using Shouldly;
using Stryker.Core.UnitTest;

namespace Stryker.TestRunner.VsTest.UnitTest;

[TestClass]
public class CoverageCaptureTests : TestBase
{
    [TestMethod]
    public void CanParseConfiguration()
    {
        var referenceConf = "<Parameters><Environment name=\"ActiveMutant\" value=\"1\"/></Parameters>";
        var node = new XmlDocument();

        node.LoadXml(referenceConf);

        node.ChildNodes.Count.ShouldBe(1);
        var coolChild = node.GetElementsByTagName("Parameters");
        coolChild[0].Name.ShouldBe("Parameters");
        var envVars = node.GetElementsByTagName("Environment");

        envVars.Count.ShouldBe(1);
    }
}
