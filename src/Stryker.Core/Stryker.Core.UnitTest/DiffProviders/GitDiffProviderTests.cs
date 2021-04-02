using DotNet.Globbing;
using LibGit2Sharp;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Shouldly;
using Stryker.Core.DashboardCompare;
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
        public void DoesNotCreateNewRepositoryWhenPassedIntoConstructor()
        {
            var options = new StrykerOptions()
            {
                BasePath = "C:\\"
            };

            var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Strict);

            Action act = () => new GitDiffProvider(options, gitInfoProvider.Object);

            act.ShouldNotThrow();

            gitInfoProvider.VerifyNoOtherCalls();
        }


        /**
         * Libgit2sharp has most of its contructors sealed. 
         * Because of that we are unable to make the repository mock return a explicit object and are only able to use mocks.
        **/
        [Fact]
        public void ScanDiffReturnsListofFiles()
        {
            // Arrange
            var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
            var options = new StrykerOptions()
            {
                BasePath = "C:\\",
                SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4"
            };

            var gitInfoMock = new Mock<IGitInfoProvider>();
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
                .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.WorkingDirectory))
                .Returns(patchMock.Object);

            repositoryMock
                .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

            gitInfoMock.Setup(x => x.DetermineCommit()).Returns(commitMock.Object);

            gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
            gitInfoMock.SetupGet(x => x.RepositoryPath).Returns("/c/Path/To/Repo");
            var target = new GitDiffProvider(options, gitInfoMock.Object);

            // Act
            var res = target.ScanDiff();

            // Assert
            res.ChangedSourceFiles.Count().ShouldBe(1);
            res.ChangedTestFiles.Count().ShouldBe(0);
        }

        [Fact]
        public void ScanDiffReturnsListofFiles_IgnoreFolderWithSameStartName()
        {
            // Arrange
            var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
            var options = new StrykerOptions()
            {
                BasePath = "C:\\",
                SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4"
            };
            var gitInfoMock = new Mock<IGitInfoProvider>();
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
                .Returns("/c/Users/JohnDoe/Project/Tests-temp/file.cs");

            patchMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges> { patchEntryChangesMock.Object }).GetEnumerator());

            repositoryMock
                .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.WorkingDirectory))
                .Returns(patchMock.Object);

            repositoryMock
                .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

            gitInfoMock.Setup(x => x.DetermineCommit()).Returns(commitMock.Object);

            gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
            gitInfoMock.SetupGet(x => x.RepositoryPath).Returns("/c/Path/To/Repo");
            var target = new GitDiffProvider(options, gitInfoMock.Object);

            // Act
            var res = target.ScanDiff();

            // Assert
            res.ChangedSourceFiles.Count().ShouldBe(1);
            res.ChangedTestFiles.Count().ShouldBe(0);
        }

        [Fact]
        public void ScanDiff_Throws_Stryker_Input_Exception_When_Commit_null()
        {
            var options = new StrykerOptions()
            {
                SinceTarget = "branch"
            };
            var repositoryMock = new Mock<IRepository>();
            var branchCollectionMock = new Mock<BranchCollection>();
            var branchMock = new Mock<Branch>();

            var commitMock = new Mock<Commit>();

            branchCollectionMock
              .Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>()))
              .Returns(new Mock<Branch>(MockBehavior.Loose).Object);

            branchMock
                .SetupGet(x => x.IsCurrentRepositoryHead)
                .Returns(true);

            branchMock
                .SetupGet(x => x.FriendlyName)
                .Returns("master");

            branchCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<Branch>)new List<Branch>
                {
                 branchMock.Object
                }).GetEnumerator());

            repositoryMock.SetupGet(x => x.Branches).Returns(branchCollectionMock.Object);

            var gitInfoMock = new Mock<IGitInfoProvider>();

            gitInfoMock.Setup(x => x.DetermineCommit()).Returns((Commit)null);
            gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
            var target = new GitDiffProvider(options, gitInfoMock.Object);

            Should.Throw<StrykerInputException>(() => target.ScanDiff());
        }

        [Fact]
        public void ScanDiffReturnsListofFiles_ExcludingTestFilesInDiffIgnoreFiles()
        {
            // Arrange
            var diffIgnoreFiles = new[] { new FilePattern(Glob.Parse("/c/Users/JohnDoe/Project/Tests/Test.cs"), false, null) };

            var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
            var options = new StrykerOptions()
            {
                BasePath = basePath,
                SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
                DiffIgnoreFilePatterns = diffIgnoreFiles
            };
            var gitInfoMock = new Mock<IGitInfoProvider>();
            var repositoryMock = new Mock<IRepository>(MockBehavior.Loose);
            var commitMock = new Mock<Commit>(MockBehavior.Loose);
            var branchMock = new Mock<Branch>(MockBehavior.Strict);
            var patchMock = new Mock<Patch>(MockBehavior.Strict);
            var patchEntryChangesMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
            var patchEntryChangesGitIgnoreMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);

            // Setup of mocks
            commitMock
                .SetupGet(x => x.Tree)
                .Returns(new Mock<Tree>(MockBehavior.Loose).Object);

            branchMock
                .SetupGet(x => x.Tip)
                .Returns(commitMock.Object);

            branchMock.SetupGet(x => x.UpstreamBranchCanonicalName).Returns("origin/branch");
            branchMock.SetupGet(x => x.CanonicalName).Returns("refs/heads/branch");
            branchMock.SetupGet(x => x.FriendlyName).Returns("branch");

            repositoryMock
                .Setup(x => x.Branches.GetEnumerator())
                .Returns(new List<Branch> { branchMock.Object }.GetEnumerator())
                .Verifiable();

            patchEntryChangesMock
                .SetupGet(x => x.Path)
                .Returns("file.cs");

            patchEntryChangesGitIgnoreMock
                .SetupGet(x => x.Path)
                .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Test.cs"));

            patchMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges>
                {
                    patchEntryChangesMock.Object,
                    patchEntryChangesGitIgnoreMock.Object
                }).GetEnumerator());

            repositoryMock
                .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.WorkingDirectory))
                .Returns(patchMock.Object);

            repositoryMock
                .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

            gitInfoMock.Setup(x => x.DetermineCommit()).Returns(commitMock.Object);

            gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
            gitInfoMock.SetupGet(x => x.RepositoryPath).Returns("/c/Path/To/Repo");
            var target = new GitDiffProvider(options, gitInfoMock.Object);

            // Act
            var res = target.ScanDiff();

            // Assert
            res.ChangedTestFiles.Count().ShouldBe(0);
            res.ChangedSourceFiles.Count().ShouldBe(1);
        }

        [Fact]
        public void ScanDiffReturnsListofFiles_ExcludingTestFilesInDiffIgnoreFiles_Single_Asterisk()
        {
            // Arrange
            var diffIgnoreFiles = new[] { new FilePattern(Glob.Parse("/c/Users/JohnDoe/Project/*/Test.cs"), false, null) };

            var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
            var options = new StrykerOptions()
            {
                BasePath = basePath,
                SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
                DiffIgnoreFilePatterns = diffIgnoreFiles
            };
            var gitInfoMock = new Mock<IGitInfoProvider>();
            var repositoryMock = new Mock<IRepository>(MockBehavior.Loose);
            var commitMock = new Mock<Commit>(MockBehavior.Loose);
            var branchMock = new Mock<Branch>(MockBehavior.Strict);
            var patchMock = new Mock<Patch>(MockBehavior.Strict);
            var patchEntryChangesMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
            var patchEntryChangesGitIgnoreMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);

            // Setup of mocks
            commitMock
                .SetupGet(x => x.Tree)
                .Returns(new Mock<Tree>(MockBehavior.Loose).Object);

            branchMock
                .SetupGet(x => x.Tip)
                .Returns(commitMock.Object);

            branchMock.SetupGet(x => x.UpstreamBranchCanonicalName).Returns("origin/branch");
            branchMock.SetupGet(x => x.CanonicalName).Returns("refs/heads/branch");
            branchMock.SetupGet(x => x.FriendlyName).Returns("branch");

            repositoryMock
                .Setup(x => x.Branches.GetEnumerator())
                .Returns(new List<Branch> { branchMock.Object }.GetEnumerator())
                .Verifiable();

            patchEntryChangesMock
                .SetupGet(x => x.Path)
                .Returns("file.cs");

            patchEntryChangesGitIgnoreMock
                .SetupGet(x => x.Path)
                .Returns(FilePathUtils.NormalizePathSeparators($"{basePath}/Test.cs"));

            patchMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges>
                {
                    patchEntryChangesMock.Object,
                    patchEntryChangesGitIgnoreMock.Object
                }).GetEnumerator());

            repositoryMock
                .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.WorkingDirectory))
                .Returns(patchMock.Object);

            repositoryMock
                .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

            gitInfoMock.Setup(x => x.DetermineCommit()).Returns(commitMock.Object);

            gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
            gitInfoMock.SetupGet(x => x.RepositoryPath).Returns(FilePathUtils.NormalizePathSeparators("/c/Path/To/Repo"));
            var target = new GitDiffProvider(options, gitInfoMock.Object);

            // Act
            var res = target.ScanDiff();

            // Assert
            res.ChangedTestFiles.Count().ShouldBe(0);
            res.ChangedSourceFiles.Count().ShouldBe(1);
        }

        [Fact]
        public void ScanDiffReturnsListofFiles_ExcludingTestFilesInDiffIgnoreFiles_Multi_Asterisk()
        {
            // Arrange
            var diffIgnoreFiles = new[] { new FilePattern(Glob.Parse("**/Test.cs"), false, null) };

            var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
            var options = new StrykerOptions()
            {
                BasePath = basePath,
                SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
                DiffIgnoreFilePatterns = diffIgnoreFiles
            };
            var gitInfoMock = new Mock<IGitInfoProvider>();
            var repositoryMock = new Mock<IRepository>(MockBehavior.Loose);
            var commitMock = new Mock<Commit>(MockBehavior.Loose);
            var branchMock = new Mock<Branch>(MockBehavior.Strict);
            var patchMock = new Mock<Patch>(MockBehavior.Strict);
            var patchEntryChangesMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
            var patchEntryChangesGitIgnoreMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);

            // Setup of mocks
            commitMock
                .SetupGet(x => x.Tree)
                .Returns(new Mock<Tree>(MockBehavior.Loose).Object);

            branchMock
                .SetupGet(x => x.Tip)
                .Returns(commitMock.Object);

            branchMock.SetupGet(x => x.UpstreamBranchCanonicalName).Returns("origin/branch");
            branchMock.SetupGet(x => x.CanonicalName).Returns("refs/heads/branch");
            branchMock.SetupGet(x => x.FriendlyName).Returns("branch");

            repositoryMock
                .Setup(x => x.Branches.GetEnumerator())
                .Returns(new List<Branch> { branchMock.Object }.GetEnumerator())
                .Verifiable();

            patchEntryChangesMock
                .SetupGet(x => x.Path)
                .Returns("file.cs");

            patchEntryChangesGitIgnoreMock
                .SetupGet(x => x.Path)
                .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Test.cs"));

            patchMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges>
                {
                    patchEntryChangesMock.Object,
                    patchEntryChangesGitIgnoreMock.Object
                }).GetEnumerator());

            repositoryMock
                .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.WorkingDirectory))
                .Returns(patchMock.Object);

            repositoryMock
                .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

            gitInfoMock.Setup(x => x.DetermineCommit()).Returns(commitMock.Object);

            gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
            gitInfoMock.SetupGet(x => x.RepositoryPath).Returns(FilePathUtils.NormalizePathSeparators("/c/Path/To/Repo"));
            var target = new GitDiffProvider(options, gitInfoMock.Object);

            // Act
            var res = target.ScanDiff();

            // Assert
            res.ChangedTestFiles.Count().ShouldBe(0);
            res.ChangedSourceFiles.Count().ShouldBe(1);
        }

        [Fact]
        public void ScanDiffReturnsListofFiles_ExcludingFilesInDiffIgnoreFiles_Multi_Asterisk()
        {
            // Arrange
            var diffIgnoreFiles = new[] { new FilePattern(Glob.Parse("**/file.cs"), false, null) };

            var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
            var options = new StrykerOptions()
            {
                BasePath = basePath,
                SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
                DiffIgnoreFilePatterns = diffIgnoreFiles
            };
            var gitInfoMock = new Mock<IGitInfoProvider>();
            var repositoryMock = new Mock<IRepository>(MockBehavior.Loose);
            var commitMock = new Mock<Commit>(MockBehavior.Loose);
            var branchMock = new Mock<Branch>(MockBehavior.Strict);
            var patchMock = new Mock<Patch>(MockBehavior.Strict);
            var patchEntryChangesMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
            var patchEntryChangesGitIgnoreMock = new Mock<PatchEntryChanges>(MockBehavior.Strict);

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
                .Returns("/c/Users/JohnDoe/Project/file.cs");

            patchMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges>
                                                              {
                                                                  patchEntryChangesMock.Object
                                                              }).GetEnumerator());

            repositoryMock
                .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.WorkingDirectory))
                .Returns(patchMock.Object);

            repositoryMock
                .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

            gitInfoMock.Setup(x => x.DetermineCommit()).Returns(commitMock.Object);

            gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
            gitInfoMock.SetupGet(x => x.RepositoryPath).Returns("/c/Path/To/Repo");
            var target = new GitDiffProvider(options, gitInfoMock.Object);

            // Act
            var res = target.ScanDiff();

            // Assert
            res.ChangedSourceFiles.Count().ShouldBe(0);
        }
    }
}
