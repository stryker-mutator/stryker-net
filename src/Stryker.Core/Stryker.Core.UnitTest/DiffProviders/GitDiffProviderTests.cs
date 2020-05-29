using LibGit2Sharp;
using Moq;
using Shouldly;
using Stryker.Core.DiffProviders;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
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


        /**
         * Libgit2sharp has most of its contructors sealed. 
         * Because of that we are unable to make the repository mock return a explicit object and are only able to use mocks.
        **/
        [Fact]
        public void ScanDiffReturnsListofFiles()
        {
            // Arrange
            var basePath = FilePathUtils.NormalizePathSeparators("C://Users/JohnDoe/Project/Tests");
            var options = new StrykerOptions(gitSource: "d670460b4b4aece5915caf5c68d12f560a9fe3e4", basePath: basePath, fileSystem: new MockFileSystem());

            var repositoryMock = new Mock<IRepository>(MockBehavior.Loose);
            var commitMock = new Mock<Commit>(MockBehavior.Loose);
            var branchMock = new Mock<Branch>(MockBehavior.Strict);
            var patchMock = new Mock<Patch>(MockBehavior.Strict);
            var patchEntryChangesMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);

            // Setup of mocks
            commitMock
                .SetupGet(x => x.Tree)
                .Returns(new Mock<Tree>(MockBehavior.Loose).Object);

            branchMock
                .SetupGet(x => x.Tip)
                .Returns(commitMock.Object);

            branchMock.SetupGet(x => x.CanonicalName).Returns("refs/heads/branch");

            branchMock.SetupGet(x => x.FriendlyName).Returns("branch");

            repositoryMock
                .Setup(x => x.Branches.GetEnumerator())
                .Returns(new List<Branch> { branchMock.Object }.GetEnumerator())
                .Verifiable();

            patchEntryChangesMock
                .SetupGet(x => x.Path)
                .Returns("file.cs");

            patchMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges> { patchEntryChangesMock.Object }).GetEnumerator());

            repositoryMock
                .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.Index | DiffTargets.WorkingDirectory))
                .Returns(patchMock.Object);

            repositoryMock
                .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

            var target = new GitDiffProvider(options, repositoryMock.Object, repositoryPath: "C://JohnDoe/Project");

            // Act
            var res = target.ScanDiff();

            // Assert
            res.ChangedFiles.Count().ShouldBe(1);
            res.TestsChanged.ShouldBeFalse();
        }
    }
}
