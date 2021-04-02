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

            var thresholds = new Thresholds
            {
                High = 100,
                Low = 50,
                Break = 0
            };

            file.GetMutationScore().ShouldBe(1);
            file.CheckHealth(thresholds).ShouldBe(Health.Good);
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

            var thresholdsDanger = new Thresholds
            {
                High = 80,
                Low = 1,
                Break = 0
            };
            file.CheckHealth(thresholdsDanger).ShouldBe(Health.Danger);
            var thresholdsWarning = new Thresholds
            {
                High = 80,
                Low = 0,
                Break = 0
            };
            file.CheckHealth(thresholdsWarning).ShouldBe(Health.Warning);
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

            var thresholdsDanger = new Thresholds
            {
                High = 80,
                Low = 60,
                Break = 0
            };
            file.CheckHealth(thresholdsDanger).ShouldBe(Health.Danger);
            var thresholdsWarning = new Thresholds
            {
                High = 80,
                Low = 50,
                Break = 0
            };
            file.CheckHealth(thresholdsWarning).ShouldBe(Health.Warning);
            var thresholdsGood = new Thresholds
            {
                High = 50,
                Low = 49,
                Break = 0
            };
            file.CheckHealth(thresholdsGood).ShouldBe(Health.Good);
        }
    }
}
