using System.IO.Abstractions;
using System.Reflection;
using Buildalyzer;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Fsharp.UnitTest.Fixtures;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Fsharp.UnitTest;

/// <summary>Integration-style: needs a real dotnet SDK. The analyzer result is a real Buildalyzer run on a temp two-file project.</summary>
[TestClass]
public class FsharpCompilingProcessTests
{
    [TestMethod]
    public void FsharpCompile_UnmutatedProject_ProducesLoadableAssembly()
    {
        using var project = new TempFsharpProject();
        var process = new FsharpCompilingProcess(project.AnalyzerResult, new FileSystem(), NullLogger.Instance);

        var result = process.Compile();

        result.Success.ShouldBeTrue(string.Join("; ", result.Diagnostics.Select(d => d.ToString())));
        var name = AssemblyName.GetAssemblyName(WriteToTemp(result.Assembly));
        name.Name.ShouldBe(project.AnalyzerResult.GetAssemblyName());
        name.Version.ShouldBe(new Version(1, 2, 3, 0));
    }

    [TestMethod]
    public void BuildArguments_DropsPdbAndSecondaryOutputOptions_AbsolutisesOthers()
    {
        var analyzerResult = new Mock<IAnalyzerResult>();
        analyzerResult.Setup(x => x.ProjectFilePath).Returns("/proj/Lib.fsproj");
        analyzerResult.Setup(x => x.CompilerArguments).Returns(["--pdb:obj/Debug/Lib.pdb", "--doc:obj/Lib.xml", "--embed:a.fs", "--keyfile:key.snk", "A.fs"]);
        var process = new FsharpCompilingProcess(analyzerResult.Object, new FileSystem(), NullLogger.Instance);

        var argv = process.BuildArguments("/work/Lib.dll", "/work", new Dictionary<string, string>(), [], []);

        argv.ShouldNotContain(a => a.StartsWith("--pdb:") || a.StartsWith("--doc:") || a.StartsWith("--embed:"));
        argv.ShouldContain(Path.GetFullPath("key.snk", "/proj").Insert(0, "--keyfile:"));
    }

    [TestMethod]
    public void FsharpCompile_SourceOverride_ReplacesFileContent_KeepingCompileOrder()
    {
        using var project = new TempFsharpProject();
        var process = new FsharpCompilingProcess(project.AnalyzerResult, new FileSystem(), NullLogger.Instance);
        var overrides = new Dictionary<string, string> { [Path.Combine(project.Directory, "A.fs")] = "module A\nlet one = 41\n" };

        var result = process.Compile(overrides);

        result.Success.ShouldBeTrue();
        var assembly = Assembly.LoadFile(WriteToTemp(result.Assembly));
        assembly.GetType("B")!.GetProperty("two")!.GetValue(null).ShouldBe(42);
    }

    [TestMethod]
    public void FsharpCompile_OverwritesTestOutput_AndBackupExists()
    {
        using var project = new TempFsharpProject();
        var testOutput = Path.Combine(project.Directory, "testout");
        Directory.CreateDirectory(testOutput);
        var injectionPath = Path.Combine(testOutput, project.AnalyzerResult.GetAssemblyFileName());
        File.WriteAllText(injectionPath, "original");
        var testProjectResult = new Mock<IAnalyzerResult>();
        testProjectResult.Setup(x => x.Properties).Returns(new Dictionary<string, string> { ["TargetDir"] = testOutput });
        var testProject = new Mock<ITestProject>();
        testProject.Setup(x => x.AnalyzerResult).Returns(testProjectResult.Object);
        var testProjects = new TestProjectsInfo(new FileSystem(), NullLogger.Instance) { TestProjects = [testProject.Object] };
        var process = new FsharpCompilingProcess(project.AnalyzerResult, new FileSystem(), NullLogger.Instance);
        var result = process.Compile();

        // same order as MutationTestProcess: backup first, then inject
        testProjects.BackupOriginalAssembly(project.AnalyzerResult);
        process.Inject(testProjects, result.Assembly);

        AssemblyName.GetAssemblyName(injectionPath).Name.ShouldBe(project.AnalyzerResult.GetAssemblyName());
        File.ReadAllText(injectionPath + ".stryker-unchanged").ShouldBe("original");
        testProjects.RestoreOriginalAssembly(project.AnalyzerResult);
        File.ReadAllText(injectionPath).ShouldBe("original");
    }

    private static string WriteToTemp(byte[] bytes)
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".dll");
        File.WriteAllBytes(path, bytes);
        return path;
    }
}
