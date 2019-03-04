using Shouldly;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.Mutants;
using System.Collections.ObjectModel;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation.ProjectComponents
{
    public class ProjectComponentTests
    {
        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_NoMutations()
        {
            var target = new FolderComposite() { Name = "RootFolder" };
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { } });

            var result = target.GetMutationScore();
            result.ShouldBe(null);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_OneMutation()
        {
            var target = new FolderComposite() { Name = "RootFolder" };
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed } } });

            var result = target.GetMutationScore();
            result.ShouldBe(100M);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_TwoFolders()
        {
            var target = new FolderComposite() { Name = "RootFolder" };
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed } } });
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });

            var result = target.GetMutationScore();
            result.ShouldBe(50M);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_Recursive()
        {
            var target = new FolderComposite() { Name = "RootFolder" };
            var subFolder = new FolderComposite() { Name = "SubFolder" };
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed } } });
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });
            target.Add(subFolder);
            subFolder.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived }, new Mutant() { ResultStatus = MutantStatus.Killed } } });

            var result = target.GetMutationScore();
            result.ShouldBe(50M);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_Recursive2()
        {
            var target = new FolderComposite() { Name = "RootFolder" };
            var subFolder = new FolderComposite() { Name = "SubFolder" };
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });
            target.Add(subFolder);
            subFolder.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived }, new Mutant() { ResultStatus = MutantStatus.Killed } } });
            subFolder.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed }, new Mutant() { ResultStatus = MutantStatus.Killed } } });
            subFolder.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived }, new Mutant() { ResultStatus = MutantStatus.Killed } } });
            subFolder.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed }, new Mutant() { ResultStatus = MutantStatus.Killed } } });
            subFolder.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed }, new Mutant() { ResultStatus = MutantStatus.Killed } } });

            var result = target.GetMutationScore();
            result.ShouldBe(66.66666666666666666666666667M);
        }

        [Theory]
        [InlineData(MutantStatus.Killed, 100)]
        [InlineData(MutantStatus.Timeout, 100)]
        [InlineData(MutantStatus.Survived, 0)]
        [InlineData(MutantStatus.NotRun, 0)]
        public void ReportComponent_ShouldCalculateMutationScore_OnlyKilledIsSuccessful(MutantStatus status, decimal expectedScore)
        {
            var target = new FolderComposite() { Name = "RootFolder" };
            var subFolder = new FolderComposite() { Name = "SubFolder" };
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = status } } });

            var result = target.GetMutationScore();
            result.ShouldBe(expectedScore);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_BuildErrorIsNull()
        {
            var target = new FolderComposite() { Name = "RootFolder" };
            var subFolder = new FolderComposite() { Name = "SubFolder" };
            target.Add(new FileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.CompileError } } });

            var result = target.GetMutationScore();
            result.ShouldBe(null);
        }
    }
}
