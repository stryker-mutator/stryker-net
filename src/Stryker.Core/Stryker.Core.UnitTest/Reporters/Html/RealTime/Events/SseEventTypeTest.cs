using System;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.Reporters.Html.RealTime.Events;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Reporters.Html.RealTime.Events;

[TestClass]
public class SseEventTypeTest : TestBase
{
    [TestMethod]
    public void ShouldSerializeFinishedCorrectly() =>
        SseEventType.Finished.Serialize().ShouldBeEquivalentTo("finished");

    [TestMethod]
    public void ShouldSerializeMutantTestedCorrectly() =>
        SseEventType.MutantTested.Serialize().ShouldBeEquivalentTo("mutant-tested");

    [TestMethod]
    public void ShouldThrowWhenPassingUnknownEnum() =>
        Should.Throw<ArgumentException>(() => ((SseEventType)100).Serialize())
            .Message.ShouldBeSemantically("Invalid SseEventType given: 100");
}
