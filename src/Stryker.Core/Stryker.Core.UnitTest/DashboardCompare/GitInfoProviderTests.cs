﻿using LibGit2Sharp;
using Moq;
using Shouldly;
using Stryker.Core.DashboardCompare;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace Stryker.Core.UnitTest.DashboardCompare
{

    public class GitInfoProviderTests
    {

        [Fact]
        public void WhenProvidedReturnsRepositoryPath()
        {
            var repository = new Mock<IRepository>(MockBehavior.Strict);

            var target = new GitInfoProvider(null, repository.Object, "path");

            target.RepositoryPath.ShouldBe("path");
        }

        [Fact]
        public void DoesNotCreateNewRepositoryWhenPassedIntoConstructor()
        {
            var options = new StrykerOptions(basePath: "C:\\");

            var repository = new Mock<IRepository>(MockBehavior.Strict);

            var branchCollectionMock = new Mock<BranchCollection>(MockBehavior.Strict);

            branchCollectionMock.Setup(x => x.Add(It.IsAny<string>(), It.IsAny<string>())).Returns(new Mock<Branch>(MockBehavior.Loose).Object);

            repository.SetupGet(x => x.Branches).Returns(branchCollectionMock.Object);

            Action act = () => new GitInfoProvider(options, repository.Object);

            act.ShouldNotThrow();
        }

        [Fact]
        public void ReturnsEmptyStringIfNoCurrentRepositoryHead()
        {
            // Arrange
            var options = new StrykerOptions();
            var repository = new Mock<IRepository>(MockBehavior.Loose);

            var target = new GitInfoProvider(options, repository.Object);
            // Act
            var result = target.GetCurrentBranchName();

            // Assert
            result.ShouldBe(string.Empty);
        }

        [Fact]
        public void ReturnsCurrentBranch()
        {
            // Arrange
            var options = new StrykerOptions();
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
            var options = new StrykerOptions();
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
            void act() => new GitInfoProvider(null, repositoryPath: string.Empty);

            Should.Throw<StrykerInputException>(act)
                .Message.ShouldBe("Could not locate git repository. Unable to determine git diff to filter mutants. Did you run inside a git repo? If not please disable the --diff feature.");
            
        }


        [Fact]
        public void DetermineCommitThrowsStrykerInputException()
        {
            var strykerOptions = new StrykerOptions(gitSource: "master");

            var repository = new Mock<IRepository>();

            var branchCollectionMock = new Mock<BranchCollection>();

            branchCollectionMock
               .Setup(x => x.GetEnumerator()).Returns(
                ((IEnumerable<Branch>)new List<Branch>()).GetEnumerator());


            repository.SetupGet(x => x.Branches).Returns(branchCollectionMock.Object);

            var target = new GitInfoProvider(strykerOptions, repository.Object);


            void act() => target.DetermineCommit();

            Should.Throw<StrykerInputException>(act);
        }
    }
}
