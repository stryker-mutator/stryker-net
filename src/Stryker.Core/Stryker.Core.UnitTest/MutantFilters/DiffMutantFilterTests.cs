using Moq;
using Shouldly;
using Stryker.Core.DiffProviders;
using Stryker.Core.MutantFilters;
using Stryker.Core.Mutants;
using Stryker.Core.Options;
using Stryker.Core.ProjectComponents;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Xunit;

namespace Stryker.Core.UnitTest.MutantFilters
{
    public class DiffMutantFilterTests
    {
        [Fact]
        public static void ShouldHaveName()
        {
            var target = new DiffMutantFilter(new StrykerOptions(), null) as IMutantFilter;
            target.DisplayName.ShouldBe("git diff file filter");
        }

        [Fact]
        public void ShouldNotMutateUnchangedFiles()
        {
            var options = new StrykerProjectOptions(diff: true);
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);
            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
            });
            var target = new DiffMutantFilter(options, diffProvider.Object);
            var file = new FileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            var filterResult = target.FilterMutants( new List<Mutant>() { mutant }, file, options);

            filterResult.ShouldBeEmpty();
        }

        [Fact]
        public void ShouldOnlyMutateChangedFiles()
        {
            var options = new StrykerProjectOptions(diff: true);
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);
            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
                {
                    myFile
                }
            });
            var target = new DiffMutantFilter(options, diffProvider.Object);
            var file = new FileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void ShouldMutateAllFilesWhenTurnedOff()
        {
            var options = new StrykerProjectOptions(diff: false);
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);
            string myFile = Path.Combine("C:/test/", "myfile.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
            });
            var target = new DiffMutantFilter(options, diffProvider.Object);
            var file = new FileLeaf { FullPath = myFile };

            var mutant = new Mutant();

            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            filterResult.ShouldContain(mutant);
        }

        [Fact]
        public void ShouldMutateAllFilesWhenATestHasBeenChanged()
        {
            string testProjectPath = "C:/MyTests";
            var options = new StrykerProjectOptions(diff: false);
            var diffProvider = new Mock<IDiffProvider>(MockBehavior.Strict);
            // If a file inside the test project is changed, a test has been changed
            string myTest = Path.Combine(testProjectPath, "myTest.cs"); ;
            diffProvider.Setup(x => x.ScanDiff()).Returns(new DiffResult()
            {
                ChangedFiles = new Collection<string>()
                {
                    myTest
                }
            });
            var target = new DiffMutantFilter(options, diffProvider.Object);

            // check the diff result for a file not inside the test project
            var file = new FileLeaf { FullPath = Path.Combine("C:/NotMyTests", "myfile.cs") };

            var mutant = new Mutant();

            var filterResult = target.FilterMutants(new List<Mutant>() { mutant }, file, options);

            filterResult.ShouldContain(mutant);
        }
    }
}
