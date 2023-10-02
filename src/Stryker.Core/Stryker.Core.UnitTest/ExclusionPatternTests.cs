using System;
using Shouldly;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class ExclusionPatternTests : TestBase
    {
        [Fact]
        public void ExclusionPattern_Null()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new ExclusionPattern(null));
        }

        [Fact]
        public void ExclusionPattern_Globs()
        {
            var s1 = new ExclusionPattern(@"Person.cs");
            var s2 = new ExclusionPattern(@"!Person.cs");

            s1.IsExcluded.ShouldBeFalse();
            s2.IsExcluded.ShouldBeTrue();
            s1.Glob.ToString().ShouldBe(s2.Glob.ToString());
        }

        [Fact]
        public void ExclusionPattern_MutantSpans()
        {
            var s1 = new ExclusionPattern(@"src/Person.cs{10..100}");
            var s2 = new ExclusionPattern(@"src/Person.cs");

            s1.MutantSpans.ShouldBe(new [] { (10, 100)});
            s2.MutantSpans.ShouldBeEmpty();
        }
    }
}
