using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.UnitTest.InjectedHelpers;

/// <summary>
/// The static-context markers (<c>MutantContext</c> using blocks) that coverage instrumentation injects
/// into static constructors and static field initializers must be behavior-neutral whenever coverage is
/// not being captured (the case for a VsTest <c>perTest</c> run outside its capture phase), and must tag
/// a covered mutant as static only while the code actually runs inside a static context.
/// Each test compiles a fresh copy of the injected helpers, so the static state under test is private
/// to the test (see <see cref="MutantControlMtpMutantFileTests"/> for the same pattern).
/// </summary>
[TestClass]
public class MutantControlStaticTaggingTests : TestBase
{
    private Assembly _helper = null!;
    private Type _mutantControl = null!;

    [TestCleanup]
    public void Cleanup()
    {
        if (_mutantControl is not null)
        {
            _mutantControl.GetField("CaptureCoverage")!.SetValue(null, false);
        }
    }

    [TestMethod]
    public void IsActive_RecordsNothing_WhenCoverageIsNotCaptured_EvenInAStaticContext()
    {
        LoadHelper();

        using (EnterStaticContext())
        {
            InvokeIsActive(9).ShouldBeFalse("no mutant is active when no coverage capture runs");
        }

        GetCoveredMutants().ShouldBeEmpty("the marker must not record anything while CaptureCoverage is false");
        GetCoveredStaticMutants().ShouldBeEmpty("the marker must not record anything while CaptureCoverage is false");
    }

    [TestMethod]
    public void IsActive_TagsACoveredMutantAsStatic_OnlyInsideAStaticContext()
    {
        LoadHelper();
        _mutantControl.GetField("CaptureCoverage")!.SetValue(null, true);

        using (EnterStaticContext())
        {
            InvokeIsActive(9).ShouldBeFalse("coverage capture never activates the mutant");
        }

        InvokeIsActive(10);

        GetCoveredMutants().ShouldContain(9);
        GetCoveredStaticMutants().ShouldContain(9);
        GetCoveredMutants().ShouldContain(10);
        GetCoveredStaticMutants().ShouldNotContain(10, "mutant 10 ran outside any static context");
    }

    private void LoadHelper()
    {
        var codeInjection = new Stryker.Core.InjectedHelpers.CodeInjection();
        var syntaxTrees = codeInjection.MutantHelpers
            .Select(helper => Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(helper.Value, path: helper.Key))
            .ToList();
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(path => (Microsoft.CodeAnalysis.MetadataReference)Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(path))
            .ToList();
        var compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
            $"StaticTagging-{Guid.NewGuid():N}",
            syntaxTrees,
            references,
            new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary));
        using var peStream = new MemoryStream();
        var result = compilation.Emit(peStream);
        result.Success.ShouldBeTrue(
            $"the injected helpers should compile: {string.Join(Environment.NewLine, result.Diagnostics.Where(diagnostic => diagnostic.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error))}");

        _helper = Assembly.Load(peStream.ToArray());
        _mutantControl = _helper.GetTypes().Single(type => type.Name == "MutantControl");
    }

    private IDisposable EnterStaticContext()
    {
        var mutantContext = _helper.GetTypes().Single(type => type.Name == "MutantContext");
        return (IDisposable)Activator.CreateInstance(mutantContext, nonPublic: true)!;
    }

    private bool InvokeIsActive(int mutantId) =>
        (bool)_mutantControl.GetMethod("IsActive")!.Invoke(null, [mutantId])!;

    private List<int> GetCoveredMutants() =>
        (List<int>)_mutantControl.GetField("_coveredMutants", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;

    private List<int> GetCoveredStaticMutants() =>
        (List<int>)_mutantControl.GetField("_coveredStaticMutants", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
}
