using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options.Inputs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    [TestClass]
    public class LanguageVersionInputTests : TestBase
    {
        [TestMethod]
        public void ShouldHaveHelpText()
        {
            var target = new LanguageVersionInput();
            target.HelpText.ShouldBe(@"The c# version used in compilation. | default: 'latest' | allowed: Default, CSharp2, CSharp3, CSharp4, CSharp5, CSharp6, CSharp7, CSharp7_1, CSharp7_2, CSharp7_3, CSharp8, CSharp9, CSharp10, CSharp11, CSharp12, LatestMajor, Preview, Latest");
        }

        [TestMethod]
        public void ShouldHaveDefault()
        {
            var target = new LanguageVersionInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBe(LanguageVersion.Default);
        }

        [TestMethod]
        public void ShouldReturnLanguageVersion()
        {
            var target = new LanguageVersionInput { SuppliedInput = "CSharp9" };

            var result = target.Validate();

            result.ShouldBe(LanguageVersion.CSharp9);
        }

        [TestMethod]
        public void ShouldValidateLanguageVersion()
        {
            var target = new LanguageVersionInput { SuppliedInput = "gibberish" };

            var ex = Should.Throw<InputException>(() => target.Validate());

            ex.Message.ShouldBe($"The given c# language version (gibberish) is invalid. Valid options are: [{string.Join(", ", ((IEnumerable<LanguageVersion>)Enum.GetValues(typeof(LanguageVersion))).Where(l => l != LanguageVersion.CSharp1))}]");
        }
    }
}
