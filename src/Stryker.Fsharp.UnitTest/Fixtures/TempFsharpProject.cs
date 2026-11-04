using Buildalyzer;

namespace Stryker.Fsharp.UnitTest.Fixtures;

/// <summary>A real two-file F# library in a temp dir, analysed by Buildalyzer (needs the dotnet SDK).</summary>
internal sealed class TempFsharpProject : IDisposable
{
    public string Directory { get; } = Path.Combine(Path.GetTempPath(), "stryker-fsharp-" + Guid.NewGuid().ToString("N"));
    public IAnalyzerResult AnalyzerResult { get; }

    public TempFsharpProject()
    {
        System.IO.Directory.CreateDirectory(Directory);
        // B.fs depends on A.fs: compile order A then B is semantic.
        File.WriteAllText(Path.Combine(Directory, "A.fs"), "module A\nlet one = 1\n");
        File.WriteAllText(Path.Combine(Directory, "B.fs"), "module B\nlet two = A.one + 1\n");
        File.WriteAllText(Path.Combine(Directory, "Lib.fsproj"), """
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup><TargetFramework>net10.0</TargetFramework><Version>1.2.3</Version></PropertyGroup>
              <ItemGroup><Compile Include="A.fs" /><Compile Include="B.fs" /></ItemGroup>
            </Project>
            """);
        var project = new AnalyzerManager().GetProject(Path.Combine(Directory, "Lib.fsproj"));
        AnalyzerResult = project.Build().First(r => r.Succeeded);
    }

    public void Dispose()
    {
        try { System.IO.Directory.Delete(Directory, true); } catch (IOException) { }
    }
}
