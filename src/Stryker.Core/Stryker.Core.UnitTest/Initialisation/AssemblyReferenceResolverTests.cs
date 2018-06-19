using Moq;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.Testing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.Initialisation
{
    public class AssemblyReferenceResolverTests
    {
        [Fact]
        public void ShouldAnalyzeProjectDependencies()
        {
            var foundReferences = new List<string>();
            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            var metadataReferenceProviderMock = new Mock<IMetadataReferenceProvider>(MockBehavior.Strict);

            processExecutorMock.Setup(x => x.Start(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = @"  ExampleProject.Contracts -> C:\Repos\ExampleProject\ExampleProject\bin\Debug\netcoreapp2.0\ExampleProject.Contracts.dll
  ExampleProject -> C:\Repos\ExampleProject\ExampleProject\bin\Debug\netcoreapp2.0\ExampleProject.dll
  C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.0.0\ref\netcoreapp2.0\Microsoft.CSharp.dll;C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.aspnetcore.mvc.formatters.json\2.0.2\lib\netstandard2.0\Microsoft.AspNetCore.Mvc.Formatters.Json.dll".Replace('\\', Path.DirectorySeparatorChar)
                });
            metadataReferenceProviderMock.Setup(x => x.CreateFromFile(It.IsAny<string>()))
                .Returns(() => null)
                .Callback((string reference) => foundReferences.Add(reference));

            string project = "C:/ProjectFolder/ExampleProject/ExampleProject.Test.csproj";

            var target = new AssemblyReferenceResolver(processExecutorMock.Object, metadataReferenceProviderMock.Object);

            var result = target.ResolveReferences(Path.GetDirectoryName(project), Path.GetFileName(project), "ExampleProject").ToList();

            processExecutorMock.Verify(x => x.Start(
                Path.GetDirectoryName(project), 
                "dotnet", "msbuild ExampleProject.Test.csproj /nologo /t:PrintReferences", 
                null), 
                Times.Once);
            foundReferences.ShouldBeSubsetOf(new List<string>()
            {
                @"C:\Repos\ExampleProject\ExampleProject\bin\Debug\netcoreapp2.0\ExampleProject.Contracts.dll",
                @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.0.0\ref\netcoreapp2.0\Microsoft.CSharp.dll",
                @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.aspnetcore.mvc.formatters.json\2.0.2\lib\netstandard2.0\Microsoft.AspNetCore.Mvc.Formatters.Json.dll"
            });
        }

        [Fact]
        public void FoundPathsShouldBeReturnedAsPortableExecutionReferences()
        {
            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            var metadataReferenceProviderMock = new Mock<IMetadataReferenceProvider>(MockBehavior.Strict);

            processExecutorMock.Setup(x => x.Start(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = @"  AnotherExampleProject -> C:\Repos\ExampleTestProject\ExampleTestProject\Bin\Debug\netcoreapp2.0\AnotherExampleProject.dll
  ExampleProject -> C:\Repos\ExampleProject\ExampleProject\Bin\Debug\netcoreapp2.0\ExampleProject.dll
  C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.0.0\ref\netcoreapp2.0\Microsoft.CSharp.dll;C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.aspnetcore.mvc.formatters.json\2.0.2\lib\netstandard2.0\Microsoft.AspNetCore.Mvc.Formatters.Json.dll"
                });
            metadataReferenceProviderMock.Setup(x => x.CreateFromFile(It.IsAny<string>())).Returns(() => null);

            string project = @"C:\ProjectFolder\ExampleProject\ExampleProject.Test.csproj";

            var target = new AssemblyReferenceResolver(processExecutorMock.Object, metadataReferenceProviderMock.Object);

            var result = target.ResolveReferences(Path.GetDirectoryName(project), Path.GetFileName(project), "ExampleProject");

            // three references should be found in the above output
            result.Count().ShouldBe(3);
            metadataReferenceProviderMock.Verify(x => x.CreateFromFile(It.IsAny<string>()), Times.Exactly(3));
        }

        [Fact]
        public void AssemblyReferenceResolver_BuildTaskNotSetShouldThrowException()
        {
            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            var metadataReferenceProviderMock = new Mock<IMetadataReferenceProvider>(MockBehavior.Strict);

            processExecutorMock.Setup(x => x.Start(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 1,
                    Output = @"C:\ProjectFolder\ExampleProject\ExampleProject.Test.csproj : error MSB4057: The target ""PrintReferences"" does not exist in the project."
                });
            string project = @"C:\ProjectFolder\ExampleProject\ExampleProject.Test.csproj";

            var target = new AssemblyReferenceResolver(processExecutorMock.Object, metadataReferenceProviderMock.Object);

            // The resolver should throw an exception when the target task was not found
            var exception = Assert.Throws<ApplicationException>(() => target.ResolveReferences(
                Path.GetDirectoryName(project), 
                Path.GetFileName(project),
                ""
                ).ToList());

            exception.Message.ShouldContain("PrintReferences");
        }

        [Fact]
        public void AssemblyReferenceResolver_ShouldSkipProjectUnderTest()
        {
            var foundReferences = new List<string>();
            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            var metadataReferenceProviderMock = new Mock<IMetadataReferenceProvider>(MockBehavior.Strict);

            processExecutorMock.Setup(x => x.Start(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<IEnumerable<KeyValuePair<string, string>>>()))
                .Returns(new ProcessResult()
                {
                    ExitCode = 0,
                    Output = @"  MGA.Contracts -> C:\Mrak\IS-MarkW\Product\MGA.Contracts\MGA.Contracts\bin\Debug\netcoreapp2.0\MGA.Contracts.dll
  MGA.MaandstaatKeuringsFrontend.Domain -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Domain\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Domain.dll
  MGA.MaandstaatKeuringsFrontend.DomainServices -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.DomainServices\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.DomainServices.dll
  MGA.MaandstaatKeuringsFrontend.DAL -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.DAL\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.DAL.dll
  MGA.MaandstaatKeuringsFrontend.Infrastructure -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Infrastructure\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Infrastructure.dll
  MGA.MaandstaatKeuringsFrontend.Facade -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Facade\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Facade.dll
  C:\Mrak\IS-MarkW\Product\MGA.Contracts\MGA.Contracts\bin\Debug\netcoreapp2.0\MGA.Contracts.dll;C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Facade\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Facade.dll;"
                });
            metadataReferenceProviderMock.Setup(x => x.CreateFromFile(It.IsAny<string>()))
                .Returns(() => null)
                .Callback((string reference) => foundReferences.Add(reference));

            string project = @"C:\ProjectFolder\ExampleProject\ExampleProject.Test.csproj";

            var target = new AssemblyReferenceResolver(processExecutorMock.Object, metadataReferenceProviderMock.Object);

            var result = target.ResolveReferences(Path.GetDirectoryName(project), Path.GetFileName(project), "MGA.MaandstaatKeuringsFrontend.Facade").ToList();

            // the project under test should be skipped, even when it is in the nuget packages list
            foundReferences.ShouldNotContain(@"C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Facade\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Facade.dll");
        }

        [Fact]
        public void AssemblyReferenceResolver_PathWithDashShouldBeAllowed()
        {
            var target = new AssemblyReferenceResolver(null, null);

            // The string should be split at " -> " and not at "-"
            var result = target.GetReferencePathsFromOutput(new Collection<string>() { @"InfoSupport.BestuurdersCoach.MessageBus -> U:\source\IS-StefanK\InfoSupport.BestuurdersCoach\InfoSupport.BestuurdersCoach.MessageBus\bin\Debug\netstandard2.0\InfoSupport.BestuurdersCoach.MessageBus.dll" });

            result.Single().ShouldBe(@"U:\source\IS-StefanK\InfoSupport.BestuurdersCoach\InfoSupport.BestuurdersCoach.MessageBus\bin\Debug\netstandard2.0\InfoSupport.BestuurdersCoach.MessageBus.dll");
        }


        [Fact]
        public void AssemblyReferenceResolver_PathShouldEndWithdllExtension()
        {
            var target = new AssemblyReferenceResolver(null, null);

            // The string should be split at " -> " and not at "-"
            var result = target.GetAssemblyPathsFromOutput(@"U:\source\IS-StefanK\InfoSupport.BestuurdersCoach\InfoSupport.BestuurdersCoach.MessageBus\bin\Debug\netstandard2.0\InfoSupport.BestuurdersCoach.MessageBus.exe");

            result.ShouldBeEmpty();
        }
    }
}
