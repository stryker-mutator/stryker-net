using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions;
using Stryker.CLI.MutationServer;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;

namespace Stryker.CLI.UnitTest.MutationServer;

[TestClass]
public class MutationServerReporterTests
{
    [TestMethod]
    public async Task ReporterShouldMapDiscoveryAndProgressResults()
    {
        var basePath = Path.GetFullPath("project");
        var filePath = Path.Combine(basePath, "src", "Calculator.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "class Calculator { bool Calculate() => true; }",
            path: filePath);
        var originalNode = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().Single();
        var mutant = new Mutant
        {
            Id = 1,
            ResultStatus = MutantStatus.Pending,
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                DisplayName = "boolean",
                Description = "Boolean literal"
            }
        };
        var file = new CsharpFileLeaf
        {
            FullPath = filePath,
            RelativePath = Path.Combine("src", "Calculator.cs"),
            SourceCode = syntaxTree.ToString(),
            SyntaxTree = syntaxTree,
            Mutants = [mutant]
        };
        var project = new FolderComposite { FullPath = basePath };
        project.Add(file);
        MutationTestResult progress = null;
        var progressCount = 0;
        var reporter = new MutationServerReporter(
            basePath,
            result =>
            {
                progress = result;
                progressCount++;
                return Task.CompletedTask;
            });

        reporter.OnMutantsCreated(project, null);

        var discovered = reporter.DiscoverResult.Files["src/Calculator.cs"].Mutants.Single();
        discovered.Id.ShouldBe("9D98A8548035BBC2BA1BBC55C5D0B287EB3BE1F122472EC32D9338B943763E0C");
        discovered.Location.Start.Line.ShouldBe(1);
        discovered.Location.Start.Column.ShouldBeGreaterThan(1);

        mutant.ResultStatus = MutantStatus.Killed;
        reporter.OnMutantTested(mutant);
        reporter.OnAllMutantsTested(project, null);
        await reporter.CompleteAsync();

