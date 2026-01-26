using Shouldly;
using Stryker.TestRunner.MicrosoftTestPlatform.Models;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class ResponseListenerTests
{
    [TestMethod]
    public void Constructor_ShouldSetRequestId()
    {
        // Arrange
        var requestId = Guid.NewGuid();

        // Act
        var listener = new TestNodeUpdatesResponseListener(requestId, _ => Task.CompletedTask);

        // Assert
        listener.RequestId.ShouldBe(requestId);
    }

    [TestMethod]
    public async Task OnMessageReceiveAsync_ShouldInvokeAction()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var messageReceived = false;
        TestNodeUpdate[]? receivedUpdates = null;

        var listener = new TestNodeUpdatesResponseListener(requestId, updates =>
        {
            messageReceived = true;
            receivedUpdates = updates;
            return Task.CompletedTask;
        });

        var testNode = new TestNode("test1", "Test 1", "test", "discovered");
        var updates = new[] { new TestNodeUpdate(testNode, null) };

        // Act
        await listener.OnMessageReceiveAsync(updates);

        // Assert
        messageReceived.ShouldBeTrue();
        receivedUpdates.ShouldNotBeNull();
        receivedUpdates.Length.ShouldBe(1);
        receivedUpdates[0].Node.Uid.ShouldBe("test1");
    }

    [TestMethod]
    public async Task WaitCompletionAsync_ShouldReturnTrue_WhenCompleted()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var listener = new TestNodeUpdatesResponseListener(requestId, _ => Task.CompletedTask);

        // Act
        var completionTask = listener.WaitCompletionAsync(TimeSpan.FromSeconds(1));

        // Complete the listener using reflection (internal method)
        var completeMethod = typeof(ResponseListener).GetMethod("Complete",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        completeMethod?.Invoke(listener, null);

        var result = await completionTask;

        // Assert
        result.ShouldBeTrue();
    }

    [TestMethod]
    public async Task WaitCompletionAsync_ShouldReturnFalse_WhenTimeout()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var listener = new TestNodeUpdatesResponseListener(requestId, _ => Task.CompletedTask);

        // Act
        var result = await listener.WaitCompletionAsync(TimeSpan.FromMilliseconds(10));

        // Assert
        result.ShouldBeFalse();
    }

    [TestMethod]
    public async Task WaitCompletionAsync_WithoutTimeout_ShouldWaitIndefinitely()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var listener = new TestNodeUpdatesResponseListener(requestId, _ => Task.CompletedTask);

        // Act
        var completionTask = listener.WaitCompletionAsync();

        // Complete the listener using reflection
        var completeMethod = typeof(ResponseListener).GetMethod("Complete",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        completeMethod?.Invoke(listener, null);

        // Wait with a timeout to prevent hanging the test
        var completedInTime = await Task.WhenAny(completionTask, Task.Delay(1000)) == completionTask;

        // Assert
        completedInTime.ShouldBeTrue();
    }

    [TestMethod]
    public async Task WaitCompletionAsync_ShouldHandleCancellation()
    {
        // Arrange
        var requestId = Guid.NewGuid();
        var listener = new TestNodeUpdatesResponseListener(requestId, _ => Task.CompletedTask);
        var cts = new CancellationTokenSource();

        // Act
        var completionTask = listener.WaitCompletionAsync(TimeSpan.FromSeconds(10), cts.Token);
        await cts.CancelAsync();

        var result = await completionTask;

        // Assert
        result.ShouldBeFalse();
        cts.Dispose();
    }
}

