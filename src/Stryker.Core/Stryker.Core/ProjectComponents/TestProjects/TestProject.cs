using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.MutantFilters;

namespace Stryker.Core.ProjectComponents.TestProjects;

public sealed class TestProject : IEquatable<ITestProject>, ITestProject
{
    public IAnalyzerResult AnalyzerResult { get; }

    public string ProjectFilePath => AnalyzerResult.ProjectFilePath;
    public IEnumerable<ITestFile> TestFiles { get; }

    public TestProject(IFileSystem fileSystem, IAnalyzerResult testProjectAnalyzerResult, string runRootDir = null)
    {
        AssertValidTestProject(testProjectAnalyzerResult);

        fileSystem ??= new FileSystem();

        AnalyzerResult = testProjectAnalyzerResult;

        // Only touch the file system's Path abstraction when there are actually files to process;
        // some callers use a strict IFileSystem mock that doesn't set up Path.
        string rootDir = null;
        if (testProjectAnalyzerResult.SourceFiles is { Length: > 0 })
        {
            var testProjectDir = fileSystem.Path.GetDirectoryName(testProjectAnalyzerResult.ProjectFilePath) ?? string.Empty;
            rootDir = string.IsNullOrEmpty(runRootDir) ? testProjectDir : fileSystem.Path.GetFullPath(runRootDir);
        }

        var testFiles = new List<TestFile>();
        foreach (var file in testProjectAnalyzerResult.SourceFiles)
        {
            var sourceCode = fileSystem.File.ReadAllText(file);
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode,
                path: file,
                encoding: Encoding.UTF32,
                options: new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None, preprocessorSymbols: testProjectAnalyzerResult.PreprocessorSymbols));

            if (!syntaxTree.IsGenerated())
            {
                testFiles.Add(new TestFile
                {
                    SyntaxTree = syntaxTree,
                    FilePath = file,
                    Source = sourceCode,
                    RelativePath = GetRelativePathSafe(fileSystem, rootDir, file)
                });
            }
        }

        TestFiles = testFiles;
    }

    // Falls back to the bare file name if a relative path can't be computed (e.g. malformed paths in
    // some test doubles) so this never crashes; only used for report/baseline keying, never for I/O.
    private static string GetRelativePathSafe(IFileSystem fileSystem, string rootDir, string file)
    {
        try
        {
            return fileSystem.Path.GetRelativePath(rootDir, file);
        }
        catch (ArgumentException)
        {
            return fileSystem.Path.GetFileName(file);
        }
    }

    private static void AssertValidTestProject(IAnalyzerResult testProjectAnalyzerResult)
    {
        if (testProjectAnalyzerResult.References.Any(r => r.Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework")))
        {
            throw new InputException("Please upgrade your test projects to MsTest V2. Stryker.NET uses VSTest which does not support MsTest V1.",
                @"See https://devblogs.microsoft.com/devops/upgrade-to-mstest-v2/ for upgrade instructions.");
        }
    }

    public bool Equals(ITestProject other) => other.AnalyzerResult.Equals(AnalyzerResult) && other.TestFiles.SequenceEqual(TestFiles);

    public override bool Equals(object obj) => obj is ITestProject project && Equals(project);

    public override int GetHashCode() => AnalyzerResult.GetHashCode();
}
