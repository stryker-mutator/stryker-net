using System.Reflection;
using Shouldly;
using Stryker.Core.InjectedHelpers;
using Stryker.Fsharp.UnitTest.Fixtures;

namespace Stryker.Fsharp.UnitTest;

/// <summary>Needs a real dotnet SDK (reference assemblies come from a Buildalyzer run).</summary>
[TestClass]
public class FsharpHelperAssemblyTests
{
    [TestMethod]
    public void HelperAssembly_Compiles_ExposingMutantControlInTheRandomisedNamespace()
    {
        using var project = new TempFsharpProject();
        var injector = new CodeInjection();

        var bytes = FsharpHelperAssembly.Compile(injector, project.AnalyzerResult);

        var path = Path.Combine(project.Directory, FsharpHelperAssembly.FileName(injector));
        File.WriteAllBytes(path, bytes);
        var type = Assembly.LoadFile(path).GetType($"{injector.HelperNamespace}.MutantControl");
        type.ShouldNotBeNull();
        type.GetMethod("IsActive").ShouldNotBeNull();
        Path.GetFileName(path).ShouldBe($"{injector.HelperNamespace}.StrykerHelpers.dll");
    }

    [TestMethod]
    public void HelperAssembly_RegisteredInDepsFile_AsReferenceLibrary_Idempotently()
    {
        var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
        fileSystem.AddFile("/out/Tests.deps.json", new System.IO.Abstractions.TestingHelpers.MockFileData(
            """{"runtimeTarget":{"name":".NETCoreApp,Version=v10.0"},"targets":{".NETCoreApp,Version=v10.0":{"Tests/1.0.0":{"runtime":{"Tests.dll":{}}}}},"libraries":{"Tests/1.0.0":{"type":"project","serviceable":false,"sha512":""}}}"""));

        FsharpHelperAssembly.RegisterInDependencyManifests(fileSystem, "/out", "StrykerAbc.StrykerHelpers.dll");
        FsharpHelperAssembly.RegisterInDependencyManifests(fileSystem, "/out", "StrykerAbc.StrykerHelpers.dll");

        var json = System.Text.Json.Nodes.JsonNode.Parse(fileSystem.File.ReadAllText("/out/Tests.deps.json"))!;
        json["targets"]![".NETCoreApp,Version=v10.0"]!["StrykerAbc.StrykerHelpers/1.0.0"]!["runtime"]!["StrykerAbc.StrykerHelpers.dll"].ShouldNotBeNull();
        json["libraries"]!["StrykerAbc.StrykerHelpers/1.0.0"]!["type"]!.GetValue<string>().ShouldBe("reference");
        json["libraries"]!.AsObject().Count.ShouldBe(2);
        json["targets"]![".NETCoreApp,Version=v10.0"]!["Tests/1.0.0"].ShouldNotBeNull("existing entries are kept");
    }

    private const string OriginalManifest = """{"runtimeTarget":{"name":".NETCoreApp,Version=v10.0"},"targets":{".NETCoreApp,Version=v10.0":{"Tests/1.0.0":{"runtime":{"Tests.dll":{}}}}},"libraries":{"Tests/1.0.0":{"type":"project","serviceable":false,"sha512":""}}}""";

    [TestMethod]
    public void RestoreDependencyManifests_PutsManifestsBackAndDeletesHelper()
    {
        var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
        fileSystem.AddFile("/out/Tests.deps.json", new System.IO.Abstractions.TestingHelpers.MockFileData(OriginalManifest));
        fileSystem.AddFile("/out/StrykerAbc.StrykerHelpers.dll", new System.IO.Abstractions.TestingHelpers.MockFileData("helper"));
        FsharpHelperAssembly.RegisterInDependencyManifests(fileSystem, "/out", "StrykerAbc.StrykerHelpers.dll");
        FsharpHelperAssembly.RegisterInDependencyManifests(fileSystem, "/out", "StrykerAbc.StrykerHelpers.dll");
        fileSystem.File.ReadAllText("/out/Tests.deps.json").ShouldNotBe(OriginalManifest);

        FsharpHelperAssembly.RestoreFromDirectory(fileSystem, "/out", "StrykerAbc.StrykerHelpers.dll");

        fileSystem.File.ReadAllText("/out/Tests.deps.json").ShouldBe(OriginalManifest);
        fileSystem.File.Exists("/out/Tests.deps.json.stryker-unchanged").ShouldBeFalse();
        fileSystem.File.Exists("/out/StrykerAbc.StrykerHelpers.dll").ShouldBeFalse();
    }

    [TestMethod]
    public void RegisterInDependencyManifests_MalformedManifest_IsSkippedWithoutThrowing()
    {
        var fileSystem = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
        fileSystem.AddFile("/out/Bad.deps.json", new System.IO.Abstractions.TestingHelpers.MockFileData("{ not json"));
        fileSystem.AddFile("/out/Tests.deps.json", new System.IO.Abstractions.TestingHelpers.MockFileData(OriginalManifest));

        FsharpHelperAssembly.RegisterInDependencyManifests(fileSystem, "/out", "StrykerAbc.StrykerHelpers.dll");

        fileSystem.File.ReadAllText("/out/Bad.deps.json").ShouldBe("{ not json");
        fileSystem.File.ReadAllText("/out/Tests.deps.json").ShouldContain("StrykerAbc.StrykerHelpers/1.0.0");
    }

    [TestMethod]
    public void MutationProcess_Restore_RemovesTheInjectedHelperFromTestOutputs()
    {
        using var project = new MutatedFsharpProject("module Lib\nlet f a b = a + b\n");
        var helper = Path.Combine(project.Output, FsharpHelperAssembly.FileName(project.Info.CodeInjector));
        File.Exists(helper).ShouldBeTrue();

        new FsharpMutationProcess(new System.IO.Abstractions.FileSystem(), Microsoft.Extensions.Logging.Abstractions.NullLogger<FsharpMutationProcess>.Instance)
            .Restore(new Stryker.Core.MutationTest.MutationTestInput { SourceProjectInfo = project.Info });

        File.Exists(helper).ShouldBeFalse();
    }
}
