using System;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Core.UnitTest
{
    [TestClass]
    public class ExclusionPatternTests : TestBase
    {
        [TestMethod]
        public void ExclusionPattern_Null()
        {
            _ = Should.Throw<ArgumentNullException>(() => new ExclusionPattern(null));
        }

        [TestMethod]
        public void ExclusionPattern_Globs()
        {
            var s1 = new ExclusionPattern(@"Person.cs");
            var s2 = new ExclusionPattern(@"!Person.cs");

            s1.IsExcluded.ShouldBeFalse();
            s2.IsExcluded.ShouldBeTrue();
            s1.Glob.ToString().ShouldBe(s2.Glob.ToString());
        }

        [TestMethod]
        public void ExclusionPattern_MutantSpans()
        {
            var s1 = new ExclusionPattern(@"src/Person.cs{10..100}");
            var s2 = new ExclusionPattern(@"src/Person.cs");

            s1.MutantSpans.ShouldBe(new [] { (10, 100)});
            s2.MutantSpans.ShouldBeEmpty();
        }
    }
}
