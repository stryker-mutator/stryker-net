using Buildalyzer;

namespace Stryker.Fsharp.UnitTest.Fixtures;

/// <summary>A real F# library in a temp dir, analysed by Buildalyzer (needs the dotnet SDK). Defaults to a two-file library.</summary>
internal sealed class TempFsharpProject : IDisposable
{
    public string Directory { get; } = Path.Combine(Path.GetTempPath(), "stryker-fsharp-" + Guid.NewGuid().ToString("N"));
    public IAnalyzerResult AnalyzerResult { get; }

    /// <param name="files">Source files in compile order; defaults to A.fs and B.fs, where B.fs depends on A.fs (order is semantic).</param>
    public TempFsharpProject(params (string Name, string Content)[] files)
    {
        System.IO.Directory.CreateDirectory(Directory);
        if (files.Length == 0)
        {
            files = [("A.fs", "module A\nlet one = 1\n"), ("B.fs", "module B\nlet two = A.one + 1\n")];
        }

        foreach (var (name, content) in files)
        {
            File.WriteAllText(Path.Combine(Directory, name), content);
        }

        var items = string.Concat(files.Select(f => $"<Compile Include=\"{f.Name}\" />"));
        File.WriteAllText(Path.Combine(Directory, "Lib.fsproj"), $$"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup><TargetFramework>net10.0</TargetFramework><Version>1.2.3</Version></PropertyGroup>
              <ItemGroup>{{items}}</ItemGroup>
            </Project>
            """);
        var project = new AnalyzerManager().GetProject(Path.Combine(Directory, "Lib.fsproj"));
        AnalyzerResult = project.Build().First(r => r.Succeeded);
    }

    public void Dispose()
    {
        try
        {
            System.IO.Directory.Delete(Directory, true);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            // a still-mapped assembly can hold the directory on Windows (access denied, not IOException);
            // leave it to the OS temp cleanup, but say so
            Console.WriteLine($"Could not delete temp project {Directory}: {exception.Message}");
        }
    }
}
