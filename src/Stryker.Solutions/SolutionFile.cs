using Microsoft.VisualStudio.SolutionPersistence.Model;
using Microsoft.VisualStudio.SolutionPersistence.Serializer;

namespace Stryker.Solutions;

public class SolutionFile
{
    private readonly Dictionary<(string buildType, string platform), List<((string, string), string)>> _configurations = [];


    public HashSet<string> GetBuildTypes() => _configurations.Keys.Select(x => x.buildType).ToHashSet();

    public HashSet<string> GetPlatforms() => _configurations.Keys.Select(x => x.platform).ToHashSet();


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
                var projects = new List<((string buildType, string platform), string path)>();
                // add all projects that are built with this configuration
                foreach (var solutionProject in solution.SolutionProjects)
                {
                    var (projectBuildType, projectPlatform, isBuilt, _) = solutionProject.GetProjectConfiguration(buildType, solutionPlatform);
                    if (!isBuilt)
                    {
                        continue;
                    }

                    projects.Add(((projectBuildType, projectPlatform), solutionProject.FilePath));
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
