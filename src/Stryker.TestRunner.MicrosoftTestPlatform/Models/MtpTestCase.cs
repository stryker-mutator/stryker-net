using System;
using System.Diagnostics.CodeAnalysis;
using Stryker.Abstractions.Testing;
namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed class MtpTestCase : ITestCase
{
    private readonly TestNode _testNode;
    public MtpTestCase(TestNode testNode)
    {
        _testNode = testNode;
    }

    public string FullyQualifiedName { get; }
    public Uri Uri => new("executor://MicrosoftTestPlatform");
    public int LineNumber { get; }

    public string Source { get; }
    public string CodeFilePath => string.Empty;

    public string AssemblyPath { get; init; }

    public Guid Guid { get; }
    public string Name => _testNode.DisplayName;

    public string Id => _testNode.Uid;
}
