using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Spectre.Console;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.CLI.CommandLineConfig;
using Stryker.CLI.Logging;
using Stryker.CLI.MutationServer;
using Stryker.Configuration.Options;
using Stryker.Core;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using Stryker.Core.ProjectComponents.Csharp;

namespace Stryker.CLI.UnitTest.MutationServer;

[TestClass]
public class MutationServerServiceTests
{
    [TestMethod]
    public async Task DiscoverShouldInitializeOutputAndPreserveStartupConfig()
    {
        var startupArguments = new[] { "--config-file", "startup.json", "--concurrency", "1" };
        var (service, runner, configBuilder, loggingInitializer) = CreateService();
        service.SetServerArguments(startupArguments);

        await service.DiscoverAsync(new DiscoverParams(), CancellationToken.None);

        configBuilder.Verify(builder => builder.Build(
            It.IsAny<IStrykerInputs>(),
            It.Is<string[]>(arguments => arguments.SequenceEqual(startupArguments)),
            It.IsAny<CommandLineApplication>(),
            It.IsAny<CommandLineConfigReader>()));
        loggingInitializer.Verify(initializer => initializer.InitializeOutputPath(
            It.IsAny<IStrykerInputs>(),
            It.IsAny<IFileSystem>(),
            It.IsAny<Action<string>>()), Times.Once);
        runner.VerifyAll();
    }

    [TestMethod]
    public async Task ConfigureShouldOverrideStartupConfig()
    {
        var (service, runner, configBuilder, _) = CreateService();
        service.SetServerArguments(["--config-file", "startup.json", "--concurrency", "1"]);
        service.Configure(new ConfigureParams { ConfigFilePath = "configured.json" });

        await service.DiscoverAsync(new DiscoverParams(), CancellationToken.None);

        configBuilder.Verify(builder => builder.Build(
            It.IsAny<IStrykerInputs>(),
            It.Is<string[]>(arguments => arguments.SequenceEqual(
                new[] { "--concurrency", "1", "--config-file", "configured.json" })),
            It.IsAny<CommandLineApplication>(),
            It.IsAny<CommandLineConfigReader>()));
        runner.VerifyAll();
    }

    [TestMethod]
    public async Task StartingANewConnectionShouldResetConfiguredFile()
    {
        var startupArguments = new[] { "--config-file", "startup.json" };
        var (service, runner, configBuilder, _) = CreateService();
        service.SetServerArguments(startupArguments);
        service.Configure(new ConfigureParams { ConfigFilePath = "configured.json" });
        await service.DiscoverAsync(new DiscoverParams(), CancellationToken.None);

        service.SetServerArguments(startupArguments);
        await service.DiscoverAsync(new DiscoverParams(), CancellationToken.None);

        var buildArguments = configBuilder.Invocations
            .Where(invocation => invocation.Method.Name == nameof(IConfigBuilder.Build))
            .Select(invocation => (string[])invocation.Arguments[1])
            .ToArray();
        buildArguments[0].ShouldBe(new[] { "--config-file", "configured.json" });
        buildArguments[1].ShouldBe(startupArguments);
        runner.VerifyAll();
    }

    [TestMethod]
    [DataRow("--config-file:startup.json", null)]
    [DataRow("--config-file=startup.json", null)]
    [DataRow("-f:startup.json", null)]
    [DataRow("-f=startup.json", null)]
    [DataRow("--config-file", "startup.json")]
    [DataRow("-f", "startup.json")]
    public async Task ConfigureShouldReplaceStartupConfigForms(
        string configOption,
        string configValue)
    {
        var startupArguments = configValue is null
            ? new[] { configOption, "--concurrency", "1" }
            : new[] { configOption, configValue, "--concurrency", "1" };
        var (service, runner, configBuilder, _) = CreateService();
        service.SetServerArguments(startupArguments);
        service.Configure(new ConfigureParams { ConfigFilePath = "configured.json" });

        await service.DiscoverAsync(new DiscoverParams(), CancellationToken.None);

        configBuilder.Verify(builder => builder.Build(
            It.IsAny<IStrykerInputs>(),
            It.Is<string[]>(arguments => arguments.SequenceEqual(
                new[] { "--concurrency", "1", "--config-file", "configured.json" })),
            It.IsAny<CommandLineApplication>(),
            It.IsAny<CommandLineConfigReader>()));
        runner.VerifyAll();
    }

