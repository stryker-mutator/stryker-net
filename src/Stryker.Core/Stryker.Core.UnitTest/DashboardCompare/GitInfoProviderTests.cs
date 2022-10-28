using System;
using System.Collections.Generic;
using LibGit2Sharp;
using Moq;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.DashboardCompare
{
    using Microsoft.Extensions.Logging;
    using Stryker.Core.Baseline.Providers;

    public class GitInfoProviderTests : TestBase
    {
        [Fact]
        public void WhenProvidedReturnsRepositoryPath()
        {
            var repository = new Mock<IRepository>(MockBehavior.Strict);

            var options = new StrykerOptions()
            {
                Since = true,
            };
            var target = new GitInfoProvider(options, repository.Object, "path", Mock.Of<ILogger<GitInfoProvider>>());

            target.RepositoryPath.ShouldBe("path");
        }

        [Fact]
        public void DoesNotCheckForRepositoryPathWhenDisabled()
        {
            var repository = new Mock<IRepository>(MockBehavior.Strict);

            var options = new StrykerOptions()
            {
                Since = false,
            };
            var target = new GitInfoProvider(options, repository.Object, null);

            target.Repository.ShouldBe(null);
        }

        [Fact]
        public void DoesNotCreateNewRepositoryWhenPassedIntoConstructor()
        {
            var options = new StrykerOptions()
            {
                ProjectPath = "C:\\",
            };

            var repository = new Mock<IRepository>(MockBehavior.Strict);

            var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);

            branchCollectionMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>())).Returns(new Mock<Branch>(MockBehavior.Loose).Object);

            repository.SetupGet(x => x.Branches).Returns(branchCollectionMock.Object);

            Action act = () => new GitInfoProvider(options, repository.Object);

            act.ShouldNotThrow();
        }

        [Fact]
        public void ThrowsExceptionIfNoCurrentBranchOrProjectVersionSet()
        {
            // Arrange
            var options = new StrykerOptions();
            var repository = new Mock<IRepository>(MockBehavior.Loose);

            var target = new GitInfoProvider(options, repository.Object);
            // Act
            Action result = () => target.GetCurrentBranchName();

            // Assert
            result.ShouldThrow<InputException>();
        }

        [Fact]
        public void ReturnsCurrentBranch()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                Since = true,
            };
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);

            var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);
            var branchMock = new Mock<Branch>();

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

            repositoryMock
                .SetupGet(x => x.Branches)
                .Returns(branchCollectionMock.Object);

            var target = new GitInfoProvider(options, repositoryMock.Object);

            // Act
            var res = target.GetCurrentBranchName();

            // Assert
            res.ShouldBe("master");

            repositoryMock.Verify();
        }

        [Fact]
        public void ReturnsCurrentBranchWhenMultipleBranches()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                Since = true,
            };
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);

            var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);
            var branchMock = new Mock<Branch>();
            var branchMock2 = new Mock<Branch>();

            branchCollectionMock
                .Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new Mock<Branch>(MockBehavior.Loose).Object);

            branchMock
                .SetupGet(x => x.IsCurrentRepositoryHead)
                .Returns(true);

            branchMock2
             .SetupGet(x => x.IsCurrentRepositoryHead)
             .Returns(false);

            branchMock
                .SetupGet(x => x.FriendlyName)
                .Returns("master");

            branchMock2
             .SetupGet(x => x.FriendlyName)
             .Returns("dev");

            branchCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<Branch>)new List<Branch>
                {
                 branchMock.Object
                }).GetEnumerator());

            repositoryMock
                .SetupGet(x => x.Branches)
                .Returns(branchCollectionMock.Object);

            var target = new GitInfoProvider(options, repositoryMock.Object);

            // Act
            var res = target.GetCurrentBranchName();

            // Assert
            res.ShouldBe("master");

            repositoryMock.Verify();
        }

        [Fact]
        public void CreateRepository_Throws_InputException_When_RepositoryPath_Empty()
        {
            static void act() => new GitInfoProvider(new StrykerOptions()
            {
                Since = true,
            }, repositoryPath: string.Empty);

            Should.Throw<InputException>(act)
                .Message.ShouldBe("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the 'since' feature.");
        }

        [Fact]
        public void DetermineCommitThrowsStrykerInputException()
        {
            var options = new StrykerOptions()
            {
                Since = true,
                SinceTarget = "main"
            };
            var repository = new Mock<IRepository>();

            var branchCollectionMock = new Mock<BranchCollection>();

            branchCollectionMock
               .Setup(x => x.GetEnumerator()).Returns(
                ((IEnumerable<Branch>)new List<Branch>()).GetEnumerator());


            repository.SetupGet(x => x.Branches).Returns(branchCollectionMock.Object);

            var tagCollectionMock = new Mock<TagCollection>();

            tagCollectionMock
                .Setup(x => x.GetEnumerator()).Returns(
                    ((IEnumerable<Tag>)new List<Tag>()).GetEnumerator());

            repository.SetupGet(x => x.Tags).Returns(tagCollectionMock.Object);

            var target = new GitInfoProvider(options, repository.Object);


            void act() => target.DetermineCommit();

            Should.Throw<InputException>(act);
        }

        [Fact]
        public void LooksUpCommitWhenGitSourceIsFortyCharacters()
        {
            // Arrange
            string sha = "5a6940131b31f6958007ecbc0c51cbc35177f4e0";
            var options = new StrykerOptions()
            {
                Since = true,
                SinceTarget = sha
            };
            var commitMock = new Mock<Commit>();
            var repositoryMock = new Mock<IRepository>();
            var branchCollectionMock = new Mock<BranchCollection>();

            branchCollectionMock
               .Setup(x => x.GetEnumerator()).Returns(
                ((IEnumerable<Branch>)new List<Branch>()).GetEnumerator());

            var tagCollectionMock = new Mock<TagCollection>();

            tagCollectionMock
               .Setup(x => x.GetEnumerator()).Returns(
                ((IEnumerable<Tag>)new List<Tag>()).GetEnumerator());

            repositoryMock.SetupGet(x => x.Branches).Returns(branchCollectionMock.Object);
            repositoryMock.SetupGet(x => x.Tags).Returns(tagCollectionMock.Object);
            repositoryMock.Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

            var target = new GitInfoProvider(options, repositoryMock.Object);

            // Act
            Commit result = target.DetermineCommit();

            // Assert
            result.ShouldNotBeNull();
            repositoryMock.Verify(x => x.Lookup(It.Is<ObjectId>(x => x.Sha == sha)), Times.Once);
        }

        [Fact]
        public void ReturnsTip_When_Canonical_Name_Is_GitSource()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                Since = true,
                SinceTarget = "origin/master"
            };
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);

            var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);
            var branchMock = new Mock<Branch>();
            var commitMock = new Mock<Commit>();

            branchMock
                .SetupGet(x => x.FriendlyName)
                .Returns("master");

            branchMock
                .SetupGet(x => x.CanonicalName)
                .Returns("origin/master");

            branchMock
                .SetupGet(x => x.UpstreamBranchCanonicalName)
                .Returns("refs/heads/master");

            branchMock.SetupGet(x => x.Tip).Returns(commitMock.Object);

            branchCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<Branch>)new List<Branch>
                {
                 branchMock.Object
                }).GetEnumerator());

            repositoryMock
                .SetupGet(x => x.Branches)
                .Returns(branchCollectionMock.Object);

            var target = new GitInfoProvider(options, repositoryMock.Object);

            // Act
            var res = target.DetermineCommit();

            // Assert
            res.ShouldNotBeNull();
            res.ShouldBe(commitMock.Object);

            repositoryMock.Verify();
        }

        [Fact]
        public void GetTargetCommit_Does_Not_Throw_NullReferenceException_When_Branch_Is_Null()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                Since = true,
                SinceTarget = "origin/master"
            };
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);

            var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);
            var branchMock = new Mock<Branch>();

            branchCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<Branch>)new List<Branch>
                {
                 branchMock.Object
                }).GetEnumerator());

            repositoryMock
                .SetupGet(x => x.Branches)
                .Returns(branchCollectionMock.Object);

            var tagCollectionMock = new Mock<TagCollection>(MockBehavior.Strict);
            var tagMock = new Mock<Tag>();

            tagCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<Tag>)new List<Tag>
                {
                 tagMock.Object
                }).GetEnumerator());

            repositoryMock
                .SetupGet(x => x.Tags)
                .Returns(tagCollectionMock.Object);

            var target = new GitInfoProvider(options, repositoryMock.Object);

            // Act
            void act() => target.DetermineCommit();

            // Assert
            Should.Throw<InputException>(act);
        }

        [Fact]
        public void ReturnsTip_When_Friendly_Name_Is_GitSource()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                Since = true,
                SinceTarget = "master"
            };
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);

            var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);
            var branchMock = new Mock<Branch>();
            var commitMock = new Mock<Commit>();

            branchMock
                .SetupGet(x => x.FriendlyName)
                .Returns("master");

            branchMock
                .SetupGet(x => x.CanonicalName)
                .Returns("origin/master");

            branchMock
                .SetupGet(x => x.UpstreamBranchCanonicalName)
                .Returns("refs/heads/master");

            branchMock.SetupGet(x => x.Tip).Returns(commitMock.Object);

            branchCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<Branch>)new List<Branch>
                {
                 branchMock.Object
                }).GetEnumerator());

            repositoryMock
                .SetupGet(x => x.Branches)
                .Returns(branchCollectionMock.Object);

            var target = new GitInfoProvider(options, repositoryMock.Object);

            // Act
            var res = target.DetermineCommit();

            // Assert
            res.ShouldNotBeNull();
            res.ShouldBe(commitMock.Object);

            repositoryMock.Verify();
        }

        [Fact]
        public void ReturnsTip_When_Upstream_Branch_Canonical_Name_Is_GitSource()
        {
            // Arrange
            var options = new StrykerOptions()
            {
                Since = true,
                SinceTarget = "refs/heads/master"
            };
            var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);

            var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);
            var branchMock = new Mock<Branch>();
            var commitMock = new Mock<Commit>();

            branchMock
                .SetupGet(x => x.FriendlyName)
                .Returns("master");

            branchMock
                .SetupGet(x => x.CanonicalName)
                .Returns("origin/master");

            branchMock
                .SetupGet(x => x.UpstreamBranchCanonicalName)
                .Returns("refs/heads/master");

            branchMock.SetupGet(x => x.Tip).Returns(commitMock.Object);

            branchCollectionMock
                .Setup(x => x.GetEnumerator())
                .Returns(((IEnumerable<Branch>)new List<Branch>
                {
                 branchMock.Object
                }).GetEnumerator());

            repositoryMock
                .SetupGet(x => x.Branches)
                .Returns(branchCollectionMock.Object);

            var target = new GitInfoProvider(options, repositoryMock.Object);

            // Act
            var res = target.DetermineCommit();

            // Assert
            res.ShouldNotBeNull();
            res.ShouldBe(commitMock.Object);

            repositoryMock.Verify();
        }
    }
}
