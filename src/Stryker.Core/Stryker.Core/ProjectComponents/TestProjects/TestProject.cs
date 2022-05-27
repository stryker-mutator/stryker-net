using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Exceptions;
using Stryker.Core.MutantFilters.Extensions;

namespace Stryker.Core.ProjectComponents.TestProjects
{
    public sealed class TestProject : IEquatable<TestProject>
    {
        private readonly IFileSystem _fileSystem;

        public IAnalyzerResult TestProjectAnalyzerResult { get; }

        public IEnumerable<TestFile> TestFiles { get; }

        public TestProject(IFileSystem fileSystem, IAnalyzerResult testProjectAnalyzerResult)
        {
            ValidateIsTestProject(testProjectAnalyzerResult);

            _fileSystem = fileSystem ?? new FileSystem();

            TestProjectAnalyzerResult = testProjectAnalyzerResult;

            var testFiles = new List<TestFile>();
            foreach (var file in testProjectAnalyzerResult.SourceFiles)
            {
                var source = _fileSystem.File.ReadAllText(file);
                if (IsNotGenerated(file, source))
                {
                    testFiles.Add(new TestFile
                    {
                        FilePath = file,
                        Source = source
                    });
                }
            }

            TestFiles = testFiles;
        }

        private bool IsNotGenerated(string file, string source)
        {
            var isGenerated = CSharpSyntaxTree.ParseText(source,
                    path: file,
                    encoding: Encoding.UTF32,
                    options: new CSharpParseOptions(LanguageVersion.Latest, DocumentationMode.None)).IsGenerated();
            return !isGenerated;
        }

        private static void ValidateIsTestProject(IAnalyzerResult testProjectAnalyzerResult)
        {
            if (testProjectAnalyzerResult.References.Any(r => r.Contains("Microsoft.VisualStudio.QualityTools.UnitTestFramework")))
            {
                throw new InputException("Please upgrade your test projects to MsTest V2. Stryker.NET uses VSTest which does not support MsTest V1.",
                    @"See https://devblogs.microsoft.com/devops/upgrade-to-mstest-v2/ for upgrade instructions.");
            };
        }

        public bool Equals(TestProject other) => other.TestProjectAnalyzerResult.Equals(TestProjectAnalyzerResult) && other.TestFiles.SequenceEqual(TestFiles);

        public override bool Equals(object obj) => obj is TestProject project && Equals(project);

        // Stryker disable once bitwise: Bitwise mutation does not change functional usage of GetHashCode
        public override int GetHashCode() => TestProjectAnalyzerResult.GetHashCode();
    }
}
