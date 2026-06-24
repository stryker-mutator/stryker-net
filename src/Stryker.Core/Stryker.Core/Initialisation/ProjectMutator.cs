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
    IMutationTestProcess MutateProject(IStrykerOptions options, MutationTestInput input, IReporter reporters, IMutationTestProcess mutationTestProcess = null);
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

        // Enrich test projects info with unit tests
        EnrichTestProjectsWithTestInfo(input.InitialTestRun, input.TestProjectsInfo);

        // mutate
        process.Mutate();

        return process;
    }

    private void EnrichTestProjectsWithTestInfo(InitialTestRun initialTestRun, ITestProjectsInfo testProjectsInfo)
    {
        var unitTests = initialTestRun.Result.TestDescriptions.Select(desc => desc.Case);

        foreach (var unitTest in unitTests)
        {
            ITestFile? testFile = null;
            SyntaxNode? node = null;

            // Primary: use location info when the framework provides it (e.g. XUnit v3, TUnit)
            if (Path.GetExtension(unitTest.CodeFilePath) == ".cs" && unitTest.LineNumber > 0)
            {
                testFile = testProjectsInfo.TestFiles.SingleOrDefault(tf => tf.FilePath == unitTest.CodeFilePath);
                if (testFile is not null)
                {
                    var lineSpan = testFile.SyntaxTree.GetText().Lines[unitTest.LineNumber - 1].Span;
                    var nodesInSpan = testFile.SyntaxTree.GetRoot().DescendantNodes(lineSpan);
                    node = nodesInSpan.FirstOrDefault(n => n is MethodDeclarationSyntax);
                }
            }

            // Fallback: search by method name when location info is missing (e.g. NUnit and MSTest with MTP)
            if (node is null)
            {
                (testFile, node) = FindTestMethodByName(unitTest, testProjectsInfo.TestFiles);
            }

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
