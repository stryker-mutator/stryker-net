using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Reporting;
using Stryker.Core.MutationTest;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.Initialisation;

public interface IProjectMutator
{
    IMutationTestProcess MutateProject(IStrykerOptions options, MutationTestInput input, IReporter reporters, IMutationTestProcess mutationTestProcess = null);

    /// <summary>
    /// Enriches the test projects info with unit test information from the initial test run.
    /// This should be called after the initial test run has completed.
    /// </summary>
    void EnrichWithInitialTestRunInfo(MutationTestInput input);
}

public class ProjectMutator : IProjectMutator
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public ProjectMutator(ILogger<ProjectMutator> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IMutationTestProcess MutateProject(IStrykerOptions options, MutationTestInput input, IReporter reporters, IMutationTestProcess mutationTestProcess = null)
    {
        var process = mutationTestProcess ?? _serviceProvider.GetRequiredService<IMutationTestProcess>();
        process.Initialize(input, options, reporters);

        process.Mutate();

        return process;
    }

    /// <inheritdoc/>
    public void EnrichWithInitialTestRunInfo(MutationTestInput input)
    {
        if (input.InitialTestRun is null)
        {
            return;
        }

        EnrichTestProjectsWithTestInfo(input.InitialTestRun, input.TestProjectsInfo);
    }

    private void EnrichTestProjectsWithTestInfo(InitialTestRun initialTestRun, ITestProjectsInfo testProjectsInfo)
    {
        var unitTests =
            initialTestRun.Result.TestDescriptions
            .Select(desc => desc.Case)
            // F# has a different syntax tree and would throw further down the line
            .Where(unitTest => Path.GetExtension(unitTest.CodeFilePath) == ".cs");

        foreach (var unitTest in unitTests)
        {
            var testFile = testProjectsInfo.TestFiles.SingleOrDefault(testFile => testFile.FilePath == unitTest.CodeFilePath);
            if (testFile is not null)
            {
                var lineSpan = testFile.SyntaxTree.GetText().Lines[unitTest.LineNumber - 1].Span;
                var nodesInSpan = testFile.SyntaxTree.GetRoot().DescendantNodes(lineSpan);
                var node = nodesInSpan.First(n => n is MethodDeclarationSyntax);
                testFile.AddTest(unitTest.Id, unitTest.FullyQualifiedName, node);
            }
            else
            {
                _logger.LogDebug("Could not locate unit test in any testfile. This should not happen and results in incorrect test reporting.");
            }
        }
    }
}
