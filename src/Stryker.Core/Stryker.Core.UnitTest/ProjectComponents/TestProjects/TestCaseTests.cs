using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Testing;
using Stryker.Core.ProjectComponents.TestProjects;

namespace Stryker.Core.UnitTest.ProjectComponents.TestProjects;

[TestClass]
public class TestCaseTests
{
    [TestMethod]
    public void TestCaseEqualsWhenAllPropertiesEqual()
    {
        // Arrange
        var id = Guid.NewGuid().ToString();
        var node = SyntaxFactory.Block();
        var testCaseA = new TestCase
        {
            Id = id,
            Name = "1",
            Node = node
        };
        var testCaseB = new TestCase
        {
            Id = id,
            Name = "1",
            Node = node
        };

        // Assert
        testCaseA.ShouldBe(testCaseB);
        testCaseA.GetHashCode().ShouldBe(testCaseB.GetHashCode());
    }

    [TestMethod]
    [DataRow("fd4896a2-1bd9-4e83-9e81-308059525bc9", "node2")]
    [DataRow("00000000-0000-0000-0000-000000000000", "node1")]
    public void TestCaseNotEqualsWhenNotAllPropertiesEqual(string guid, string name)
    {
        // Arrange
        var node = SyntaxFactory.Block();
        var testCaseA = new TestCase
        {
            Id = guid,
            Name = name,
            Node = node
        };
        var testCaseB = new TestCase
        {
            Id = "00000000-0000-0000-0000-000000000000",
            Name = "node2",
            Node = node
        };

        // Assert
        testCaseA.ShouldNotBe(testCaseB);
    }
}
