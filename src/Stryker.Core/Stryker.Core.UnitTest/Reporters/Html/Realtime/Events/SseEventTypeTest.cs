using System;
using Shouldly;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Html.Realtime.Events;

public class SseEventTypeTest : TestBase
{
    [Fact]
    public void ShouldSerializeFinishedCorrectly() =>
        SseEventType.Finished.Serialize().ShouldBeEquivalentTo("finished");

    [Fact]
    public void ShouldSerializeMutantTestedCorrectly() =>
        SseEventType.MutantTested.Serialize().ShouldBeEquivalentTo("mutant-tested");

    [Fact]
    public void ShouldThrowWhenPassingUnknownEnum() =>
        Should.Throw<ArgumentException>(() => ((SseEventType)100).Serialize())
            .Message.ShouldBeSemantically("Invalid SseEventType given: 100");
}
