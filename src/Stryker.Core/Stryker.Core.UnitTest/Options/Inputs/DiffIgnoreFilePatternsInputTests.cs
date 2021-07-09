using System.Linq;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class DiffIgnoreFilePatternsInputTests
    {
        [Fact]
        public void ShouldHaveHelptext()
        {
            var target = new DiffIgnoreFilePatternsInput();
            target.HelpText.ShouldBe(@"Allows to specify an array of C# files which should be ignored if present in the diff.
Any non-excluded files will trigger all mutants to be tested because we cannot determine what mutants are affected by these files. 
This feature is only recommended when you are sure these files will not affect results, or when you are prepared to sacrifice accuracy for perfomance.

Use glob syntax for wildcards: https://en.wikipedia.org/wiki/Glob_(programming)
Example: ['**/*Assets.json','**/favicon.ico'] | default: []");
        }

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
