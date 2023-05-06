using System;
using System.Collections.Generic;
using System.Linq;
using DotNet.Globbing;
using LibGit2Sharp;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Providers;
using Stryker.Core.DiffProviders;
using Stryker.Core.Exceptions;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.DiffProviders;

public class GitDiffProviderTests : TestBase
{
    [Fact]
    public void DoesNotCreateNewRepositoryWhenPassedIntoConstructor()
    {
        var options = new StrykerOptions()
        {
            ProjectPath = "C:\\"
        };

        var gitInfoProvider = new Mock<IGitInfoProvider>(MockBehavior.Strict);

        Action act = () => new GitDiffProvider(options, null, gitInfoProvider.Object);

        act.ShouldNotThrow();

        gitInfoProvider.VerifyNoOtherCalls();
    }


    /**
     * Libgit2sharp has most of its constructors sealed.
     * Because of that we are unable to make the repository mock return a explicit object and are only able to use mocks.
    **/
    [Fact]
    public void ScanDiffReturnsListOfFiles()
    {
        // Arrange
        var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
        var options = new StrykerOptions()
        {
            ProjectPath = "C:\\",
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
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

        // Act
        var res = target.ScanDiff();

        // Assert
        res.ChangedSourceFiles.Count().ShouldBe(1);
        res.ChangedTestFiles.Count().ShouldBe(0);
    }

    [Fact]
    public void ScanDiffReturnsListOfFiles_IgnoreFolderWithSameStartName()
    {
        // Arrange
        var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
        var options = new StrykerOptions()
        {
            ProjectPath = "C:\\",
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
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

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
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

        Should.Throw<InputException>(() => target.ScanDiff());
    }

    [Fact]
    public void ScanDiffReturnsListOfFiles_ExcludingTestFilesInDiffIgnoreFiles()
    {
        // Arrange
        var diffIgnoreFiles = new[] { new FilePattern(Glob.Parse("/c/Users/JohnDoe/Project/Tests/Test.cs"), false, null) };

        var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
            DiffIgnoreChanges = diffIgnoreFiles
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
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

        // Act
        var res = target.ScanDiff();

        // Assert
        res.ChangedTestFiles.Count().ShouldBe(0);
        res.ChangedSourceFiles.Count().ShouldBe(1);
    }

    [Fact]
    public void ScanDiffReturnsListOfFiles_ExcludingTestFilesInDiffIgnoreFiles_Single_Asterisk()
    {
        // Arrange
        var diffIgnoreFiles = new[] { new FilePattern(Glob.Parse("/c/Users/JohnDoe/Project/*/Test.cs"), false, null) };

        var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
            DiffIgnoreChanges = diffIgnoreFiles
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
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

        // Act
        var res = target.ScanDiff();

        // Assert
        res.ChangedTestFiles.Count().ShouldBe(0);
        res.ChangedSourceFiles.Count().ShouldBe(1);
    }

    [Fact]
    public void ScanDiffReturnsListOfFiles_ExcludingTestFilesInDiffIgnoreFiles_Multi_Asterisk()
    {
        // Arrange
        var diffIgnoreFiles = new[] { new FilePattern(Glob.Parse("**/Test.cs"), false, null) };

        var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
            DiffIgnoreChanges = diffIgnoreFiles
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
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

        // Act
        var res = target.ScanDiff();

        // Assert
        res.ChangedTestFiles.Count().ShouldBe(0);
        res.ChangedSourceFiles.Count().ShouldBe(1);
    }

    [Fact]
    public void ScanDiffReturnsListOfFiles_ExcludingFilesInDiffIgnoreFiles_Multi_Asterisk()
    {
        // Arrange
        var diffIgnoreFiles = new[] { new FilePattern(Glob.Parse("**/file.cs"), false, null) };

        var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests");
        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
            DiffIgnoreChanges = diffIgnoreFiles
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
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

        // Act
        var res = target.ScanDiff();

        // Assert
        res.ChangedSourceFiles.Count().ShouldBe(0);
    }

    [Fact]
    public void ScanDiffReturnsListOfFiles_ShouldCorrectlyAssignTestAndSourceFiles()
    {
        // Arrange
        var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Source");
        var test1Path = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Tests1");
        var test2Path = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Tests2");
        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            TestProjects = new[] { test1Path, test2Path },
            SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
        };
        var gitInfoMock = new Mock<IGitInfoProvider>();
        var repositoryMock = new Mock<IRepository>(MockBehavior.Loose);
        var commitMock = new Mock<Commit>(MockBehavior.Loose);
        var branchMock = new Mock<Branch>(MockBehavior.Strict);
        var patchMock = new Mock<Patch>(MockBehavior.Strict);
        var patchEntrySourceFile1Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
        var patchEntrySourceFile2Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
        var patchEntrySourceFile3Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
        var patchEntryTestFile1Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
        var patchEntryTestFile2Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);

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

        patchEntrySourceFile1Mock
            .SetupGet(x => x.Path)
            .Returns("file.cs");
        patchEntrySourceFile2Mock
            .SetupGet(x => x.Path)
            .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Source/Category/Source1.cs"));
        patchEntrySourceFile3Mock
            .SetupGet(x => x.Path)
            .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/RootFile.cs"));

        patchEntryTestFile1Mock
            .SetupGet(x => x.Path)
            .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Tests1/Test.cs"));
        patchEntryTestFile2Mock
            .SetupGet(x => x.Path)
            .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Tests2/CategoryA/Test2.cs"));

        patchMock
            .Setup(x => x.GetEnumerator())
            .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges>
            {
                patchEntrySourceFile1Mock.Object,
                patchEntrySourceFile2Mock.Object,
                patchEntrySourceFile3Mock.Object,
                patchEntryTestFile1Mock.Object,
                patchEntryTestFile2Mock.Object
            }).GetEnumerator());

        repositoryMock
            .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.WorkingDirectory))
            .Returns(patchMock.Object);

        repositoryMock
            .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

        gitInfoMock.Setup(x => x.DetermineCommit()).Returns(commitMock.Object);

        gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
        gitInfoMock.SetupGet(x => x.RepositoryPath).Returns("/c/Path/To/Repo");
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

        // Act
        var res = target.ScanDiff();

        // Assert
        res.ChangedTestFiles.Count().ShouldBe(2);
        res.ChangedSourceFiles.Count().ShouldBe(3);
    }
    [Fact]
    public void ScanDiffReturnsListOfFiles_WithoutTestProjects_ShouldCorrectlyAssignTestAndSourceFiles()
    {
        // Arrange
        var basePath = FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Tests1");
        var options = new StrykerOptions()
        {
            ProjectPath = basePath,
            SinceTarget = "d670460b4b4aece5915caf5c68d12f560a9fe3e4",
        };
        var gitInfoMock = new Mock<IGitInfoProvider>();
        var repositoryMock = new Mock<IRepository>(MockBehavior.Loose);
        var commitMock = new Mock<Commit>(MockBehavior.Loose);
        var branchMock = new Mock<Branch>(MockBehavior.Strict);
        var patchMock = new Mock<Patch>(MockBehavior.Strict);
        var patchEntrySourceFile1Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
        var patchEntrySourceFile2Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
        var patchEntrySourceFile3Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
        var patchEntryTestFile1Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);
        var patchEntryTestFile2Mock = new Mock<PatchEntryChanges>(MockBehavior.Strict);

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

        patchEntrySourceFile1Mock
            .SetupGet(x => x.Path)
            .Returns("file.cs");
        patchEntrySourceFile2Mock
            .SetupGet(x => x.Path)
            .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Source/Category/Source1.cs"));
        patchEntrySourceFile3Mock
            .SetupGet(x => x.Path)
            .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/RootFile.cs"));

        patchEntryTestFile1Mock
            .SetupGet(x => x.Path)
            .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Tests1/Test.cs"));
        patchEntryTestFile2Mock
            .SetupGet(x => x.Path)
            .Returns(FilePathUtils.NormalizePathSeparators("/c/Users/JohnDoe/Project/Tests/Tests2/CategoryA/Test2.cs"));

        patchMock
            .Setup(x => x.GetEnumerator())
            .Returns(((IEnumerable<PatchEntryChanges>)new List<PatchEntryChanges>
            {
                patchEntrySourceFile1Mock.Object,
                patchEntrySourceFile2Mock.Object,
                patchEntrySourceFile3Mock.Object,
                patchEntryTestFile1Mock.Object,
                patchEntryTestFile2Mock.Object
            }).GetEnumerator());

        repositoryMock
            .Setup(x => x.Diff.Compare<Patch>(It.IsAny<Tree>(), DiffTargets.WorkingDirectory))
            .Returns(patchMock.Object);

        repositoryMock
            .Setup(x => x.Lookup(It.IsAny<ObjectId>())).Returns(commitMock.Object);

        gitInfoMock.Setup(x => x.DetermineCommit()).Returns(commitMock.Object);

        gitInfoMock.SetupGet(x => x.Repository).Returns(repositoryMock.Object);
        gitInfoMock.SetupGet(x => x.RepositoryPath).Returns("/c/Path/To/Repo");
        var target = new GitDiffProvider(options, null, gitInfoMock.Object);

        // Act
        var res = target.ScanDiff();

        // Assert
        res.ChangedTestFiles.Count().ShouldBe(1);
        res.ChangedSourceFiles.Count().ShouldBe(4);
    }
}
