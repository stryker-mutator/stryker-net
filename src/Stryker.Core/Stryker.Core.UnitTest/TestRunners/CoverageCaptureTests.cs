using Shouldly;
using System;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners;

public class CoverageCaptureTests : TestBase
{
    private static bool WaitFor(object lck, Func<bool> predicate, int timeout)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        while (stopwatch.ElapsedMilliseconds < timeout)
        {
            lock (lck)
            {
                if (predicate())
                {
                    return true;
                }
                Monitor.Wait(lck, (int)Math.Max(0, timeout - stopwatch.ElapsedMilliseconds));
            }
        }

        return predicate();
    }


    [Fact]
    public void CanParseConfiguration()
    {
        var referenceConf="<Parameters><Environment name=\"ActiveMutant\" value=\"1\"/></Parameters>";
        var node = new XmlDocument();

        node.LoadXml(referenceConf);

        node.ChildNodes.Count.ShouldBe(1);
        var coolChild = node.GetElementsByTagName("Parameters");
        coolChild[0].Name.ShouldBe("Parameters");
        var envVars = node.GetElementsByTagName("Environment");

        envVars.Count.ShouldBe(1);
    }
}
