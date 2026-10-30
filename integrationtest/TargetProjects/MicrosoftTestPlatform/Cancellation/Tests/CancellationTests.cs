using CancellationTarget;

namespace CancellationTests;

[TestClass]
[DoNotParallelize]
public sealed class CancellationTests(TestContext context)
{
    private static int _retryInvocations;
    private static string Mode => Environment.GetEnvironmentVariable("STRYKER_CANCELLATION_SCENARIO") ?? "cooperative";

    [TestMethod]
    [Retry(2)]
    public void A_KillingFailure()
    {
        Mark("kill-start");
        if (Mode == "crash" && !Subject.IsEnabled())
        {
            Environment.Exit(17);
        }
        if (Mode is not ("timeout" or "legacy"))
        {
            Assert.IsTrue(Subject.IsEnabled());
        }
        Mark("kill-finished");
    }

    [TestMethod]
    public void B_LegacyFailure()
    {
        if (Mode == "legacy")
        {
            Assert.IsTrue(Subject.IsEnabled());
        }
    }

    [TestMethod]
    [Retry(2)]
    public void C_RetryFailureThenPass()
    {
        _ = Subject.IsEnabled();
        if (Interlocked.Increment(ref _retryInvocations) % 2 == 1)
        {
            Assert.Fail("This attempt is intentionally superseded by a passing retry.");
        }
    }

    [TestMethod]
    [Retry(2)]
    public void D_BaselineFailure()
    {
        if (Mode == "baseline")
        {
            Assert.Fail("This failure is also present without mutations.");
        }
    }

    [TestMethod]
    public async Task E_SlowSelectedTest()
    {
        Mark("slow-start");
        _ = Subject.Add(1);
        _ = Subject.Unobserved();
        var enabled = Subject.IsEnabled();
        try
        {
            if (Mode == "timeout" && !enabled)
            {
                await Task.Delay(Timeout.Infinite, CancellationToken.None);
            }
            await Task.Delay(TimeSpan.FromSeconds(2),
                Mode == "noncooperative" ? CancellationToken.None : context.CancellationToken);
            Mark("slow-finished");
        }
        finally
        {
            Mark("cleanup-start");
            if (Mode == "held-cleanup")
            {
                var directory = Environment.GetEnvironmentVariable("STRYKER_CANCELLATION_EVIDENCE")
                    ?? throw new InvalidOperationException("Held cleanup requires an evidence directory.");
                while (!File.Exists(Path.Combine(directory, "release-cleanup")))
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(10), CancellationToken.None);
                }
            }
            await Task.Delay(TimeSpan.FromMilliseconds(50), CancellationToken.None);
            Mark("cleanup-finished");
        }
    }

    private static void Mark(string marker)
    {
        var directory = Environment.GetEnvironmentVariable("STRYKER_CANCELLATION_EVIDENCE");
        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
            File.AppendAllText(Path.Combine(directory, $"{Environment.ProcessId}.events"),
                $"{DateTime.UtcNow:O}|{marker}{Environment.NewLine}");
            File.WriteAllText(Path.Combine(directory, $"{Environment.ProcessId}.{marker}"), DateTime.UtcNow.ToString("O"));
        }
    }
}
