using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Reporting;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Validation;

public class ValidateStrykerResults
{
    private readonly ReadOnlyCollection<SyntaxKind> _blacklistedSyntaxKindsForMutating =
        new([
                // Usings
                SyntaxKind.UsingDirective,
                SyntaxKind.UsingKeyword,
                SyntaxKind.UsingStatement,
                // Comments
                SyntaxKind.DocumentationCommentExteriorTrivia,
                SyntaxKind.EndOfDocumentationCommentToken,
                SyntaxKind.MultiLineCommentTrivia,
                SyntaxKind.MultiLineDocumentationCommentTrivia,
                SyntaxKind.SingleLineCommentTrivia,
                SyntaxKind.SingleLineDocumentationCommentTrivia,
                SyntaxKind.XmlComment,
                SyntaxKind.XmlCommentEndToken,
                SyntaxKind.XmlCommentStartToken,
            ]
    );
    private readonly ReadOnlyCollection<SyntaxKind> _parentSyntaxKindsForMutating =
        new([
                SyntaxKind.MethodDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.FieldDeclaration,
                SyntaxKind.OperatorDeclaration,
                SyntaxKind.IndexerDeclaration,
                SyntaxKind.GlobalStatement,
            ]
    );
    private const string MutationReportJson = "mutation-report.json";

