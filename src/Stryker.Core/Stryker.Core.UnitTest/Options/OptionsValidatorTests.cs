using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class OptionsValidatorTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void OptionsValidator_ShouldThrowValidationExceptionOnEmptyBasePathValues(string basePath)
        {
            var target = new StrykerOptionsValidator();
            var options = new StrykerOptions(basePath, null, null, null, null);

            Assert.Throws<ValidationException>(() => target.Validate(options));
        }

        [Theory]
        [InlineData("SomeNonexistingReporter")]
        [InlineData("Wrong")]
        public void OptionsValidator_ShouldThrowValidationExceptionOnFalseBuildConfigValues(string fakeReporter)
        {
            var target = new StrykerOptionsValidator();
            var options = new StrykerOptions("C:/ExampleProject/", fakeReporter, null);

            Assert.Throws<ValidationException>(() => target.Validate(options));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Console")]
        [InlineData("RapportOnly")]
        public void OptionsValidator_ShouldAcceptReporterValues(string reporter)
        {
            var target = new StrykerOptionsValidator();
            var options = new StrykerOptions("C:/ExampleProject/", reporter, null);

            var result = target.Validate(options);

            // Should default to Console
            result.Reporter.ShouldBe(string.IsNullOrWhiteSpace(reporter) ? "Console" : reporter);
        }
    }
}
