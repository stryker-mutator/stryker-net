using System.IO.Abstractions;
using System.Reflection;
using System.Runtime.Loader;
using Buildalyzer;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Fsharp.ProjectComponents;
using Stryker.Fsharp.UnitTest.Fixtures;
using Stryker.TestRunner.Tests;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Fsharp.UnitTest;

/// <summary>
/// Walking skeleton below the VsTest runner: real orchestration, real splice compile with the helper
/// assembly, real injection into a test output directory, then the mutated function is invoked through
/// reflection the way a test would call it. The runner itself (VsTest) is not driven here.
/// </summary>
[TestClass]
public class FsharpSkeletonEndToEndTests
{
    [TestMethod]
    public void FsharpSkeleton_EndToEnd_ReportsKilledMutant()
    {
        using var project = new TempFsharpProject(("Lib.fs", "module Lib\nlet isEnabled () = true\n"));
        var testOutput = Path.Combine(project.Directory, "testout");
        Directory.CreateDirectory(testOutput);
        var injectionPath = Path.Combine(testOutput, project.AnalyzerResult.GetAssemblyFileName());
        File.WriteAllText(injectionPath, "original");
        var testProjects = TestProjectsInfoWithOutput(testOutput);
        var info = new SourceProjectInfo(project.AnalyzerResult, testProjects);
        info.ProjectContents = new FsharpProjectComponentsBuilder(info, NullLogger.Instance, new FileSystem()).Build();
        var input = new MutationTestInput { SourceProjectInfo = info };
        // same order as MutationTestProcess.Initialize then Mutate
        info.BackupOriginalAssembly(project.AnalyzerResult);

        new FsharpMutationProcess(new FileSystem(), NullLogger<FsharpMutationProcess>.Instance).Mutate(input, Mock.Of<IStrykerOptions>());

        var leaf = info.ProjectContents.GetAllFiles().Cast<FsharpFileLeaf>().Single();
        var mutant = leaf.Mutants.Single();
        // the mutated assembly is injected, with the helper next to it
        File.Exists(Path.Combine(testOutput, $"{info.HelperNamespace}.StrykerHelpers.dll")).ShouldBeTrue();
        var (isEnabled, setActive) = LoadMutatedFunction(testOutput, info.HelperNamespace, project.AnalyzerResult.GetAssemblyFileName());
        setActive(-1);
        isEnabled().ShouldBeTrue("no mutant active: original behaviour");
        setActive(mutant.Id);
        isEnabled().ShouldBeFalse("mutant active: boolean flipped, a test asserting true fails");

        // a failing test while the mutant is active kills it, and the report carries F# metadata
        mutant.AnalyzeTestRun(new TestIdentifierList("IsEnabledTest"), TestIdentifierList.EveryTest(), TestIdentifierList.NoTest(), false, false);
        var jsonFile = new SourceFile(leaf, NullLogger.Instance);
        jsonFile.Language.ShouldBe("fs");
        var jsonMutant = jsonFile.Mutants.Single();
        jsonMutant.Status.ShouldBe("Killed");
        jsonMutant.Replacement.ShouldBe("false");
        // "let isEnabled () = true": literal at line 2, columns 1-based, end exclusive
        jsonMutant.Location.Start.Line.ShouldBe(2);
        jsonMutant.Location.Start.Column.ShouldBe(20);
        jsonMutant.Location.End.Line.ShouldBe(2);
        jsonMutant.Location.End.Column.ShouldBe(24);

        info.RestoreOriginalAssembly(project.AnalyzerResult);
        File.ReadAllText(injectionPath).ShouldBe("original");
    }

    private static TestProjectsInfo TestProjectsInfoWithOutput(string testOutput)
    {
        var testProjectResult = new Mock<IAnalyzerResult>();
        testProjectResult.Setup(x => x.Properties).Returns(new Dictionary<string, string> { ["TargetDir"] = testOutput });
        var testProject = new Mock<ITestProject>();
        testProject.Setup(x => x.AnalyzerResult).Returns(testProjectResult.Object);
        return new TestProjectsInfo(new FileSystem(), NullLogger.Instance) { TestProjects = [testProject.Object] };
    }

    private static (Func<bool> IsEnabled, Action<int> SetActive) LoadMutatedFunction(string directory, string helperNamespace, string assemblyFileName)
    {
        var context = new AssemblyLoadContext(null, isCollectible: true);
        context.Resolving += (ctx, name) =>
        {
            var candidate = Path.Combine(directory, name.Name + ".dll");
            return File.Exists(candidate) ? ctx.LoadFromAssemblyPath(candidate) : null;
        };
        var lib = context.LoadFromAssemblyPath(Path.Combine(directory, assemblyFileName));
        var helper = context.LoadFromAssemblyPath(Path.Combine(directory, $"{helperNamespace}.StrykerHelpers.dll"));
        var activeMutant = helper.GetType($"{helperNamespace}.MutantControl")!.GetField("ActiveMutant", BindingFlags.Public | BindingFlags.Static)!;
        var method = lib.GetType("Lib")!.GetMethod("isEnabled")!;
        return (() => (bool)method.Invoke(null, null)!, id => activeMutant.SetValue(null, id));
    }
}
