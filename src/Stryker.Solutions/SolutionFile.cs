using System.Collections.Immutable;
using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

namespace Stryker.Solutions;
public interface ISolutionProvider
{
    SolutionFile GetSolution(string solutionPath);
}

public class SolutionProvider: ISolutionProvider
{
    public SolutionFile GetSolution(string solutionPath) => SolutionFile.GetSolution(solutionPath);
}

public class SolutionFile
{
    private readonly Dictionary<(string buildType, string platform), Dictionary<string, (string buildType, string platform)>> _configurations = [];

    public HashSet<string> GetBuildTypes() => _configurations.Keys.Select(x => x.buildType).ToHashSet();

    /// <summary>
    /// Checks if a given configuration (and optional platform) is defined in the solution
    /// </summary>
    /// <param name="buildType">solution configuration name (Debug, Release...)</param>
    /// <param name="platform">platform (AnyCPU, x86, x64...)</param>
    /// <returns>true if at least one project exists in this configuration (and platform if specified)</returns>
    public bool ConfigurationExists(string buildType, string? platform = null) => _configurations.Keys.Any(x => x.buildType == buildType && (platform == null || platform == x.platform));

    /// <summary>
    /// Gets all projects for a given solution configuration (and optional platform)
    /// </summary>
    /// <param name="buildType">solution configuration name (Debug, Release...)</param>
    /// <param name="platform">platform (AnyCPU, x86, x64...)</param>
    /// <returns>A collection of projects' filenames that are defined for this configuration.</returns>
    public IReadOnlyCollection<string> GetProjects(string buildType, string? platform = null)
        => _configurations
            .Where(entry => entry.Key.buildType == buildType && (platform == null || entry.Key.platform == platform))
            .SelectMany(entry => entry.Value.Keys).ToImmutableList();

    private const string DefaultBuildType = "Debug";

    private string? GetBuildType(string? buildType)
    {
        if (!string.IsNullOrWhiteSpace(buildType))
        {
            return buildType;
        }
        return (_configurations.Count == 0 || ConfigurationExists(DefaultBuildType)) ? DefaultBuildType : _configurations.Keys.First().buildType;
    }

    /// <summary>
    /// Gets all projects for a given solution configuration (and optional platform) with project details
    /// </summary>
    /// <param name="effectiveBuildType">configuration name (Debug, Release...)</param>
    /// <param name="platform">platform (AnyCPU, x86, x64...)</param>
    /// <returns>A collection of tuples with the projects' filename, project's configuration and project's platform for the provided criteria .</returns>
    public IReadOnlyCollection<(string file, string buildType, string platform)> GetProjectsWithDetails(string effectiveBuildType, string? platform = null)
    {
        effectiveBuildType = GetBuildType(effectiveBuildType);
        return _configurations
            .Where(entry => entry.Key.buildType == effectiveBuildType && (platform == null || entry.Key.platform == platform))
            .SelectMany(entry => entry.Value).Select(p => (p.Key, p.Value.buildType, p.Value.platform))
            .ToImmutableList();
    }

    /// <summary>
    /// Create a solution file from a list of projects, assuming all projects are built in Debug|Any CPU
    /// </summary>
    /// <param name="projects">list of csproj filenames</param>
    /// <returns>a solution instance</returns>
    /// <remarks>this method is used for testing purposes, as the underlying solution parser do not support any form of mocking</remarks>
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

    /// <summary>
    /// Loads a solution file from disk
    /// </summary>
    /// <param name="solutionPath">path to a sln or slnx file</param>
    /// <returns>a solution instance</returns>
    /// <exception cref="InvalidOperationException">if the solution file format is not supported</exception>
    public static SolutionFile GetSolution(string solutionPath)
    {
        // Implementation to load a solution file
        var serializer = SolutionSerializers.GetSerializerByMoniker(solutionPath);

        return serializer == null ? throw new InvalidOperationException($"No suitable solution serializer found for the given path ({solutionPath}).")
            : AnalyzeSolution(serializer.OpenAsync(solutionPath, CancellationToken.None).GetAwaiter().GetResult());
    }
}
