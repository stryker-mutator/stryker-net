using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Abstractions;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.ProjectComponents;
using Stryker.Core.Initialisation;
using Stryker.Core.ProjectComponents.SourceProjects;

namespace Stryker.Core.UnitTest.Initialisation;

[TestClass]
public class MutableProjectTargetLanguageTests
{
    private sealed class StubBuilder(IFileSystem fileSystem, IReadOnlyProjectComponent contents) : ProjectComponentsBuilder(fileSystem)
    {
        public override IReadOnlyProjectComponent Build() => contents;
        public override void InjectHelpers(IReadOnlyProjectComponent inputFiles) { }
        public override Action PostBuildAction() => () => { };
    }

    [TestInitialize]
    public void Setup() => Stryker.Utilities.Logging.ApplicationLogging.LoggerFactory = NullLoggerFactory.Instance;

    private static MutableProjectTarget FsharpTarget() => new(
        TestHelper.SetupProjectAnalyzerResult(projectFilePath: "/p/p.fsproj", properties: new Dictionary<string, string> { ["Language"] = "F#" }).Object,
        NullLogger.Instance);

    [TestMethod]
    public void BuildSourceProjectInfo_FsharpProjectWithRegisteredBuilder_UsesTheKeyedBuilder()
    {
        var fileSystem = new MockFileSystem();
        var contents = new Mock<IReadOnlyProjectComponent>().Object;
        ProjectComponentsBuilderFactory factory = (_, _, _, _, fs) => new StubBuilder(fs, contents);
        var provider = new Mock<IKeyedServiceProvider>();
        provider.Setup(p => p.GetKeyedService(typeof(ProjectComponentsBuilderFactory), Language.Fsharp)).Returns((object)factory);

        var info = FsharpTarget().BuildSourceProjectInfo(Mock.Of<IStrykerOptions>(), fileSystem, provider.Object);

        info.ProjectContents.ShouldBeSameAs(contents);
    }

    [TestMethod]
    public void BuildSourceProjectInfo_FsharpProjectWithoutRegisteredBuilder_ThrowsInputException()
    {
        Should.Throw<InputException>(() => FsharpTarget().BuildSourceProjectInfo(Mock.Of<IStrykerOptions>(), new MockFileSystem(), Mock.Of<IKeyedServiceProvider>()))
            .Message.ShouldContain("/p/p.fsproj");
    }
}
