using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Configuration.Options.Inputs;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class MutateInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new MutateInput();
        target.HelpText.ShouldBe(@"Allows to specify file that should in- or excluded for the mutations.
    Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
    Use '!' at the start of a pattern to exclude all matched files.
    Use '{<start>..<end>}' at the end of a pattern to specify spans of text in files to in- or exclude.
    Example: ['**/*Service.cs','!**/MySpecialService.cs', '**/MyOtherService.cs{1..10}{32..45}'] | default: ['**/*']");
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new MutateInput { SuppliedInput = new string[] { } };

        var result = target.Validate();

        var item = result.ShouldHaveSingleItem();
        item.Glob.ToString().ShouldBe(Path.Combine("**", "*"));
        item.IsExclude.ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldReturnFiles()
    {
        var target = new MutateInput { SuppliedInput = new[] { Path.Combine("**", "*.cs") } };

        var result = target.Validate();

        var item = result.ShouldHaveSingleItem();
        item.Glob.ToString().ShouldBe(Path.Combine("**", "*.cs"));
        item.IsExclude.ShouldBeFalse();
    }

    [TestMethod]
    public void ShouldExcludeAll()
    {
        var target = new MutateInput { SuppliedInput = new[] { "!" + Path.Combine("**", "Test.cs") } };

        var result = target.Validate();

        result.Count().ShouldBe(2);
        result.First().Glob.ToString().ShouldBe(Path.Combine("**", "Test.cs"));
        result.First().IsExclude.ShouldBeTrue();
        result.Last().Glob.ToString().ShouldBe(Path.Combine("**", "*"));
        result.Last().IsExclude.ShouldBeFalse();
    }

    [TestMethod]
    public void WhenDefaultPatternsAreUsed_ShouldLogFileMutationPatterns()
    {
        var loggerMock = new Mock<ILogger<MutateInput>>();
        var target = new MutateInput();

        target.Validate(loggerMock.Object);

        loggerMock.Verify(LogLevel.Information, "File mutation patterns: **/*.");
        loggerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void WhenSuppliedInputIsEmpty_ShouldLogDefaultFileMutationPatterns()
    {
        var loggerMock = new Mock<ILogger<MutateInput>>();
        var target = new MutateInput { SuppliedInput = new string[] { } };

        target.Validate(loggerMock.Object);

        loggerMock.Verify(LogLevel.Information, "File mutation patterns: **/*.");
        loggerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void WhenIncludePatternsAreSupplied_ShouldLogFileMutationPatterns()
    {
        var loggerMock = new Mock<ILogger<MutateInput>>();
        var pattern = Path.Combine("**", "*.cs");
        var target = new MutateInput { SuppliedInput = new[] { pattern } };

        target.Validate(loggerMock.Object);

        loggerMock.Verify(LogLevel.Information, $"File mutation patterns: {pattern}.");
        loggerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void WhenOnlyExcludePatternsAreSupplied_ShouldLogEffectiveFileMutationPatterns()
    {
        var loggerMock = new Mock<ILogger<MutateInput>>();
        var excludePattern = "!" + Path.Combine("**", "Test.cs");
        var target = new MutateInput { SuppliedInput = new[] { excludePattern } };

        target.Validate(loggerMock.Object);

        loggerMock.Verify(LogLevel.Information, $"File mutation patterns: {excludePattern}, **/*.");
        loggerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void WhenIncludeAndExcludePatternsAreSupplied_ShouldLogFileMutationPatterns()
    {
        var loggerMock = new Mock<ILogger<MutateInput>>();
        var includePattern = Path.Combine("**", "*Service.cs");
        var excludePattern = "!" + Path.Combine("**", "MySpecialService.cs");
        var target = new MutateInput { SuppliedInput = new[] { includePattern, excludePattern } };

        target.Validate(loggerMock.Object);

        loggerMock.Verify(LogLevel.Information, $"File mutation patterns: {includePattern}, {excludePattern}.");
        loggerMock.VerifyNoOtherCalls();
    }

    public static IEnumerable<object[]> MultiplePatternLogCases()
    {
        yield return new object[] { new[] { "**/Program.cs", "!**/Controllers/**" }, "File mutation patterns: **/Program.cs, !**/Controllers/**." };
        yield return new object[] { new[] { "!**/A.cs", "!**/B.cs" }, "File mutation patterns: !**/A.cs, !**/B.cs, **/*." };
    }

    [DataTestMethod]
    [DynamicData(nameof(MultiplePatternLogCases), DynamicDataSourceType.Method)]
    public void WhenPatternsAreSupplied_ShouldLogJoinedFileMutationPatterns(string[] patterns, string expectedLogMessage)
    {
        var loggerMock = new Mock<ILogger<MutateInput>>();
        var target = new MutateInput { SuppliedInput = patterns };

        target.Validate(loggerMock.Object);

        loggerMock.Verify(LogLevel.Information, expectedLogMessage);
        loggerMock.VerifyNoOtherCalls();
    }

}
