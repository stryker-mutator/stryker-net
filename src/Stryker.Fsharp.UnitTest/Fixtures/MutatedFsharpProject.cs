using System.IO.Abstractions;
using System.Reflection;
using System.Runtime.Loader;
using Buildalyzer;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.MutationTest;
using Stryker.Core.ProjectComponents.SourceProjects;
using Stryker.Core.ProjectComponents.TestProjects;
using Stryker.Fsharp.ProjectComponents;
using Stryker.Utilities.Buildalyzer;

namespace Stryker.Fsharp.UnitTest.Fixtures;

/// <summary>An F# project run through the real mutation process and injected in a test output directory, loadable like a test host would.</summary>
internal sealed class MutatedFsharpProject : IDisposable
{
    private readonly TempFsharpProject _project;
    private readonly string _assemblyFileName;

    public MutatedFsharpProject(string source)
    {
        _project = new TempFsharpProject(("Lib.fs", source));
        _assemblyFileName = _project.AnalyzerResult.GetAssemblyFileName();
        Output = Path.Combine(_project.Directory, "testout");
        Directory.CreateDirectory(Output);
        File.WriteAllText(Path.Combine(Output, _assemblyFileName), "original");

        var testProjectResult = new Mock<IAnalyzerResult>();
        testProjectResult.Setup(x => x.Properties).Returns(new Dictionary<string, string> { ["TargetDir"] = Output });
        var testProject = new Mock<ITestProject>();
        testProject.Setup(x => x.AnalyzerResult).Returns(testProjectResult.Object);
        var testProjects = new TestProjectsInfo(new FileSystem(), NullLogger.Instance) { TestProjects = [testProject.Object] };

        Info = new SourceProjectInfo(_project.AnalyzerResult, testProjects);
        Info.ProjectContents = new FsharpProjectComponentsBuilder(Info, NullLogger.Instance, new FileSystem()).Build();
        Info.BackupOriginalAssembly(_project.AnalyzerResult);
        new FsharpMutationProcess(new FileSystem(), NullLogger<FsharpMutationProcess>.Instance)
            .Mutate(new MutationTestInput { SourceProjectInfo = Info }, Mock.Of<IStrykerOptions>(o => o.MutationLevel == Stryker.Abstractions.MutationLevel.Complete));
        Leaf = Info.ProjectContents.GetAllFiles().Cast<FsharpFileLeaf>().Single();
    }

    public string Output { get; }
    public SourceProjectInfo Info { get; }
    public FsharpFileLeaf Leaf { get; }
    public string Ns => Info.HelperNamespace;

    /// <summary>Loads the mutated assembly and its helper in a fresh load context; helper types are reached by reflection only.</summary>
    public LoadedMutatedAssembly Load()
    {
        var context = new AssemblyLoadContext(null, isCollectible: true);
        context.Resolving += (ctx, name) =>
        {
            var candidate = Path.Combine(Output, name.Name + ".dll");
            return File.Exists(candidate) ? ctx.LoadFromAssemblyPath(candidate) : null;
        };
        var lib = context.LoadFromAssemblyPath(Path.Combine(Output, _assemblyFileName));
        var helper = context.LoadFromAssemblyPath(Path.Combine(Output, $"{Ns}.StrykerHelpers.dll"));
        return new LoadedMutatedAssembly(lib, helper.GetType($"{Ns}.MutantControl")!);
    }

    public void Dispose() => _project.Dispose();
}

internal sealed class LoadedMutatedAssembly(Assembly lib, Type mutantControl)
{
    public void Set(string field, object value) => mutantControl.GetField(field, BindingFlags.Public | BindingFlags.Static)!.SetValue(null, value);

    public object? Call(string module, string function, params object[] args) => lib.GetType(module)!.GetMethod(function)!.Invoke(null, args);

    /// <summary>The runners' contract: GetCoverageData returns [covered, coveredInStatic].</summary>
    public IList<int>[] CoverageData() => (IList<int>[])mutantControl.GetMethod("GetCoverageData")!.Invoke(null, null)!;
}
