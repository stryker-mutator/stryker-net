using System;
using Shouldly;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class ExcludableStringTests : TestBase
    {
        [Fact]
        public void ExcludableString_Null()
        {
            _ = Assert.Throws<ArgumentNullException>(() => new ExcludableString(null));
        }

        [Fact]
        public void ExcludableString_Globs()
        {
            var s1 = new ExcludableString(@"Person.cs");
            var s2 = new ExcludableString(@"!Person.cs");

            s1.IsExcluded.ShouldBeFalse();
            s2.IsExcluded.ShouldBeTrue();
            s1.Glob.ToString().ShouldBe(s2.Glob.ToString());
        }

        [Fact]
        public void ExcludableString_MutantSpans()
        {
            var s1 = new ExcludableString(@"src/Person.cs{10..100}");
            var s2 = new ExcludableString(@"src/Person.cs");

            s1.MutantSpans.ShouldBe("{10..100}");
            s2.MutantSpans.ShouldBeEmpty();
        }
    }
}
