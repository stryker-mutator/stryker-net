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
        => _configurations.Where(entry => entry.Key.buildType == buildType && (platform == null || entry.Key.platform == platform)).SelectMany( entry => entry.Value.Keys).ToImmutableList();

    public static SolutionFile? LoadSolution(string path)
    {
        // Implementation to load a solution file
        // This is a placeholder for the actual implementation
        var serializer = SolutionSerializers.GetSerializerByMoniker(path);

        if (serializer == null)
        {
            return null;
        }

        SolutionModel solution;
        try
        {
            solution = serializer.OpenAsync(path, CancellationToken.None).Result;

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }

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
