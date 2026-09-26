using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Extension;

/// <summary>
/// Registers Stryker's per-test coverage consumer when this assembly is loaded as an MTP dynamic extension.
/// </summary>
public static class BlockingCoverageTestingPlatformBuilderHook
{
    /// <summary>
    /// Adds the blocking coverage consumer to the in-process test host.
    /// </summary>
    public static void AddExtensions(ITestApplicationBuilder builder, string[] arguments) =>
        builder.TestHost.AddDataConsumer(static _ => new BlockingCoverageDataConsumer());
}

#pragma warning disable TPEXP
internal sealed class BlockingCoverageDataConsumer : IBlockingDataConsumer
#pragma warning restore TPEXP
{
    private const string BlockingCoverageEnvironmentVariable = "STRYKER_BLOCKING_COVERAGE_CONSUMER";

    public string Uid { get; } = $"Stryker.BlockingCoverageDataConsumer.{GetAssemblyLocationHash()}";

    public string Version => "1.0.0";

    public string DisplayName => "Stryker blocking coverage consumer";

    public string Description => "Flushes Stryker per-test coverage before an MTP test-result publication completes.";

    public Type[] DataTypesConsumed => new[] { typeof(TestNodeUpdateMessage) };

    public Task<bool> IsEnabledAsync() =>
        Task.FromResult(Environment.GetEnvironmentVariable(BlockingCoverageEnvironmentVariable) is not null);

    public Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        if (value is TestNodeUpdateMessage message
            && message.TestNode.Properties.SingleOrDefault<TestNodeStateProperty>() is not
                (null or DiscoveredTestNodeStateProperty or InProgressTestNodeStateProperty))
        {
            BlockingCoverageFlusher.FlushLoadedMutantControls();
        }

        return Task.CompletedTask;
    }

    private static string GetAssemblyLocationHash()
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(typeof(BlockingCoverageDataConsumer).Assembly.Location));
        return BitConverter.ToString(hash, 0, 8).Replace("-", string.Empty);
    }
}

internal static class BlockingCoverageFlusher
{
    internal const string Marker = "BlockingCoverageConsumer.v1";

    public static void FlushLoadedMutantControls()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var type in GetLoadableTypes(assembly))
            {
                try
                {
                    var marker = type.GetField(
                        "BlockingCoverageConsumerMarker",
                        BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                    if (!string.Equals(marker?.GetRawConstantValue() as string, Marker, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    type.GetMethod(
                            "FlushCoverageToFileForBlockingConsumer",
                            BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        ?.Invoke(null, null);
                }
                catch (Exception exception) when (exception is
                    AmbiguousMatchException or
                    InvalidOperationException or
                    MemberAccessException or
                    TargetInvocationException)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"[Stryker] Failed to flush coverage through injected helper '{type.FullName}': {exception}");
                }
            }
        }
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
        catch (NotSupportedException)
        {
            return Array.Empty<Type>();
        }
        catch (FileLoadException)
        {
            return Array.Empty<Type>();
        }
    }
}
