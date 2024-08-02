using Shouldly;
using Stryker.Configuration.Mutants;
using Stryker.Configuration;
using Stryker.Configuration.ProjectComponents;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.ProjectComponents
{
    [TestClass]
    public class CsharpProjectComponentTests : TestBase
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void ReportComponent_ShouldCalculateMutationScoreNaN_NoMutations()
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { } });

            var result = target.GetMutationScore();
            result.ShouldBe(double.NaN);
        }

        [TestMethod]
        public void ReportComponent_ShouldCalculateMutationScore_OneMutation()
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed } } });

            var result = target.GetMutationScore();
            result.ShouldBe(1);
        }

        [TestMethod]
        public void ReportComponent_ShouldCalculateMutationScore_TwoFolders()
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Killed } } });
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Survived } } });

            var result = target.GetMutationScore();
            result.ShouldBe(0.5);
        }

        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        [DataRow(MutantStatus.Killed, 1)]
        [DataRow(MutantStatus.Timeout, 1)]
        [DataRow(MutantStatus.Survived, 0)]
        [DataRow(MutantStatus.Pending, double.NaN)]
        public void ReportComponent_ShouldCalculateMutationScore_OnlyKilledIsSuccessful(MutantStatus status, double expectedScore)
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = status } } });

            var result = target.GetMutationScore();
            result.ShouldBe(expectedScore);
        }

        [TestMethod]
        public void ReportComponent_ShouldCalculateMutationScore_BuildErrorIsNull()
        {
            var target = new CsharpFolderComposite();
            target.Add(new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.CompileError } } });

            var result = target.GetMutationScore();
            result.ShouldBe(double.NaN);
        }

        [TestMethod]
        public void ShouldGetNaNMutationScoreWhenAllExcluded()
        {
            var file = new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.Ignored } } };

            file.GetMutationScore().ShouldBe(double.NaN);
        }

        [TestMethod]
        public void ShouldGet0MutationScoreWhenAllNoCoverage()
        {
            var file = new CsharpFileLeaf() { Mutants = new Collection<Mutant>() { new Mutant() { ResultStatus = MutantStatus.NoCoverage } } };

            file.GetMutationScore().ShouldBe(0);
        }
    }
}
