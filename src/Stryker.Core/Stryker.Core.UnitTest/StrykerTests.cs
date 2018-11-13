using Moq;
using Stryker.Core.Initialisation;
using Stryker.Core.Initialisation.ProjectComponent;
using Stryker.Core.MutationTest;
using Stryker.Core.Options;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Shouldly;
using Xunit;

namespace Stryker.Core.UnitTest
{
    public class StrykerTests
    {
        private string cached = null;
        private int? cachedId;

        private string GetMutant()
        {
            if (cached != null)
            {
                return cached;
            }

            cached = System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS");
            return cached;
        }

        private int GetMutantID()
        {
            if (cachedId != null)
            {
                return cachedId.Value;
            }

            cachedId = int.Parse(System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS"));
            return cachedId.Value;
        }

        [Fact]
        public void SpecOnPerf()
        {
            var count = 0;
            var start = new Stopwatch();
            start.Start();
            var l = 100000;
            for (var i = 0; i < l; i++)
            {
                if (GetMutantID() == 4)
                {
                    count++;
                }
            }

            start.Stop();
            start.ElapsedMilliseconds.ShouldBeLessThan(50);
            start.Reset();
            start.Start();
            for (var i = 0; i < l; i++)
            {
                if (GetMutant() == "4")
                {
                    count++;
                }
            }

            start.Stop();
            start.ElapsedMilliseconds.ShouldBeLessThan(60);            
            start.Reset();
            start.Start();
            for (var i = 0; i < l; i++)
            {
                if (System.Environment.GetEnvironmentVariable("NUMBER_OF_PROCESSORS") == "4")
                {
                    count++;
                }
            }

            start.Stop();
            start.ElapsedMilliseconds.ShouldBeLessThan(500);
        }

        [Fact]
        public void Stryker_ShouldInvokeAllProcesses()
        {
            string basePath = @"c:\TestProject\";

            var initialisationMock = new Mock<IInitialisationProcess>(MockBehavior.Strict);
            var mutationTestProcessMock = new Mock<IMutationTestProcess>(MockBehavior.Strict);

            initialisationMock.Setup(x => x.Initialize(It.IsAny<StrykerOptions>())).Returns(new MutationTestInput()
            {
                ProjectInfo = new ProjectInfo()
                {
                    TestProjectPath = basePath,
                    ProjectContents = new FolderComposite()
                    {
                        Name = "ProjectRoot",
                        Children = new Collection<ProjectComponent>() {
                            new FileLeaf() {
                                Name = "SomeFile.cs"
                            }
                        }
                    },
                    ProjectUnderTestAssemblyName = "ExampleProject.dll",
                    ProjectUnderTestPath = @"c:\ExampleProject\",
                    TargetFramework = "netcoreapp2.0"
                },
            });
            mutationTestProcessMock.Setup(x => x.Mutate());
            mutationTestProcessMock.Setup(x => x.Test(It.IsAny<StrykerOptions>()))
                .Returns(new StrykerRunResult(It.IsAny<StrykerOptions>(), It.IsAny<decimal?>()));

            var target = new StrykerRunner(initialisationMock.Object, mutationTestProcessMock.Object);

            var options = new StrykerOptions("c:/test", "Console", "", 2000, null, false, 1, 80, 60, 0);

            target.RunMutationTest(options);

            initialisationMock.Verify(x => x.Initialize(It.IsAny<StrykerOptions>()), Times.Once);
            mutationTestProcessMock.Verify(x => x.Mutate(), Times.Once);
            mutationTestProcessMock.Verify(x => x.Test(It.IsAny<StrykerOptions>()), Times.Once);
        }
    }
}
