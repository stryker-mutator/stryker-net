using LibGit2Sharp;
using Moq;
using Shouldly;
using Stryker.Core.DiffProviders;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.DiffProviders
{
    public class GitDiffProviderTests
    {
        [Fact]
        public void WhenRepositoryPathNullThrowsStrykerInputException()
        {
            var options = new StrykerOptions(basePath: "C:\\");

            Should.Throw<StrykerInputException>(() => new GitDiffProvider(options))
                .Message
                .ShouldBe("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
        }

        [Fact]
        public void DoesNotCreateNewRepositoryWhenPassedIntoConstructor()
        {
            var options = new StrykerOptions(basePath: "C:\\");

            var repository = new Mock<IRepository>(MockBehavior.Strict);

            Action act = () => new GitDiffProvider(options, repository.Object);

            act.ShouldNotThrow();

            repository.VerifyNoOtherCalls();
        }

        [Fact]
        public void DetermineCommitThrowsStrykerInputExceptionWhenBothSourceBranchAndCommitAreNull()
        {
            // Arrange
            var options = new StrykerOptions(gitSource: "d670460b4b4aece5915caf5c68d12f560a9fe3e4");

            var repository = new Mock<IRepository>(MockBehavior.Strict);

            repository.Setup(x => x.Branches["d670460b4b4aece5915caf5c68d12f560a9fe3e4"]).Returns((Branch)null).Verifiable();
            repository.Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns((GitObject)null).Verifiable();

            var target = new GitDiffProvider(options, repository.Object);

            // Act / Assert
            Should.Throw<StrykerInputException>(() => target.ScanDiff())
                .Message
                .ShouldBe("No Branch or commit found with given source d670460b4b4aece5915caf5c68d12f560a9fe3e4. Please provide a different --git-source or remove this option.");

            repository.Verify(x => x.Branches[It.Is<string>(o => o == "d670460b4b4aece5915caf5c68d12f560a9fe3e4")], Times.Once);
            repository.Verify(x => x.Lookup(It.Is<ObjectId>(o => o.Sha == "d670460b4b4aece5915caf5c68d12f560a9fe3e4")), Times.Once);
            repository.VerifyNoOtherCalls();

        }


    }
}
