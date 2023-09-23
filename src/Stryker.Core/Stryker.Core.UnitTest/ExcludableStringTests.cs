using System;
using Shouldly;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class ExcludableStringTests : TestBase
    {
        [Fact]
        public void ExcludableString_NullCtor()
        {
            Assert.Throws<ArgumentNullException>(() => new ExcludableString(null));
        }

        [Fact]
        public void ExcludableString_NullParse()
        {
            Assert.Throws<ArgumentNullException>(() => ExcludableString.Parse(null));
        }

        [Fact]
        public void ExcludableString_Globs()
        {
            var s1 = new ExcludableString(@"src/Person.cs");
            var s2 = new ExcludableString(@"!src\Person.cs");

            s1.IsExcluded.ShouldBeFalse();
            s2.IsExcluded.ShouldBeTrue();
            s1.Glob.ToString().ShouldBe(s2.Glob.ToString());
        }
    }
}
