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

        CheckReportMutants(report, total: 660, ignored: 269, survived: 4, killed: 9, timeout: 2, nocoverage: 338);
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

        CheckReportMutants(report, total: 660, ignored: 115, survived: 5, killed: 11, timeout: 2, nocoverage: 489);
        CheckReportTestCounts(report, total: 21);
    }

    [Fact]
    [Trait("Category", "MSTestMTP")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_MSTestMTP()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/NetCoreTestProject.MSTest.MTP/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 269, survived: 1, killed: 1, timeout: 0, nocoverage: 351);
    }

    [Fact]
    [Trait("Category", "XUnitMTP")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_XUnitMTP()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/NetCoreTestProject.XUnit.MTP/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 269, survived: 1, killed: 1, timeout: 0, nocoverage: 351);
    }

    [Fact]
    [Trait("Category", "NUnitMTP")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_NUnitMTP()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/NetCoreTestProject.NUnit.MTP/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 269, survived: 1, killed: 1, timeout: 0, nocoverage: 351);
    }

    [Fact]
    [Trait("Category", "TUnit")]
    [Trait("Runtime", "netcore")]
    public async Task CSharp_NetCore_TUnit()
    {
        var directory = new DirectoryInfo("../../../../../TargetProjects/NetCore/NetCoreTestProject.TUnit/StrykerOutput");
        directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

        var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
            .OrderByDescending(f => f.LastWriteTime)
            .First();

        using var strykerRunOutput = File.OpenRead(latestReport.FullName);

        var report = await strykerRunOutput.DeserializeJsonReportAsync();

        CheckReportMutants(report, total: 660, ignored: 269, survived: 1, killed: 1, timeout: 0, nocoverage: 351);
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

        CheckReportMutants(report, total: 660, ignored: 269, survived: 4, killed: 9, timeout: 2, nocoverage: 338);
        CheckReportTestCounts(report, total: 25);
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

    private void CheckReportMutants(IJsonReport report, int total, int ignored, int survived, int killed, int timeout, int nocoverage)
    {
        var actualTotal = report.Files.Select(f => f.Value.Mutants.Count()).Sum();
        var actualIgnored = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Ignored.ToString())).Sum();
        var actualSurvived = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Survived.ToString())).Sum();
        var actualKilled = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Killed.ToString())).Sum();
        var actualTimeout = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.Timeout.ToString())).Sum();
        var actualNoCoverage = report.Files.Select(f => f.Value.Mutants.Count(m => m.Status == MutantStatus.NoCoverage.ToString())).Sum();

        report.Files.ShouldSatisfyAllConditions(
            () => actualTotal.ShouldBe(total),
            () => actualIgnored.ShouldBe(ignored),
            () => actualSurvived.ShouldBe(survived),
            () => actualKilled.ShouldBe(killed),
            () => actualTimeout.ShouldBe(timeout),
            () => actualNoCoverage.ShouldBe(nocoverage)
        );

        CheckMutationKindsValidity(report);
    }

    private void CheckReportTestCounts(IJsonReport report, int total)
    {
        var actualTotal = report.TestFiles.Sum(tf => tf.Value.Tests.Count);

        actualTotal.ShouldBe(total);
    }
}
