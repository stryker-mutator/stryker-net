using System.Collections.Immutable;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

namespace Stryker.Solutions;

public class SolutionFile
{
    private readonly Dictionary<(string buildType, string platform), Dictionary<string, (string buildType, string platform)>> _configurations = [];

    public HashSet<string> GetBuildTypes() => _configurations.Keys.Select(x => x.buildType).ToHashSet();

    public HashSet<string> GetPlatforms() => _configurations.Keys.Select(x => x.platform).ToHashSet();

    public bool ConfigurationExists(string buildType, string? platform = null) => _configurations.Keys.Any(x => x.buildType == buildType && (platform == null || platform == x.platform));

    public IReadOnlyCollection<string> GetProjects(string buildType, string? platform = null)
        => _configurations
            .Where(entry => entry.Key.buildType == buildType && (platform == null || entry.Key.platform == platform))
            .SelectMany(entry => entry.Value.Keys).ToImmutableList();

    private const string DefaultBuildType = "Debug";
    private string? GetBuildType(string? buildType)
    {
        if (!string.IsNullOrWhiteSpace(buildType) || _configurations.Count == 0)
        {
            return buildType;
        }
        return ConfigurationExists(DefaultBuildType) ? DefaultBuildType : _configurations.Keys.First().buildType;
    }
    public IReadOnlyCollection<(string file, string buildType, string platform)> GetProjectsWithDetails(string buildType, string? platform = null)
    {
        buildType = GetBuildType(buildType);
        return _configurations
            .Where(entry => entry.Key.buildType == buildType && (platform == null || entry.Key.platform == platform))
            .SelectMany(entry => entry.Value).Select(p => (p.Key, p.Value.buildType, p.Value.platform))
            .ToImmutableList();
    }

    public static SolutionFile BuildFromProjectList(List<string> projects)
    {
        var result = new SolutionFile();
        // default to Debug|Any CPU
        foreach (var buildType in (string[])["Debug", "Release"])
        {
            var platform = "Any CPU";
            var projectDict = new Dictionary<string, (string buildType, string platform)>();
            foreach (var project in projects)
            {
                projectDict[project] = (buildType, platform);
            }
            result._configurations.Add((buildType, platform), projectDict);
        }
        return result;
    }

    public static SolutionFile? LoadSolution(string path)
    {
        // Implementation to load a solution file
        var serializer = SolutionSerializers.GetSerializerByMoniker(path);

        if (serializer == null)
        {
            throw new InvalidOperationException($"No suitable solution serializer found for the given path ({path}).");
        }

        var solution = serializer.OpenAsync(path, CancellationToken.None).
            ConfigureAwait(false).GetAwaiter().GetResult();
        return AnalyzeSolution(solution);
    }

    private static SolutionFile AnalyzeSolution(SolutionModel solution)
    {
        // extract needed information
        var result = new SolutionFile();
        foreach (var buildType in solution.BuildTypes)
        {
            foreach (var solutionPlatform in solution.Platforms)
            {
                var projects = new Dictionary<string, (string buildType, string platform)>();
                // add all projects that are built with this configuration
                foreach (var solutionProject in solution.SolutionProjects)
                {
                    var (projectBuildType, projectPlatform, isBuilt, _) = solutionProject.GetProjectConfiguration(buildType, solutionPlatform);
                    if (!isBuilt || projectBuildType == null || projectPlatform == null)
                    {
                        continue;
                    }

                    projects[solutionProject.FilePath] = (projectBuildType, projectPlatform);
                }
                if (projects.Count == 0)
                {
                    continue;
                }
                result._configurations.Add((buildType, solutionPlatform), projects);
            }
        }

        return result;
    }
}
