using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class LanguageVersionInputTests : TestBase
{
    private readonly Mock<ILogger<LanguageVersionInput>> _loggerMock = new();

    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new LanguageVersionInput();
        target.HelpText.ShouldBe(@"Deprecated: configure the C# language version in the project file. This option is ignored. | allowed: Default, CSharp2, CSharp3, CSharp4, CSharp5, CSharp6, CSharp7, CSharp7_1, CSharp7_2, CSharp7_3, CSharp8, CSharp9, CSharp10, CSharp11, CSharp12, CSharp13, CSharp14, LatestMajor, Preview, Latest");
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new LanguageVersionInput { SuppliedInput = null };

        target.Default.ShouldBeNull();
        var result = target.Validate(_loggerMock.Object);

        result.ShouldBe(LanguageVersion.Default);
        _loggerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void ShouldIgnoreLanguageVersion()
    {
        var target = new LanguageVersionInput { SuppliedInput = "CSharp9" };

        var result = target.Validate(_loggerMock.Object);

        result.ShouldBe(LanguageVersion.Default);
        _loggerMock.Verify(LogLevel.Warning, "The language-version option is deprecated and ignored. Configure LangVersion in the project file instead.", Times.Once);
        _loggerMock.VerifyNoOtherCalls();
    }

    [TestMethod]
    public void ShouldValidateLanguageVersion()
    {
        var target = new LanguageVersionInput { SuppliedInput = "gibberish" };

        var ex = Should.Throw<InputException>(() => target.Validate(_loggerMock.Object));

        ex.Message.ShouldBe($"The given c# language version (gibberish) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<LanguageVersion>)Enum.GetValues(typeof(LanguageVersion))).Where(l => l != LanguageVersion.CSharp1))}]");
        _loggerMock.VerifyNoOtherCalls();
    }
}