    [TestMethod]
    public async Task MutationTestShouldPassCancellationToTheRunnerAndReleaseTheLock()
    {
        var basePath = Path.GetFullPath("project");
        var outputPath = Path.Combine(basePath, "StrykerOutput", "test");
        var runner = new Mock<IStrykerRunner>(MockBehavior.Strict);
        runner.Setup(current => current.RunMutationTestAsync(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<Stryker.Abstractions.Reporting.IReporter>(),
                It.IsAny<Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>>(),
                It.IsAny<CancellationToken>()))
            .Returns<IStrykerInputs, Stryker.Abstractions.Reporting.IReporter,
                Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>, CancellationToken>(
                async (_, _, _, cancellationToken) =>
                {
                    await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                    return null;
                });
        var configBuilder = new Mock<IConfigBuilder>();
        configBuilder
            .Setup(builder => builder.Build(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<string[]>(),
                It.IsAny<CommandLineApplication>(),
                It.IsAny<CommandLineConfigReader>()))
            .Callback<IStrykerInputs, string[], CommandLineApplication, CommandLineConfigReader>(
                (inputs, _, _, _) => inputs.BasePathInput.SuppliedInput = basePath);
        var loggingInitializer = new Mock<ILoggingInitializer>();
        loggingInitializer
            .Setup(initializer => initializer.InitializeOutputPath(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<IFileSystem>(),
                It.IsAny<Action<string>>()))
            .Callback<IStrykerInputs, IFileSystem, Action<string>>(
                (inputs, _, _) => inputs.OutputPathInput.SuppliedInput = outputPath)
            .Returns(outputPath);
        using var services = new ServiceCollection()
            .AddSingleton(runner.Object)
            .BuildServiceProvider();
        var service = new MutationServerService(
            services,
            configBuilder.Object,
            loggingInitializer.Object,
            Mock.Of<IAnsiConsole>());
        using var cancellationTokenSource = new CancellationTokenSource();

        var run = service.MutationTestAsync(
            new MutationTestParams(),
            _ => Task.CompletedTask,
            cancellationTokenSource.Token);
        cancellationTokenSource.Cancel();

        await Should.ThrowAsync<OperationCanceledException>(
            () => run.WaitAsync(TimeSpan.FromSeconds(1)));
        await service.WaitForIdleAsync().WaitAsync(TimeSpan.FromSeconds(1));
        runner.VerifyAll();
    }

    [TestMethod]
    public async Task MutationTestShouldSurfaceProgressFailuresAfterSuccessfulRuns()
    {
        var expectedException = new InvalidOperationException("Connection closed");
        var service = CreateMutationService(_ => Task.CompletedTask);

        var exception = await Should.ThrowAsync<InvalidOperationException>(() => service.MutationTestAsync(
            new MutationTestParams(),
            _ => Task.FromException(expectedException),
            CancellationToken.None));

        exception.InnerException.ShouldBeSameAs(expectedException);
    }

    [TestMethod]
    public async Task MutationTestShouldNotMaskRunnerFailuresWithProgressFailures()
    {
        var expectedRunException = new InputException("Mutation failed");
        var service = CreateMutationService(_ => Task.FromException(expectedRunException));

        var exception = await Should.ThrowAsync<InputException>(() => service.MutationTestAsync(
            new MutationTestParams(),
            _ => Task.FromException(new InvalidOperationException("Connection closed")),
            CancellationToken.None));

        exception.ShouldBeSameAs(expectedRunException);
    }

