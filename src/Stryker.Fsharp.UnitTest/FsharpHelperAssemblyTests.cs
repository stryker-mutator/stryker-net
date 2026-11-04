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
}
