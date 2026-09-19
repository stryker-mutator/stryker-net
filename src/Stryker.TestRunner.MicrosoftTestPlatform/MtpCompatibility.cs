using System.Collections.Concurrent;
using System.Text.Json;

namespace Stryker.TestRunner.MicrosoftTestPlatform;

internal static class MtpCompatibility
{
    public const string BlockingCoverageEnvironmentVariable = "STRYKER_BLOCKING_COVERAGE_CONSUMER";
    public const string CoverageEpochFileEnvironmentVariable = "STRYKER_COVERAGE_EPOCH_FILE";

    private static readonly Version DynamicExtensionsMinimumVersion = new(2, 4);
    private static readonly ConcurrentDictionary<string, bool> BlockingConsumerSupport = new(StringComparer.OrdinalIgnoreCase);

    public static bool SupportsBlockingDataConsumer(string testApplication) =>
        BlockingConsumerSupport.GetOrAdd(testApplication, static assembly =>
            SupportsBlockingDataConsumerFromDepsFile(Path.ChangeExtension(assembly, ".deps.json")));

    internal static bool SupportsBlockingDataConsumerFromDepsFile(string dependenciesFile)
    {
        if (!File.Exists(dependenciesFile))
        {
            return false;
        }

        try
        {
            using var document = JsonDocument.Parse(File.ReadAllText(dependenciesFile));
            if (!document.RootElement.TryGetProperty("libraries", out var libraries))
            {
                return false;
            }

            foreach (var library in libraries.EnumerateObject())
            {
                const string packagePrefix = "Microsoft.Testing.Platform/";
                if (!library.Name.StartsWith(packagePrefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var versionText = library.Name[packagePrefix.Length..].Split('-', 2)[0];
                return Version.TryParse(versionText, out var version) && version >= DynamicExtensionsMinimumVersion;
            }
        }
        catch (JsonException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        return false;
    }
}