    private static (
        MutationServerService Service,
        Mock<IStrykerRunner> Runner,
        Mock<IConfigBuilder> ConfigBuilder,
        Mock<ILoggingInitializer> LoggingInitializer) CreateService()
    {
        var basePath = Path.GetFullPath("project");
        var outputPath = Path.Combine(basePath, "StrykerOutput", "test");
        var runner = new Mock<IStrykerRunner>(MockBehavior.Strict);
        runner.Setup(current => current.DiscoverMutantsAsync(
                It.Is<IStrykerInputs>(inputs => inputs.OutputPathInput.SuppliedInput == outputPath),
                It.IsAny<Stryker.Abstractions.Reporting.IReporter>(),
                It.IsAny<Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var configBuilder = new Mock<IConfigBuilder>(MockBehavior.Strict);
        configBuilder
            .Setup(builder => builder.Build(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<string[]>(),
                It.IsAny<CommandLineApplication>(),
                It.IsAny<CommandLineConfigReader>()))
            .Callback<IStrykerInputs, string[], CommandLineApplication, CommandLineConfigReader>(
                (inputs, _, _, _) => inputs.BasePathInput.SuppliedInput = basePath);
        var loggingInitializer = new Mock<ILoggingInitializer>(MockBehavior.Strict);
        loggingInitializer
            .Setup(initializer => initializer.InitializeOutputPath(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<IFileSystem>(),
                It.IsAny<Action<string>>()))
            .Callback<IStrykerInputs, IFileSystem, Action<string>>(
                (inputs, _, _) => inputs.OutputPathInput.SuppliedInput = outputPath)
            .Returns(outputPath);
        var services = new ServiceCollection()
            .AddSingleton(runner.Object)
            .BuildServiceProvider();
        var service = new MutationServerService(
            services,
            configBuilder.Object,
            loggingInitializer.Object,
            Mock.Of<IAnsiConsole>());

        return (service, runner, configBuilder, loggingInitializer);
    }

    private static MutationServerService CreateMutationService(
        Func<Stryker.Abstractions.Reporting.IReporter, Task> completeRun)
    {
        var basePath = Path.GetFullPath("project");
        var outputPath = Path.Combine(basePath, "StrykerOutput", "test");
        var runner = new Mock<IStrykerRunner>(MockBehavior.Strict);
        runner.Setup(current => current.RunMutationTestAsync(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<Stryker.Abstractions.Reporting.IReporter>(),
                It.IsAny<Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>>(),
                It.IsAny<CancellationToken>()))
            .Returns<IStrykerInputs, Stryker.Abstractions.Reporting.IReporter,
                Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>, CancellationToken>(
                async (_, reporter, _, _) =>
                {
                    var project = CreateProjectWithTerminalMutant(basePath);
                    reporter.OnMutantsCreated(project, null);
                    await completeRun(reporter);
                    return new StrykerRunResult(
                        new StrykerOptions { Thresholds = new Thresholds() },
                        1);
                });
        var configBuilder = new Mock<IConfigBuilder>();
        configBuilder
            .Setup(builder => builder.Build(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<string[]>(),
                It.IsAny<CommandLineApplication>(),
                It.IsAny<CommandLineConfigReader>()))
            .Callback<IStrykerInputs, string[], CommandLineApplication, CommandLineConfigReader>(
                (inputs, _, _, _) => inputs.BasePathInput.SuppliedInput = basePath);
        var loggingInitializer = new Mock<ILoggingInitializer>();
        loggingInitializer
            .Setup(initializer => initializer.InitializeOutputPath(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<IFileSystem>(),
                It.IsAny<Action<string>>()))
            .Callback<IStrykerInputs, IFileSystem, Action<string>>(
                (inputs, _, _) => inputs.OutputPathInput.SuppliedInput = outputPath)
            .Returns(outputPath);
        var services = new ServiceCollection()
            .AddSingleton(runner.Object)
            .BuildServiceProvider();
        return new MutationServerService(
            services,
            configBuilder.Object,
            loggingInitializer.Object,
            Mock.Of<IAnsiConsole>());
    }

    private static FolderComposite CreateProjectWithTerminalMutant(string basePath)
    {
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
        return project;
    }
}
