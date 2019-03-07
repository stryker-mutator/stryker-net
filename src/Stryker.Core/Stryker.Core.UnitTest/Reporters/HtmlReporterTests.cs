using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Shouldly;
using Stryker.Core.Logging;
using Stryker.Core.Mutants;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using Stryker.Core.Reporters.Html;
using Stryker.Core.Reporters.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class HtmlReporterTests
    {
        public HtmlReporterTests()
        {
            ApplicationLogging.ConfigureLogger(new LogOptions(Serilog.Events.LogEventLevel.Fatal, false, null));
            ApplicationLogging.LoggerFactory.CreateLogger<HtmlReporterTests>();
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldWriteJsonToFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new HtmlReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(CreateProjectWith());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");
            mockFileSystem.FileExists(reportPath).ShouldBeTrue($"Path {reportPath} should exist but it does not.");
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldReplacePlaceholdersInHtmlFile()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new HtmlReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(CreateProjectWith());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            fileContents.ShouldSatisfyAllConditions(
                () => fileContents.ShouldNotContain("##REPORT_JS##"),
                () => fileContents.ShouldNotContain("##REPORT_TITLE##"),
                () => fileContents.ShouldNotContain("##REPORT_JSON##"));
        }

        [Fact]
        public void JsonReporter_OnAllMutantsTestedShouldContainJsonReport()
        {
            var mockFileSystem = new MockFileSystem();
            var options = new StrykerOptions(thresholdBreak: 0, thresholdHigh: 80, thresholdLow: 60);
            var reporter = new HtmlReporter(options, mockFileSystem);

            reporter.OnAllMutantsTested(CreateProjectWith());
            var reportPath = Path.Combine(options.OutputPath, "reports", $"mutation-report.html");

            var fileContents = mockFileSystem.GetFile(reportPath).TextContents;

            JsonReport.Build(options, null).ToJson().ShouldBeSubsetOf(fileContents);
        }

        private IReadOnlyInputComponent CreateProjectWith(bool duplicateMutant = false, int mutationScore = 60)
        {
            var tree = CSharpSyntaxTree.ParseText("void M(){ int i = 0 + 8; }");
            var originalNode = tree.GetRoot().DescendantNodes().OfType<BinaryExpressionSyntax>().First();

            var mutation = new Mutation()
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.BinaryExpression(SyntaxKind.SubtractExpression, originalNode.Left, originalNode.Right),
                DisplayName = "This name should display",
                Type = Mutator.Arithmetic
            };

            var folder = new FolderComposite { Name = "RootFolder", RelativePath = "src" };

            for (var i = 1; i <= 2; i++)
            {
                var addedFolder = new FolderComposite { Name = $"{i}", RelativePath = $"src/{i}" };
                folder.Add(addedFolder);

                for (var y = 0; y <= 4; y++)
                {
                    var m = new Collection<Mutant>();
                    addedFolder.Add(new FileLeaf()
                    {
                        Name = $"SomeFile{i}.cs",
                        RelativePath = $"src/{i}/SomeFile{i}.cs",
                        Mutants = m,
                        SourceCode = "void M(){ int i = 0 + 8; }"
                    });

                    for (var z = 0; z <= 5; z++)
                    {
                        m.Add(new Mutant()
                        {
                            Id = duplicateMutant ? 2 : new Random().Next(1, 5000),
                            ResultStatus = 100 / 6 * z < mutationScore ? MutantStatus.Killed : MutantStatus.Survived,
                            Mutation = mutation
                        });
                    }
                }
            }

            return folder;
        }
    }
}