        var tested = progress.Files["src/Calculator.cs"].Mutants.Single();
        tested.Status.ShouldBe(nameof(MutantStatus.Killed));
        tested.MutatorName.ShouldBe("boolean");
        progressCount.ShouldBe(1);
    }

    [TestMethod]
    public async Task ReporterShouldImmediatelyReportSelectedTerminalMutants()
    {
        var basePath = Path.GetFullPath("project");
        var filePath = Path.Combine(basePath, "src", "Calculator.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "class Calculator { bool Calculate() => true; }",
            path: filePath);
        var originalNode = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().Single();
        var mutant = new Mutant
        {
            Id = 1,
            ResultStatus = MutantStatus.NoCoverage,
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                DisplayName = "boolean"
            }
        };
        var file = new CsharpFileLeaf
        {
            FullPath = filePath,
            RelativePath = Path.Combine("src", "Calculator.cs"),
            SyntaxTree = syntaxTree,
            Mutants = [mutant]
        };
        var project = new FolderComposite { FullPath = basePath };
        project.Add(file);
        MutationTestResult progress = null;
        var reporter = new MutationServerReporter(
            basePath,
            result =>
            {
                progress = result;
                return Task.CompletedTask;
            },
            (_, selectedMutant) => selectedMutant.Id == mutant.Id);

        reporter.OnMutantsCreated(project, null);
        await reporter.CompleteAsync();

        progress.Files["src/Calculator.cs"].Mutants.Single().Status
            .ShouldBe(nameof(MutantStatus.NoCoverage));
    }

    [TestMethod]
    public void ReporterShouldPreservePathsOutsideTheWorkingDirectory()
    {
        var basePath = Path.GetFullPath(Path.Combine("project", "tests"));
        var filePath = Path.GetFullPath(Path.Combine("project", "src", "Calculator.cs"));
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "class Calculator { bool Calculate() => true; }",
            path: filePath);
        var originalNode = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().Single();
        var mutant = new Mutant
        {
            Id = 1,
            ResultStatus = MutantStatus.Pending,
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                DisplayName = "boolean"
            }
        };
        var file = new CsharpFileLeaf
        {
            FullPath = filePath,
            RelativePath = "Calculator.cs",
            SyntaxTree = syntaxTree,
            Mutants = [mutant]
        };
        var project = new FolderComposite { FullPath = basePath };
        project.Add(file);
        var reporter = new MutationServerReporter(basePath);

        reporter.OnMutantsCreated(project, null);

        reporter.DiscoverResult.Files.Keys.Single().ShouldBe("../src/Calculator.cs");
    }

    [TestMethod]
    public void ReporterShouldIncludeSelectedFilesWithoutMutants()
    {
        var basePath = Path.GetFullPath("project");
        var file = new CsharpFileLeaf
        {
            FullPath = Path.Combine(basePath, "src", "Calculator.cs"),
            RelativePath = Path.Combine("src", "Calculator.cs"),
            Mutants = []
        };
        var project = new FolderComposite { FullPath = basePath };
        project.Add(file);
        var reporter = new MutationServerReporter(
            basePath,
            fileSelection: _ => true);

        reporter.OnMutantsCreated(project, null);

        reporter.DiscoverResult.Files["src/Calculator.cs"].Mutants.ShouldBeEmpty();
    }

    [TestMethod]
    public async Task ReporterShouldMapPendingMutantsToRuntimeErrors()
    {
        var (basePath, project, mutant) = CreateProjectWithMutant(MutantStatus.Pending);
        MutationTestResult progress = null;
        var reporter = new MutationServerReporter(
            basePath,
            reportProgress: result =>
            {
                progress = result;
                return Task.CompletedTask;
            });
        reporter.OnMutantsCreated(project, null);

        reporter.OnAllMutantsTested(project, null);
        await reporter.CompleteAsync();

        var result = progress.Files["src/Calculator.cs"].Mutants.Single();
        result.Status.ShouldBe(nameof(MutantStatus.RuntimeError));
        result.StatusReason.ShouldBe("Stryker did not complete this mutant test.");
    }

    [TestMethod]
    public async Task ReporterShouldCaptureProgressFailuresWithoutThrowingFromCompletion()
    {
        var (basePath, project, _) = CreateProjectWithMutant(MutantStatus.NoCoverage);
        var expectedException = new InvalidOperationException("Connection closed");
        var reporter = new MutationServerReporter(
            basePath,
            reportProgress: _ => Task.FromException(expectedException));

        reporter.OnMutantsCreated(project, null);
        await reporter.CompleteAsync();

        reporter.ProgressException.ShouldBeSameAs(expectedException);
    }

    [TestMethod]
    public void ProtocolIdShouldIgnoreInternalIdAndIncludeMutationFingerprint()
    {
        var (basePath, project, mutant) = CreateProjectWithMutant(MutantStatus.Pending);
        var file = project.GetAllFiles().Single();
        var originalId = MutationServerMutantIdentity.GetId(basePath, file, mutant);

        mutant.Id = 999;
        MutationServerMutantIdentity.GetId(basePath, file, mutant).ShouldBe(originalId);

        mutant.Mutation.Description = "Different mutation";
        MutationServerMutantIdentity.GetId(basePath, file, mutant).ShouldNotBe(originalId);
    }

    [TestMethod]
    public async Task ReporterShouldBoundAndDrainProgressInOrder()
    {
        const int MutantCount = 300;
        var basePath = Path.GetFullPath("project");
        var filePath = Path.Combine(basePath, "src", "Calculator.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "class Calculator { bool Calculate() => true; }",
            path: filePath);
        var originalNode = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().Single();
        var mutants = Enumerable.Range(0, MutantCount)
            .Select(index => new Mutant
            {
                Id = index,
                ResultStatus = MutantStatus.NoCoverage,
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                    ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                    DisplayName = "boolean",
                    Description = $"Mutation {index}"
                }
            })
            .ToArray();
        var file = new CsharpFileLeaf
        {
            FullPath = filePath,
            RelativePath = Path.Combine("src", "Calculator.cs"),
            SyntaxTree = syntaxTree,
            Mutants = mutants
        };
        var project = new FolderComposite { FullPath = basePath };
        project.Add(file);
        var firstNotificationStarted = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseNotifications = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var reportedIds = new List<string>();
        var reporter = new MutationServerReporter(
            basePath,
            reportProgress: async result =>
            {
                if (reportedIds.Count == 0)
                {
                    firstNotificationStarted.TrySetResult();
                    await releaseNotifications.Task;
                }

                reportedIds.Add(result.Files["src/Calculator.cs"].Mutants.Single().Id);
            });

        var producer = Task.Run(() => reporter.OnMutantsCreated(project, null));
        await firstNotificationStarted.Task.WaitAsync(TimeSpan.FromSeconds(1));
        SpinWait.SpinUntil(
                () => reporter.QueuedProgressCount == MutationServerReporter.ProgressBufferCapacity,
                TimeSpan.FromSeconds(1))
            .ShouldBeTrue();
        producer.IsCompleted.ShouldBeFalse();

        releaseNotifications.SetResult();
        await producer;
        await reporter.CompleteAsync();

        var expectedIds = mutants
            .Select(mutant => MutationServerMutantIdentity.GetId(basePath, file, mutant))
            .ToArray();
        reportedIds.ShouldBe(expectedIds);
    }

    [TestMethod]
    public async Task CancellationShouldUnblockASaturatedProgressBuffer()
    {
        const int MutantCount = 300;
        var basePath = Path.GetFullPath("project");
        var filePath = Path.Combine(basePath, "src", "Calculator.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "class Calculator { bool Calculate() => true; }",
            path: filePath);
        var originalNode = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().Single();
        var mutants = Enumerable.Range(0, MutantCount)
            .Select(index => new Mutant
            {
                Id = index,
                ResultStatus = MutantStatus.NoCoverage,
                Mutation = new Mutation
                {
                    OriginalNode = originalNode,
                    ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                    DisplayName = "boolean",
                    Description = $"Mutation {index}"
                }
            })
            .ToArray();
        var file = new CsharpFileLeaf
        {
            FullPath = filePath,
            RelativePath = Path.Combine("src", "Calculator.cs"),
            SyntaxTree = syntaxTree,
            Mutants = mutants
        };
        var project = new FolderComposite { FullPath = basePath };
        project.Add(file);
        using var cancellationTokenSource = new CancellationTokenSource();
        var sendStarted = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseSend = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var reporter = new MutationServerReporter(
            basePath,
            reportProgress: async _ =>
            {
                sendStarted.SetResult();
                await releaseSend.Task;
            },
            cancellationToken: cancellationTokenSource.Token);

        var producer = Task.Run(() => reporter.OnMutantsCreated(project, null));
        await sendStarted.Task.WaitAsync(TimeSpan.FromSeconds(1));
        SpinWait.SpinUntil(
                () => reporter.QueuedProgressCount == MutationServerReporter.ProgressBufferCapacity,
                TimeSpan.FromSeconds(1))
            .ShouldBeTrue();
        cancellationTokenSource.Cancel();

        await producer.WaitAsync(TimeSpan.FromSeconds(1));
        await reporter.CompleteAsync().WaitAsync(TimeSpan.FromSeconds(1));
        releaseSend.SetResult();
    }

    private static (string BasePath, FolderComposite Project, Mutant Mutant) CreateProjectWithMutant(
        MutantStatus status)
    {
        var basePath = Path.GetFullPath("project");
        var filePath = Path.Combine(basePath, "src", "Calculator.cs");
        var syntaxTree = CSharpSyntaxTree.ParseText(
            "class Calculator { bool Calculate() => true; }",
            path: filePath);
        var originalNode = syntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>().Single();
        var mutant = new Mutant
        {
            Id = 1,
            ResultStatus = status,
            Mutation = new Mutation
            {
                OriginalNode = originalNode,
                ReplacementNode = SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
                DisplayName = "boolean"
            }
        };
        var file = new CsharpFileLeaf
        {
            FullPath = filePath,
            RelativePath = Path.Combine("src", "Calculator.cs"),
            SyntaxTree = syntaxTree,
            Mutants = [mutant]
        };
        var project = new FolderComposite { FullPath = basePath };
        project.Add(file);
        return (basePath, project, mutant);
    }
}
