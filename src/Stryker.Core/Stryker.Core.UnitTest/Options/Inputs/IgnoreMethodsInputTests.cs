using System.Linq;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Abstractions.Exceptions;
using Stryker.Configuration.Options.Inputs;

namespace Stryker.Core.UnitTest.Options.Inputs;

[TestClass]
public class IgnoreMethodsInputTests : TestBase
{
    [TestMethod]
    public void ShouldHaveHelpText()
    {
        var target = new IgnoreMethodsInput();
        target.HelpText.ShouldBe(@"Ignore mutations in calls to the given methods. Add (*) to a method name to ignore only the mutations in the arguments of its calls. | default: []");
    }

    [TestMethod]
    public void ShouldReturnRegex()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose" } };

        var result = target.Validate();

        result.ShouldHaveSingleItem().ToString().ShouldBe(@"^(?:[^.]*\.)*Dispose(<[^>]*>)?$");
    }

    [TestMethod]
    public void ShouldReturnMultipleItems()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = new[] { "Dispose", "Test" } };

        var result = target.Validate();

        result.Count().ShouldBe(2);
        result.First().ToString().ShouldBe(@"^(?:[^.]*\.)*Dispose(<[^>]*>)?$");
        result.Last().ToString().ShouldBe(@"^(?:[^.]*\.)*Test(<[^>]*>)?$");
    }

    [TestMethod]
    public void ShouldHaveDefault()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = null };

        var result = target.Validate();

        result.ShouldBeEmpty();
        target.ValidateArgumentsOnly().ShouldBeEmpty();
    }

    [TestMethod]
    public void ShouldSplitArgumentsOnlyPatternsFromWholeCallPatterns()
    {
        var target = new IgnoreMethodsInput { SuppliedInput = new[] { "Log*", "ConfigureAwait(*)", "*Exception.ctor(*)" } };

        target.Validate().ShouldHaveSingleItem().ToString().ShouldBe(@"^(?:[^.]*\.)*Log[^.]*(<[^>]*>)?$");
        target.ValidateArgumentsOnly().Select(r => r.ToString()).ShouldBe(new[]
        {
            @"^(?:[^.]*\.)*ConfigureAwait(<[^>]*>)?$",
            @"^(?:[^.]*\.)*[^.]*Exception\.ctor(<[^>]*>)?$",
        });
    }

    [TestMethod]
    [DataRow("ConfigureAwait()")]
    [DataRow("ConfigureAwait(false)")]
    [DataRow("ConfigureAwait(*, *)")]
    [DataRow("ConfigureAwait(*)(*)")]
    [DataRow("Configure(*)Await")]
    [DataRow("(*)")]
    public void ShouldRejectOtherParenthesisedPatterns(string pattern)
    {
        var target = new IgnoreMethodsInput { SuppliedInput = new[] { "Log*", pattern } };

        var wholeCallException = Should.Throw<InputException>(() => target.Validate());
        Should.Throw<InputException>(() => target.ValidateArgumentsOnly());

        wholeCallException.Message.ShouldBe($"Invalid ignore-methods pattern ({pattern}). Use a method name, optionally followed by (*) to ignore only the mutations in the arguments of its calls, for example ConfigureAwait(*).");
    }
}
