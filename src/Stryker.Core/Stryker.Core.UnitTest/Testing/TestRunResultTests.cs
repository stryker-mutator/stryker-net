using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;

namespace Stryker.Core.UnitTest.Testing;

[TestClass]
public class TestRunResultTests
{
    [TestMethod]
    [DataRow(false, false)]
    [DataRow(false, true)]
    [DataRow(true, false)]
    public void ShouldEnumerateExecutedIdentifiersAtMostOnce(bool everyTest, bool empty)
    {
        var enumerations = 0;
        IEnumerable<string> Identifiers()
        {
            enumerations++;
            if (!empty)
            {
                yield return "a";
            }
        }

        var executedTests = new Mock<ITestIdentifiers>();
        executedTests.SetupGet(tests => tests.IsEveryTest).Returns(everyTest);
        executedTests.Setup(tests => tests.GetIdentifiers()).Returns(Identifiers());
        var descriptions = new[] { "a", "b", "A", "a" }
            .Select(id => Mock.Of<IFrameworkTestDescription>(test => test.Id == id)).ToArray();

        var result = new TestRunResult(descriptions, executedTests.Object,
            TestIdentifierList.NoTest(), TestIdentifierList.NoTest(), "", [], TimeSpan.Zero);

        result.TestDescriptions.ShouldBe(everyTest ? descriptions : empty ? [] : [descriptions[0], descriptions[3]]);
        enumerations.ShouldBe(everyTest ? 0 : 1);
    }
}
