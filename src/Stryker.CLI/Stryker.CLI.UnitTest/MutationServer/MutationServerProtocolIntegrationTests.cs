using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nerdbank.Streams;
using Shouldly;
using Spectre.Console;
using Stryker.Abstractions;
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
public class MutationServerProtocolIntegrationTests
{
    [TestMethod]
    public async Task MutationTestProgressShouldUseObjectParameters()
    {
        var basePath = Path.GetFullPath("project");
        var runner = CreateRunner(basePath);
        using var services = new ServiceCollection()
            .AddSingleton(runner.Object)
            .BuildServiceProvider();
        var service = CreateService(services, basePath);
        var host = new MutationServerHost(service);
        var (serverStream, clientStream) = FullDuplexStream.CreatePair();
        using var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var serverTask = host.RunConnectionAsync(serverStream, serverStream, cancellationTokenSource.Token);

        await WriteMessageAsync(
            clientStream,
            """
            {"jsonrpc":"2.0","id":1,"method":"mutationTest","params":{}}
            """);
        using var notification = await ReadMessageAsync(clientStream, cancellationTokenSource.Token);
        using var response = await ReadMessageAsync(clientStream, cancellationTokenSource.Token);

        notification.RootElement.GetProperty("method").GetString()
            .ShouldBe(MutationServerProtocol.ReportMutationTestProgressMethod);
        notification.RootElement.GetProperty("params").ValueKind.ShouldBe(JsonValueKind.Object);
        notification.RootElement.GetProperty("params").GetProperty("files").ValueKind
            .ShouldBe(JsonValueKind.Object);
        response.RootElement.GetProperty("id").GetInt32().ShouldBe(1);
        response.RootElement.GetProperty("result").GetProperty("files").EnumerateObject()
            .ShouldBeEmpty();

        clientStream.Dispose();
        await serverTask.WaitAsync(cancellationTokenSource.Token);
        runner.VerifyAll();
    }

    [TestMethod]
    public async Task ClosingTheConnectionShouldCancelAnActiveMutationTest()
    {
        var basePath = Path.GetFullPath("project");
        var runner = new Mock<IStrykerRunner>(MockBehavior.Strict);
        var runStarted = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);
        var cancellationObserved = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);
        runner.Setup(current => current.RunMutationTestAsync(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<Stryker.Abstractions.Reporting.IReporter>(),
                It.IsAny<Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>>(),
                It.IsAny<CancellationToken>()))
            .Returns<IStrykerInputs, Stryker.Abstractions.Reporting.IReporter,
                Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>, CancellationToken>(
                async (_, _, _, cancellationToken) =>
                {
                    runStarted.SetResult();
                    try
                    {
                        await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
                    }
                    finally
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            cancellationObserved.SetResult();
                        }
                    }

                    return null;
                });
        using var services = new ServiceCollection()
            .AddSingleton(runner.Object)
            .BuildServiceProvider();
        var host = new MutationServerHost(CreateService(services, basePath));
        var (serverStream, clientStream) = FullDuplexStream.CreatePair();
        var serverTask = host.RunConnectionAsync(serverStream, serverStream, CancellationToken.None);

        await WriteMessageAsync(
            clientStream,
            """
            {"jsonrpc":"2.0","id":1,"method":"mutationTest","params":{}}
            """);
        await runStarted.Task.WaitAsync(TimeSpan.FromSeconds(1));
        clientStream.Dispose();

        await cancellationObserved.Task.WaitAsync(TimeSpan.FromSeconds(1));
        await serverTask.WaitAsync(TimeSpan.FromSeconds(1));
        runner.VerifyAll();
    }

    private static Mock<IStrykerRunner> CreateRunner(string basePath)
    {
        var runner = new Mock<IStrykerRunner>(MockBehavior.Strict);
        runner.Setup(current => current.RunMutationTestAsync(
                It.IsAny<IStrykerInputs>(),
                It.IsAny<Stryker.Abstractions.Reporting.IReporter>(),
                It.IsAny<Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>>(),
                It.IsAny<CancellationToken>()))
            .Returns<IStrykerInputs, Stryker.Abstractions.Reporting.IReporter,
                Func<IReadOnlyFileLeaf, IReadOnlyMutant, bool>, CancellationToken>(
                (_, reporter, _, _) =>
                {
                    var filePath = Path.Combine(basePath, "src", "Calculator.cs");
                    var syntaxTree = CSharpSyntaxTree.ParseText(
                        "class Calculator { bool Calculate() => true; }",
                        path: filePath);
                    var originalNode = syntaxTree.GetRoot().DescendantNodes()
                        .OfType<LiteralExpressionSyntax>()
                        .Single();
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
                        RelativePath = Path.Combine("src", "Calculator.cs"),
                        SyntaxTree = syntaxTree,
                        Mutants = [mutant]
                    };
                    var project = new FolderComposite { FullPath = basePath };
                    project.Add(file);
                    reporter.OnMutantsCreated(project, null);
                    reporter.OnStartMutantTestRun([mutant]);
                    mutant.ResultStatus = MutantStatus.Killed;
                    reporter.OnMutantTested(mutant);
                    reporter.OnAllMutantsTested(project, null);
                    return Task.FromResult(new StrykerRunResult(
                        new StrykerOptions { Thresholds = new Thresholds() },
                        1));
                });
        return runner;
    }

    private static MutationServerService CreateService(IServiceProvider services, string basePath)
    {
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
                (inputs, _, _) => inputs.OutputPathInput.SuppliedInput =
                    Path.Combine(basePath, "StrykerOutput", "test"))
            .Returns(Path.Combine(basePath, "StrykerOutput", "test"));
        return new MutationServerService(
            services,
            configBuilder.Object,
            loggingInitializer.Object,
            Mock.Of<IAnsiConsole>());
    }

    private static async Task WriteMessageAsync(Stream stream, string json)
    {
        var payload = Encoding.UTF8.GetBytes(json);
        var header = Encoding.ASCII.GetBytes($"Content-Length: {payload.Length}\r\n\r\n");
        await stream.WriteAsync(header);
        await stream.WriteAsync(payload);
        await stream.FlushAsync();
    }

    private static async Task<JsonDocument> ReadMessageAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        var contentLength = 0;
        while (true)
        {
            var line = await ReadLineAsync(stream, cancellationToken);
            if (line.Length == 0)
            {
                break;
            }

            const string ContentLengthHeader = "Content-Length:";
            if (line.StartsWith(ContentLengthHeader, StringComparison.OrdinalIgnoreCase))
            {
                contentLength = int.Parse(line[ContentLengthHeader.Length..]);
            }
        }

        var payload = new byte[contentLength];
        await stream.ReadExactlyAsync(payload, cancellationToken);
        return JsonDocument.Parse(payload);
    }

    private static async Task<string> ReadLineAsync(
        Stream stream,
        CancellationToken cancellationToken)
    {
        var bytes = new List<byte>();
        var buffer = new byte[1];
        while (await stream.ReadAsync(buffer, cancellationToken) == 1)
        {
            if (buffer[0] == '\n')
            {
                break;
            }

            if (buffer[0] != '\r')
            {
                bytes.Add(buffer[0]);
            }
        }

        return Encoding.ASCII.GetString(bytes.ToArray());
    }
}
