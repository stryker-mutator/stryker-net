using Shouldly;
using Stryker.Core.Reporters.Html.Realtime.Events;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Html.Realtime.Events;

public class SseEventTest : TestBase
{
    [Fact]
    public void ShouldSetProperties()
    {
        var sut = new SseEvent<string>
        {
            Event = "mutant-tested",
            Data = "testData",
        };

        sut.Event.ShouldBeEquivalentTo("mutant-tested");
        sut.Data.ShouldBeEquivalentTo("testData");
    }
}
