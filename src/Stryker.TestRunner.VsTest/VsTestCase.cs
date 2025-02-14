using System;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Stryker.Abstractions.Testing;

namespace Stryker.TestRunner.VsTest;

public class VsTestCase : ITestCase
{
    public VsTestCase(TestCase testCase)
    {
        OriginalTestCase = testCase;
        Id = testCase.Id.ToString();
        Guid = testCase.Id;
        Name = testCase.DisplayName;
        FullyQualifiedName = testCase.FullyQualifiedName;
        Uri = testCase.ExecutorUri;
        CodeFilePath = testCase.CodeFilePath ?? string.Empty;
        LineNumber = testCase.LineNumber;
        Source = testCase.Source;
    }

    public TestCase OriginalTestCase { get; }

    public string Id { get; }

    public Guid Guid { get; }

    public string Name { get; }

    public Uri Uri { get; }

    public string CodeFilePath { get; }

    public string FullyQualifiedName { get; }

    public int LineNumber { get; }

    public string Source { get; }
}
