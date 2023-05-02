using Stryker.Core.Reporters.Html.Realtime.Events;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters.Html.Realtime.Events;

public class SseEventTest : TestBase
{
    [Fact]
    public void ShouldSerializeFinishedCorrectly()
    {
        var @event = new SseEvent<string>
        {
            Event = SseEventType.Finished,
            Data = ""
        };

        @event.Serialize().ShouldBeSemantically("event:finished\ndata:\"\"");
    }

    [Fact]
    public void ShouldSerializeMutantTestedCorrectly()
    {
        var @object = new { Id = "1", Status = "Survived" };
        var @event = new SseEvent<object>
        {
            Event = SseEventType.MutantTested,
            Data = @object
        };

        @event.Serialize().ShouldBeSemantically("event:mutant-tested\ndata:{\"id\":\"1\",\"status\":\"Survived\"}");
    }
}
