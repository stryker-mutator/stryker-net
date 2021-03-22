using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class SinceBranchInputTests
    {
        [Fact]
        public void ShouldValidateGitSource()
        {
            var ex = Assert.Throws<StrykerInputException>(() =>
            {
                new SinceTargetInput { SuppliedInput = "" }.Validate(true);
            });
            ex.Message.ShouldBe("GitDiffTarget may not be empty, please provide a valid git branch name");
        }

        [Fact]
        public void ShouldNotValidateGitSourceWhenSinceIsTurnedOff()
        {
            var validatedSinceBranch = new SinceTargetInput { SuppliedInput = "" }.Validate(false);
            validatedSinceBranch.ShouldBe(null);
        }
    }
}
