using Shouldly;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.ObjectModel;
using Xunit;

namespace Stryker.Core.UnitTest.ProjectComponents
{
    public class ProjectComponentsTests
    {
        [Fact]
        public void ShouldGet100MutationScore()
        {
            var file = new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Killed },
                }
            };

            file.GetMutationScore().ShouldBe(1);
            file.CheckHealth(new Thresholds(high: 100, low: 50, @break: 0)).ShouldBe(Health.Good);
        }

        [Fact]
        public void ShouldGet0MutationScore()
        {
            var file = new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Survived },
                }
            };

            file.GetMutationScore().ShouldBe(0);
            file.CheckHealth(new Thresholds(high: 80, low: 1, @break: 0)).ShouldBe(Health.Danger);
            file.CheckHealth(new Thresholds(high: 80, low: 0, @break: 0)).ShouldBe(Health.Warning);
        }

        [Fact]
        public void ShouldGet50MutationScore()
        {
            var file = new CsharpFileLeaf()
            {
                RelativePath = "RootFolder/SomeFile.cs",
                FullPath = "C://RootFolder/SomeFile.cs",
                Mutants = new Collection<Mutant>()
                {
                    new Mutant() { ResultStatus = MutantStatus.Survived },
                    new Mutant() { ResultStatus = MutantStatus.Killed },
                }
            };

            file.GetMutationScore().ShouldBe(0.5);
            file.CheckHealth(new Thresholds(high: 80, low: 60, @break: 0)).ShouldBe(Health.Danger);
            file.CheckHealth(new Thresholds(high: 80, low: 50, @break: 0)).ShouldBe(Health.Warning);
            file.CheckHealth(new Thresholds(high: 50, low: 49, @break: 0)).ShouldBe(Health.Good);
        }
    }
}
