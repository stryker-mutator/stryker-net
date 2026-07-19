using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Abstractions.Reporting;
using Stryker.Abstractions.Options;
using Stryker.Core.MutationTest;

namespace Stryker.Core.Initialisation;

public interface IProjectMutator
{
    /// <summary>
    /// Mutates the project syntax trees and stores them in memory.
    /// Does not compile or write to disk, making it safe to run in parallel with initial tests.
    /// </summary>
    IMutationTestProcess MutateProject(IStrykerOptions options, MutationTestInput input, IReporter reporters, IMutationTestProcess mutationTestProcess = null);

    /// <summary>
    /// Compiles the mutated project and writes the assembly to disk.
    /// Must be called after initial tests complete.
    /// </summary>
    void CompileProject(IMutationTestProcess process);

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
    public void CompileProject(IMutationTestProcess process)
    {
        process.Compile();
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
        var unitTests = initialTestRun.Result.TestDescriptions.Select(desc => desc.Case);

        foreach (var unitTest in unitTests)
        {
            // Primary: use location info when the framework provides it (e.g. XUnit v3, TUnit)
            if (unitTest.LineNumber != Stryker.Abstractions.Testing.ITestCase.LineNumberNotFound
                && TryAddTestByLineNumber(unitTest, testProjectsInfo))
            {
                continue;
            }

            // Fallback: search by method name when location info is missing (e.g. NUnit and MSTest with MTP)
            var (testFile, node) = FindTestMethodByName(unitTest, testProjectsInfo.TestFiles);
            if (testFile is not null && node is not null)
            {
                testFile.AddTest(unitTest.Id, unitTest.FullyQualifiedName, node);
            }
            else
            {
                _logger.LogDebug("Could not locate unit test in any testfile. This should not happen and results in incorrect test reporting.");
            }
        }
    }

    private static bool TryAddTestByLineNumber(Stryker.Abstractions.Testing.ITestCase unitTest, ITestProjectsInfo testProjectsInfo)
    {
        if (Path.GetExtension(unitTest.CodeFilePath) != ".cs")
        {
            return false;
        }

        var testFile = testProjectsInfo.TestFiles.SingleOrDefault(tf => tf.FilePath == unitTest.CodeFilePath);
        if (testFile is null)
        {
            return false;
        }

        var lines = testFile.SyntaxTree.GetText().Lines;
        if (unitTest.LineNumber > lines.Count)
        {
            return false;
        }

        var lineSpan = lines[unitTest.LineNumber - 1].Span;
        var node = testFile.SyntaxTree.GetRoot().DescendantNodes(lineSpan).FirstOrDefault(n => n is MethodDeclarationSyntax);
        if (node is null)
        {
            return false;
        }

        testFile.AddTest(unitTest.Id, unitTest.FullyQualifiedName, node);
        return true;
    }

    private static (ITestFile? testFile, SyntaxNode? node) FindTestMethodByName(Stryker.Abstractions.Testing.ITestCase unitTest, IEnumerable<ITestFile> testFiles)
    {
        // Use FullyQualifiedName when it's an FQN (NUnit UIDs are FQNs).
        // Fall back to Name (display name) when Id is a GUID (MSTest UIDs are GUIDs).
        var isGuid = Guid.TryParse(unitTest.Id, out _);
        var sourceName = isGuid ? unitTest.Name : unitTest.FullyQualifiedName;

        var (methodName, className, namespaceName) = ParseMethodComponents(sourceName);
        if (string.IsNullOrEmpty(methodName))
        {
            return (null, null);
        }

        foreach (var testFile in testFiles)
        {
            var root = testFile.SyntaxTree.GetRoot();
            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                .Where(m => m.Identifier.Text == methodName);

            if (className is not null)
            {
                methods = methods.Where(m =>
                    m.Ancestors().OfType<ClassDeclarationSyntax>()
                        .Any(c => c.Identifier.Text == className));
            }

            if (namespaceName is not null)
            {
                methods = methods.Where(m =>
                    m.Ancestors().OfType<BaseNamespaceDeclarationSyntax>()
                        .Any(ns => ns.Name.ToString() == namespaceName));
            }

            var method = methods.FirstOrDefault();
            if (method is not null)
            {
                return (testFile, method);
            }
        }

        return (null, null);
    }

    private static (string methodName, string? className, string? namespaceName) ParseMethodComponents(string sourceName)
    {
        var nameWithoutParams = sourceName.Contains('(')
            ? sourceName[..sourceName.IndexOf('(')].Trim()
            : sourceName.Trim();

        if (!nameWithoutParams.Contains('.'))
        {
            return (nameWithoutParams, null, null);
        }

        var parts = nameWithoutParams.Split('.');
        return (
            parts[^1],
            parts.Length >= 2 ? parts[^2] : null,
            parts.Length >= 3 ? string.Join(".", parts[..^2]) : null
        );
    }
}