    [Fact]
    [Trait("Category", "SingleTestProject")]
    [Trait("Runtime", "netframework")]
    public async Task CSharp_NetFramework_SingleTestProject()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetFramework/FullFrameworkApp.Test/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 29, ignored: 7, survived: 3, killed: 7, timeout: 0, nocoverage: 11);
    }

    [Fact]
    [Trait("Category", "SingleTestProject")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_SingleTestProject()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/NetCoreTestProject.XUnit/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 271, survived: 4, killed: 9, timeout: 2, nocoverage: 340);
        CheckReportTestCounts(report, total: 11);
    }

    [Fact]
    [Trait("Category", "MultipleTestProjects")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_WithTwoTestProjects()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/TargetProject/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 117, survived: 5, killed: 11, timeout: 2, nocoverage: 491);
        CheckReportTestCounts(report, total: 21);
    }

    [Fact]
    [Trait("Category", "MSTestMTP")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_MSTestMTP()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/MicrosoftTestPlatform/UnitTests.MSTest/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 667, ignored: 272, survived: 1, killed: 1, timeout: 0, nocoverage: 359);
        CheckReportTestCounts(report, total: 2);
    }

    [Fact]
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_XUnitMTP()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/MicrosoftTestPlatform/UnitTests.XUnit/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        // Only the project under test is mutated here, so this run holds a single mutated assembly. The
        // extra test covers ExtraProject, which the solution run mutates as well; see MTPSolution.
        CheckReportMutants(report, total: 660, ignored: 271, survived: 1, killed: 1, timeout: 0, nocoverage: 353);
        CheckReportTestCounts(report, total: 3);
    }

    [Fact]
    [Trait("Category", "NUnitMTP")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_NUnitMTP()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/MicrosoftTestPlatform/UnitTests.NUnit/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 271, survived: 1, killed: 1, timeout: 0, nocoverage: 353);
        CheckReportTestCounts(report, total: 2);
    }

    [Fact]
    [Trait("Category", "TUnit")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_TUnit()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/MicrosoftTestPlatform/UnitTests.TUnit/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 271, survived: 1, killed: 1, timeout: 0, nocoverage: 353);
        CheckReportTestCounts(report, total: 2);
    }

    [Fact]
    [Trait("Category", "MTPSolution")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_MTPSolution()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        // Coverage is the union over all four test projects, so the Timeout.cs mutants covered only
        // by the MSTest project count as covered (1 survived + 2 timeout), like in the MSTestMTP run.
        // Before coverage files were split per test host, the final flush overwrote the shared
        // file, usually losing exactly those three mutants to NoCoverage.
        // Teacher.cs and Lesson.cs are reached from one test that spans both of their assemblies, so
        // losing either one's coverage names the assembly at fault instead of moving a global count by one.
        CheckEveryMutatedProjectIsCovered(report, "KilledMutants.cs", "Teacher.cs", "Lesson.cs");
        CheckReportMutants(report, total: 673, ignored: 275, survived: 2, killed: 3, timeout: 2, nocoverage: 357);
        CheckReportTestCounts(report, total: 11);
    }

    [Fact]
    [Trait("Category", value: "WebApiWithOpenApi")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_WebApiWithOpenApi()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/WebApiWithOpenApi/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 11, ignored: 2, survived: 5, killed: 4, timeout: 0, nocoverage: 0);
        CheckReportTestCounts(report, total: 3);
    }

    [Fact]
    [Trait("Category", "Solution")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_SolutionRun()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 271, survived: 4, killed: 9, timeout: 2, nocoverage: 340);
        CheckReportTestCounts(report, total: 23);
    }

    [Fact]
    [Trait("Category", "Solution")]
    [Trait("Runtime", "netframework")]
    public async Task CSharp_NetFramework_SolutionRun()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetFramework/FullFrameworkApp.Test/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 29, ignored: 7, survived: 3, killed: 7, timeout: 0, nocoverage: 11);
    }

    [Fact]
    [Trait("Category", "Fsharp")]
    [Trait("Runtime", "netcore")]
    public async Task Fsharp_NetCore_SingleTestProject()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/FsharpTestProject/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        // The C# syntax-kind validation of CheckReportMutants cannot parse F# sources, so only the status counts are pinned.
        CheckReportMutantStatuses(report, total: 11, ignored: 0, survived: 2, killed: 5, timeout: 0, nocoverage: 1, compileError: 3);
        report.Files.Keys.ShouldAllBe(file => file.EndsWith(".fs", StringComparison.Ordinal));
        report.Files.Keys.Select(Path.GetFileName).ShouldBe(["A.fs", "B.fs"], ignoreOrder: true);
    }

    private void CheckMutationKindsValidity(IJsonReport report)
    {
        foreach (var file in report.Files)
        {
            var syntaxTreeRootNode = CSharpSyntaxTree.ParseText(file.Value.Source).GetRoot();
            var textLines = SourceText.From(file.Value.Source).Lines;

            foreach (var mutation in file.Value.Mutants)
            {
                var linePositionSpan = new LinePositionSpan(new LinePosition(mutation.Location.Start.Line - 1, mutation.Location.Start.Column), new LinePosition(mutation.Location.End.Line - 1, mutation.Location.End.Column));
                var textSpan = textLines.GetTextSpan(linePositionSpan);
                var node = syntaxTreeRootNode.FindNode(textSpan);
                var nodeKind = node.Kind();
                _blacklistedSyntaxKindsForMutating.ShouldNotContain(nodeKind);

                node
                    .AncestorsAndSelf()
                    .ShouldContain(pn =>
                        _parentSyntaxKindsForMutating.Contains(pn.Kind()),
                        $"Mutation {mutation.MutatorName} on line {mutation.Location.Start.Line} in file {file.Key} does not have one of the known parent syntax kinds as it's parent.{Environment.NewLine}" +
                        $"Instead it has: {Environment.NewLine} {string.Join($",{Environment.NewLine}", node.AncestorsAndSelf().Select(n => n.Kind()))}");
            }
        }
    }

    /// <summary>
    /// Asserts that every mutated project contributed its own coverage to the report.
    /// A global count can stay correct while one assembly loses its coverage and another gains some,
    /// so each source file is checked on its own and the failure message names the assembly at fault.
    /// </summary>
    private void CheckEveryMutatedProjectIsCovered(IJsonReport report, params string[] sourceFiles)
    {
        foreach (var sourceFile in sourceFiles)
        {
            var file = report.Files.SingleOrDefault(f => f.Key.EndsWith(sourceFile, StringComparison.Ordinal));
            file.Key.ShouldNotBeNull($"{sourceFile} is missing from the report entirely");

            // An assembly whose coverage is lost has every one of its mutants reported as uncovered, so
            // one killed mutant is enough to prove its coverage arrived. Asserting no uncovered mutants
            // instead would be wrong: these files also hold code no test exercises.
            file.Value.Mutants.Count(m => m.Status == MutantStatus.Killed.ToString())
                .ShouldBeGreaterThan(0, $"no mutant of {sourceFile} was killed: the coverage of its assembly was lost");
        }
    }

    private void CheckReportMutants(IJsonReport report, int total, int ignored, int survived, int killed, int timeout, int nocoverage, int runtimeError = 0)
    {
        CheckReportMutantStatuses(report, total, ignored, survived, killed, timeout, nocoverage, runtimeError: runtimeError);

        CheckMutationKindsValidity(report);
    }

    private static void CheckReportMutantStatuses(IJsonReport report, int total, int ignored, int survived, int killed, int timeout, int nocoverage, int runtimeError = 0, int? compileError = null)
    {
        int CountOf(MutantStatus status) => report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == status.ToString())).Sum();
        var actualTotal = report.Files.Select(f => f.Value.Mutants.Count()).Sum();

        report.Files.ShouldSatisfyAllConditions(
            () => actualTotal.ShouldBe(total),
            () => CountOf(MutantStatus.Ignored).ShouldBe(ignored),
            () => CountOf(MutantStatus.Survived).ShouldBe(survived),
            () => CountOf(MutantStatus.Killed).ShouldBe(killed),
            () => CountOf(MutantStatus.Timeout).ShouldBe(timeout),
            () => CountOf(MutantStatus.NoCoverage).ShouldBe(nocoverage),
            () => CountOf(MutantStatus.RuntimeError).ShouldBe(runtimeError),
            () => { if (compileError is { } expected) CountOf(MutantStatus.CompileError).ShouldBe(expected); }
        );
    }

    private void CheckReportTestCounts(IJsonReport report, int total)
    {
        var actualTestCount = report.TestFiles.Sum(tf => tf.Value.Tests.Count);

        actualTestCount.ShouldBe(total);
    }
}
