using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Reporters.Json;
using Stryker.CLI;
using Xunit;

namespace IntegrationTests
{
    public class ValidateStrykerResults
    {
        private readonly ReadOnlyCollection<SyntaxKind> _blacklistedSyntaxKindsForMutating =
            new(new[]
            {
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
            }
        );
        private readonly ReadOnlyCollection<SyntaxKind> _parentSyntaxKindsForMutating =
            new(new[]
            {
                SyntaxKind.MethodDeclaration,
                SyntaxKind.PropertyDeclaration,
                SyntaxKind.ConstructorDeclaration,
                SyntaxKind.FieldDeclaration,
            }
        );
        private const string MutationReportJson = "mutation-report.json";

        [Fact]
        [Trait("Category", "SingleTestProject")]
        public void NetFullFramework()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var directory = new DirectoryInfo("../../../../TargetProjects/NetFramework/FullFrameworkApp.Test/StrykerOutput");
                directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

                var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                    .OrderByDescending(f => f.LastWriteTime)
                    .First();

                var strykerRunOutput = File.ReadAllText(latestReport.FullName);

                var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

                CheckReportMutants(report, total: 28, ignored: 7, survived: 2, killed: 7, timeout: 0, nocoverage: 11);
            }
        }

        [Fact]
        [Trait("Category", "SingleTestProject")]
        public void NetCore()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/NetCoreTestProject.XUnit/StrykerOutput");
            directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

            var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutants(report, total: 114, ignored: 55, survived: 4, killed: 6, timeout: 2, nocoverage: 45);
            CheckReportTestCounts(report, total: 14);
        }

        [Fact]
        [Trait("Category", "FSharp")]
        public void FSharp()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/Library.FSharp.XUnit/StrykerOutput");
            directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

            var latestReport = directory
                .GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutants(report, total: 0, ignored: 0, survived: 0, killed: 0, timeout: 0, nocoverage: 0);
            CheckReportTestCounts(report, total: 0);
        }

        [Fact]
        [Trait("Category", "MultipleTestProjects")]
        public void NetCoreWithTwoTestProjects()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/Targetproject/StrykerOutput");
            directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

            var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutants(report, total: 114, ignored: 27, survived: 8, killed: 8, timeout: 2, nocoverage: 67);
            CheckReportTestCounts(report, total: 30);
        }

        [Fact]
        [Trait("Category", "Solution")]
        public void SolutionRun()
        {
            var directory = new DirectoryInfo("../../../../TargetProjects/StrykerOutput");
            directory.GetFiles("*.json", SearchOption.AllDirectories).ShouldNotBeEmpty("No reports available to assert");

            var latestReport = directory.GetFiles(MutationReportJson, SearchOption.AllDirectories)
                .OrderByDescending(f => f.LastWriteTime)
                .First();

            var strykerRunOutput = File.ReadAllText(latestReport.FullName);

            var report = JsonConvert.DeserializeObject<JsonReport>(strykerRunOutput);

            CheckReportMutants(report, total: 114, ignored: 55, survived: 4, killed: 6, timeout: 2, nocoverage: 45);
            CheckReportTestCounts(report, total: 30);
        }

        private void CheckMutationKindsValidity(JsonReport report)
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

                    node.AncestorsAndSelf().ShouldContain(pn => _parentSyntaxKindsForMutating.Contains(pn.Kind()));
                }
            }
        }

        private void CheckReportMutants(JsonReport report, int total, int ignored, int survived, int killed, int timeout, int nocoverage)
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

        private void CheckReportTestCounts(JsonReport report, int total)
        {
            var actualTotal = report.TestFiles.Sum(tf => tf.Value.Tests.Count);

            actualTotal.ShouldBe(total);
        }
    }
}
