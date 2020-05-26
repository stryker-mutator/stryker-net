using LibGit2Sharp;
using Moq;
using Shouldly;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.DashboardCompare
{
    public class GitBranchProviderTests
    {
        [Fact]
        public void WhenRepositoryPathNullThorwsStrykerInputException()
        {
            var options = new StrykerOptions(basePath: "C:\\");

            Should.Throw<StrykerInputException>(() => new GitBranchProvider(options))
                .Message
                .ShouldBe("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
        }

        [Fact]
        public void DoesNotCreateNewRepositoryWhenPassedIntoConstructor()
        {
            var options = new StrykerOptions(basePath: "C:\\");

            var repository = new Mock<IRepository>(MockBehavior.Strict);

            Action act = () => new GitBranchProvider(options, repository.Object);

            act.ShouldNotThrow();

            repository.VerifyNoOtherCalls();
        }

        [Fact]
        public void DisposingAlsoDisposesRepository()
        {
            // Arrange
            var options = new StrykerOptions();
            var repository = new Mock<IRepository>(MockBehavior.Strict);

            repository.Setup(x => x.Dispose()).Verifiable();

            var target = new GitBranchProvider(options, repository.Object);

            // Act
            target.Dispose();

            // Assert
            repository.Verify(x => x.Dispose(), Times.Once);
        }

        [Fact]
        public void ReturnsEmptyStringIfNoCurrentRepositoryHead()
        {
            // Arrange
            var options = new StrykerOptions();
            var repository = new Mock<IRepository>(MockBehavior.Loose);

            var target = new GitBranchProvider(options, repository.Object);
            // Act
            var result = target.GetCurrentBranchCanonicalName();

            // Assert
            result.ShouldBe(string.Empty);
        }
    }
}
