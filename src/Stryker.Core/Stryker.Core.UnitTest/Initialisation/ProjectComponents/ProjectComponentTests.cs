using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.ProjectComponents;
using System.Collections.ObjectModel;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents
{
    public class ProjectComponentTests : TestBase
    {
        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_NoMutations()
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { } });

            var result = target.GetMutationScore();
            result.ShouldBe(double.NaN);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_OneMutation()
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed } } });

            var result = target.GetMutationScore();
            result.ShouldBe(1);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_TwoFolders()
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed } } });
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });

            var result = target.GetMutationScore();
            result.ShouldBe(0.5);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_Recursive()
        {
            var target = new CsharpFolderComposite();
            var subFolder = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed } } });
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });
            target.Add(subFolder);
            subFolder.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived }, new Mutant() { ResultStatus = MutantStatus.Killed } } });

            var result = target.GetMutationScore();
            result.ShouldBe(0.5);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_Recursive2()
        {
            var target = new CsharpFolderComposite();
            var subFolder = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });
            target.Add(subFolder);
            subFolder.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived }, new Mutant() { ResultStatus = MutantStatus.Killed } } });
            subFolder.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed }, new Mutant() { ResultStatus = MutantStatus.Killed } } });
            subFolder.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived }, new Mutant() { ResultStatus = MutantStatus.Killed } } });
            subFolder.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed }, new Mutant() { ResultStatus = MutantStatus.Killed } } });
            subFolder.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed }, new Mutant() { ResultStatus = MutantStatus.Killed } } });

            var result = target.GetMutationScore();
            result.ShouldBe(0.6666666666666666666666666666666666667);
        }

        [Theory]
        [InlineData(MutantStatus.Killed, 1)]
        [InlineData(MutantStatus.Timeout, 1)]
        [InlineData(MutantStatus.Survived, 0)]
        [InlineData(MutantStatus.NotRun, 0)]
        public void ReportComponent_ShouldCalculateMutationScore_OnlyKilledIsSuccessful(MutantStatus status, double expectedScore)
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = status } } });

            var result = target.GetMutationScore();
            result.ShouldBe(expectedScore);
        }

        [Fact]
        public void ReportComponent_ShouldCalculateMutationScore_BuildErrorIsNull()
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.CompileError } } });

            var result = target.GetMutationScore();
            result.ShouldBe(double.NaN);
        }
    }
}
