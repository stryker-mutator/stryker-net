using System.IO.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using Stryker.Core.Infrastructure;

namespace Stryker.Core.UnitTest.Infrastructure;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddStrykerCoreShouldRegisterDefaultServicesWithoutAConfigurationCallback()
    {
        var services = new ServiceCollection();

        services.AddStrykerCore();

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IFileSystem>().ShouldBeOfType<FileSystem>();
    }

    [TestMethod]
    public void AddStrykerCoreShouldAllowDefaultServicesToBeOverridden()
    {
        var fileSystem = new Mock<IFileSystem>().Object;
        var services = new ServiceCollection();

        services.AddStrykerCore(configure =>
            configure.AddSingleton(fileSystem));

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IFileSystem>().ShouldBeSameAs(fileSystem);
    }
}
