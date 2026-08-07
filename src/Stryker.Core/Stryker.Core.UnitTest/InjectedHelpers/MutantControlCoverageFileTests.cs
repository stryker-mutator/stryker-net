using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.UnitTest.InjectedHelpers;

/// <summary>
/// Covers the coverage file the injected <c>MutantControl</c> writes for the MTP runner.
/// A test host loads one copy of the helper per mutated assembly, and every copy reads the same
/// STRYKER_COVERAGE_FILE environment variable, so the copies must not write to the same file.
/// The helpers are compiled and loaded here (once per assembly name) to reproduce that situation.
/// </summary>
[TestClass]
public class MutantControlCoverageFileTests : TestBase
{
    private const string CoverageFileEnvironmentVariable = "STRYKER_COVERAGE_FILE";

    [TestMethod]
    public void FlushCoverage_ShouldWriteOneFilePerMutatedAssembly_WhenSeveralShareATestHost()
    {
        var coverageFileName = $"stryker-coverage-test-{Guid.NewGuid():N}.txt";
        var previousEnvironmentValue = Environment.GetEnvironmentVariable(CoverageFileEnvironmentVariable);

        try
        {
            // The runner passes the file name, the helper resolves it against the temp directory
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, coverageFileName);

            var firstAssembly = CompileMutantControl("MutatedAssemblyOne");
            var secondAssembly = CompileMutantControl("MutatedAssemblyTwo");

            // In coverage mode IsActive registers the mutant instead of activating it
            RegisterCoverage(firstAssembly, 1);
            RegisterCoverage(secondAssembly, 2);

            FlushCoverage(firstAssembly);
            FlushCoverage(secondAssembly);

            var writtenFiles = FindCoverageFiles(coverageFileName);

            writtenFiles.Count.ShouldBe(2,
                "each mutated assembly must write its own coverage file, otherwise the last flush overwrites the others");

            var contents = writtenFiles.Select(File.ReadAllText).ToList();
            contents.ShouldContain(content => ParseCoveredMutants(content).Contains(1),
                "the coverage of the first mutated assembly must survive");
            contents.ShouldContain(content => ParseCoveredMutants(content).Contains(2),
                "the coverage of the second mutated assembly must survive");
        }
        finally
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, previousEnvironmentValue);
            foreach (var file in FindCoverageFiles(coverageFileName))
            {
                File.Delete(file);
            }
        }
    }

    [TestMethod]
    public void FlushCoverage_ShouldWriteAFileTheRunnerCanFind_WhenASingleAssemblyIsMutated()
    {
        var coverageFileName = $"stryker-coverage-test-{Guid.NewGuid():N}.txt";
        var previousEnvironmentValue = Environment.GetEnvironmentVariable(CoverageFileEnvironmentVariable);

        try
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, coverageFileName);

            var assembly = CompileMutantControl("SingleMutatedAssembly");
            RegisterCoverage(assembly, 7);
            FlushCoverage(assembly);

            var writtenFiles = FindCoverageFiles(coverageFileName);

            writtenFiles.Count.ShouldBe(1);
            // The runner looks for files whose name starts with the name it handed out, minus the
            // extension, and ends with that extension
            var writtenFileName = Path.GetFileName(writtenFiles[0]);
            writtenFileName.ShouldStartWith(Path.GetFileNameWithoutExtension(coverageFileName));
            writtenFileName.ShouldEndWith(Path.GetExtension(coverageFileName));
            ParseCoveredMutants(File.ReadAllText(writtenFiles[0])).ShouldContain(7);
        }
        finally
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, previousEnvironmentValue);
            foreach (var file in FindCoverageFiles(coverageFileName))
            {
                File.Delete(file);
            }
        }
    }

    private static List<string> FindCoverageFiles(string coverageFileName) =>
        Directory.GetFiles(
                Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(coverageFileName) + "*" + Path.GetExtension(coverageFileName))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();

    private static IReadOnlyCollection<int> ParseCoveredMutants(string content) =>
        content.Split(';')[0]
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

    private static void RegisterCoverage(Assembly assembly, int mutantId) =>
        GetMutantControl(assembly).GetMethod("IsActive")!.Invoke(null, new object[] { mutantId });

    private static void FlushCoverage(Assembly assembly) =>
        GetMutantControl(assembly).GetMethod("FlushCoverageToFile")!.Invoke(null, null);

    private static Type GetMutantControl(Assembly assembly) =>
        assembly.GetTypes().Single(type => type.Name == "MutantControl");

    /// <summary>
    /// Compiles the injected helpers into their own assembly, as Stryker does for every mutated
    /// project, and loads it. Each assembly gets its own copy of the helper statics.
    /// </summary>
    private static Assembly CompileMutantControl(string assemblyName)
    {
        var codeInjection = new CodeInjection();
        var syntaxTrees = codeInjection.MutantHelpers
            .Select(helper => CSharpSyntaxTree.ParseText(helper.Value, path: helper.Key))
            .ToList();

        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToList();

        var compilation = CSharpCompilation.Create(assemblyName,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var peStream = new MemoryStream();
        var result = compilation.Emit(peStream);
        result.Success.ShouldBeTrue(
            $"the injected helpers should compile: {string.Join(Environment.NewLine, result.Diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))}");

        return Assembly.Load(peStream.ToArray());
    }
}
