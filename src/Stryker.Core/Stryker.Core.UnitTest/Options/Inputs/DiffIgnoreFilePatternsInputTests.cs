using System.Linq;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class DiffIgnoreFilePatternsInputTests
    {
        [Fact]
        public void ShouldAcceptGlob()
        {
            var target = new DiffIgnoreFilePatternsInput { SuppliedInput = new[] { "*" } };

            var result = target.Validate();

            result.ShouldHaveSingleItem().Glob.ToString().ShouldBe("*");
        }
        
        [Fact]
        public void ShouldParseAll()
        {
            var target = new DiffIgnoreFilePatternsInput { SuppliedInput = new[] { "*", "MyFile.cs" } };

            var result = target.Validate();

            result.Count().ShouldBe(2);

            result.First().Glob.ToString().ShouldBe("*");
            result.Last().Glob.ToString().ShouldBe("MyFile.cs");
        }

        [Fact]
        public void ShouldHaveDefault()
        {
            var target = new DiffIgnoreFilePatternsInput { SuppliedInput = null };

            var result = target.Validate();

            result.ShouldBeEmpty();
        }
    }
}
