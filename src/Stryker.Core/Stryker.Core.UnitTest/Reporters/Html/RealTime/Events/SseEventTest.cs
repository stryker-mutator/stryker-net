using Stryker.Configuration.Reporters.Html.RealTime.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Reporters.Html.RealTime.Events;

[TestClass]
public class SseEventTest : TestBase
{
    [TestMethod]
    public void ShouldSerializeFinishedCorrectly()
    {
        var @event = new SseEvent<string>
        {
            Event = SseEventType.Finished,
            Data = ""
        };

        @event.Serialize().ShouldBeSemantically("event:finished\ndata:\"\"");
    }

    [TestMethod]
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

    [TestMethod]
    public void ShouldSerializeMutantWhitespaceCorrectly()
    {
        var @object = new { Id = "1", Status = "Survived", Replacement = "Stryker was here!" };
        var @event = new SseEvent<object>
        {
            Event = SseEventType.MutantTested,
            Data = @object
        };

        @event.Serialize().ShouldBeSemantically("event:mutant-tested\ndata:{\"id\":\"1\",\"status\":\"Survived\",\"replacement\":\"Stryker was here!\"}");
    }
}
