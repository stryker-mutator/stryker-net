using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Shared.Tests;

namespace Stryker.TestRunner.VSTest;
public class VsTestCase : ITestCase
{
    public VsTestCase(TestCase testCase)
    {
        Id = testCase.Id;
        Name = testCase.DisplayName;
        FullyQualifiedName = testCase.FullyQualifiedName;
        Uri = testCase.ExecutorUri;
        CodeFilePath = testCase.CodeFilePath ?? string.Empty;
        LineNumber = testCase.LineNumber;
        Source = testCase.Source;
    }

    public Guid Id { get; }

    public string Name { get; }

    public Uri Uri { get; }

    public string CodeFilePath { get; }

    public string FullyQualifiedName { get; }

    public int LineNumber { get; }

    public string Source { get; }
}
