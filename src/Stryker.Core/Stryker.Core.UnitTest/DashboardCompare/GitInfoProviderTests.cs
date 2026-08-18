using System;
using System.Collections.Generic;
using LibGit2Sharp;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions;
using Stryker.Configuration.Options;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.UnitTest;

namespace Stryker.Core.UnitTest.DashboardCompare;


[TestClass]
public class GitInfoProviderTests : TestBase
{
    [TestMethod]
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

    [TestMethod]
    public void UsesProvidedRepositoryWhenSinceIsDisabled()
    {
        var repository = new Mock<IRepository>(MockBehavior.Strict);

        var options = new StrykerOptions()
        {
            Since = false,
        };
        var target = new GitInfoProvider(options, repository.Object, null);

        target.Repository.ShouldBe(repository.Object);
    }

    [TestMethod]
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

    [TestMethod]
    public void ReturnsNullIfNoCurrentBranchIsAvailable()
    {
        // Arrange
        var options = new StrykerOptions();
        var repository = new Mock<IRepository>(MockBehavior.Loose);

        var target = new GitInfoProvider(options, repository.Object);
        // Act
        var result = target.GetCurrentBranchName();

        // Assert
        result.ShouldBeNull();
    }

    [TestMethod]
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

    [TestMethod]
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

    [TestMethod]
    public void DoesNotCreateRepository_When_RepositoryPath_Empty()
    {
        var target = new GitInfoProvider(new StrykerOptions()
        {
            Since = true,
        }, repositoryPath: string.Empty);

        target.IsRepository.ShouldBeFalse();
        target.Repository.ShouldBeNull();
    }

    [TestMethod]
    public void DetermineCommitReturnsNullWhenTargetCannotBeResolved()
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


        var result = target.DetermineCommit("main");

        result.ShouldBeNull();
    }

    [TestMethod]
    public void LooksUpCommitWhenGitSourceIsFortyCharacters()
    {
        // Arrange
        var sha = "5a6940131b31f6958007ecbc0c51cbc35177f4e0";
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
        var result = target.DetermineCommit(sha);

        // Assert
        result.ShouldNotBeNull();
        repositoryMock.Verify(x => x.Lookup(It.Is<ObjectId>(x => x.Sha == sha)), Times.Once);
    }

    [TestMethod]
    public void ReturnsTargetCommit_When_TagNameMatchesTarget()
    {
        var options = new StrykerOptions();
        var commitMock = new Mock<Commit>();
        var tagMock = new Mock<Tag>(MockBehavior.Strict);
        var repositoryMock = new Mock<IRepository>(MockBehavior.Strict);
        var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);
        var tagCollectionMock = new Mock<TagCollection>(MockBehavior.Strict);

        branchCollectionMock
            .Setup(x => x.GetEnumerator())
            .Returns(((IEnumerable<Branch>)new List<Branch>()).GetEnumerator());

        tagMock
            .SetupGet(x => x.CanonicalName)
            .Returns("refs/tags/v1.0.0");
        tagMock
            .SetupGet(x => x.Target)
            .Returns(commitMock.Object);

        tagCollectionMock
            .Setup(x => x.GetEnumerator())
            .Returns(((IEnumerable<Tag>)new List<Tag> { tagMock.Object }).GetEnumerator());

        repositoryMock.SetupGet(x => x.Branches).Returns(branchCollectionMock.Object);
        repositoryMock.SetupGet(x => x.Tags).Returns(tagCollectionMock.Object);

        var target = new GitInfoProvider(options, repositoryMock.Object);

        var result = target.DetermineCommit("v1.0.0");

        result.ShouldBe(commitMock.Object);
    }

    [TestMethod]
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
        var res = target.DetermineCommit("origin/master");

        // Assert
        res.ShouldNotBeNull();
        res.ShouldBe(commitMock.Object);

        repositoryMock.Verify();
    }

    [TestMethod]
    public void DetermineCommitReturnsNullWhenBranchPropertiesAreUnavailable()
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
        var result = target.DetermineCommit("origin/master");

        // Assert
        result.ShouldBeNull();
    }

    [TestMethod]
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
        var res = target.DetermineCommit("master");

        // Assert
        res.ShouldNotBeNull();
        res.ShouldBe(commitMock.Object);

        repositoryMock.Verify();
    }

    [TestMethod]
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
        var res = target.DetermineCommit("refs/heads/master");

        // Assert
        res.ShouldNotBeNull();
        res.ShouldBe(commitMock.Object);

        repositoryMock.Verify();
    }
}
