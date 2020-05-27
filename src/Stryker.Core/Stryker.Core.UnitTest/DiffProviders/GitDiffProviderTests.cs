using LibGit2Sharp;
using Moq;
using Shouldly;
using Stryker.Core.DiffProviders;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public void ScanDiffReturnsListofFiles()
        {
            // Arrange
            var options = new StrykerOptions(gitSource: "d670460b4b4aece5915caf5c68d12f560a9fe3e4");

            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
            var commitMock = new Mock<Commit>(MockBehavior.Loose);
            var branchMock = new Mock<Branch>(MockBehavior.Strict);
            var patchMock = new Mock<Patch>(MockBehavior.Strict);
            var patchEntryChangesMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);

            commitMock.SetupGet(x => x.Tree).Returns(new Mock<Tree>(MockBehavior.Loose).Object);
            branchMock.SetupGet(x => x.Tip).Returns(commitMock.Object);
            repositoryMock.Setup(x => x.Branches["d670460b4b4aece5915caf5c68d12f560a9fe3e4"]).Returns(branchMock.Object).Verifiable();
            patchEntryChangesMock.SetupGet(x => x.Path).Returns("file.cs");
            patchMock.Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges> { patchEntryChangesMock.Object }).GetEnumerator());
            repositoryMock.Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.Index | DiffTargets.WorkingDirectory)).Returns(patchMock.Object);

            var target = new GitDiffProvider(options, repositoryMock.Object);

            // Act
            var res = target.ScanDiff();

            // Assert
            res.ChangedFiles.Count().ShouldBe(1);
            res.TestsChanged.ShouldBeTrue();
        }
    }
}
