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
    /// <summary>
    /// msbuild will return a string with all referenced assemblies for the given test project. 
    /// The AssemblyReferenceResolver should parse this string correctly into separate dependencies for Roslyn.
    /// </summary>
    public class AssemblyReferenceResolverTests
    {
        [Fact]
        public void ShouldAnalyzeProjectDependencies()
        {
            var foundReferences = new List<string>();
            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            var metadataReferenceProviderMock = new Mock<IMetadataReferenceProvider>(MockBehavior.Strict);

            processExecutorMock.SetupProcessMockToReturn(@"  ExampleProject.Contracts -> C:\Repos\ExampleProject\ExampleProject\bin\Debug\netcoreapp2.0\ExampleProject.Contracts.dll
  ExampleProject -> C:\Repos\ExampleProject\ExampleProject\bin\Debug\netcoreapp2.0\ExampleProject.dll
  C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.0.0\ref\netcoreapp2.0\Microsoft.CSharp.dll;C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.aspnetcore.mvc.formatters.json\2.0.2\lib\netstandard2.0\Microsoft.AspNetCore.Mvc.Formatters.Json.dll");

            metadataReferenceProviderMock.Setup(x => x.CreateFromFile(It.IsAny<string>()))
                .Returns(() => null)
                .Callback((string reference) => foundReferences.Add(reference));

            string project = Path.Combine("C:", "ProjectFolder", "ExampleProject", "ExampleProject.Test.csproj");

            var target = new AssemblyReferenceResolver(processExecutorMock.Object, metadataReferenceProviderMock.Object);

            var result = target.ResolveReferences(Path.GetDirectoryName(project), Path.GetFileName(project), "ExampleProject").ToList();

            processExecutorMock.Verify(x => x.Start(
                Path.GetDirectoryName(project),
                "dotnet", "msbuild ExampleProject.Test.csproj /nologo /t:PrintReferences",
                null,
                It.IsAny<int>()),
                Times.Once);
            foundReferences.ShouldBeSubsetOf(new List<string>()
            {
                Path.Combine("C:", "Repos", "ExampleProject", "ExampleProject", "bin", "Debug", "netcoreapp2.0", "ExampleProject.Contracts.dll"),
                Path.Combine("C:", "Program Files", "dotnet", "sdk", "NuGetFallbackFolder", "microsoft.netcore.app", "2.0.0", "ref", "netcoreapp2.0", "Microsoft.CSharp.dll"),
                Path.Combine("C:", "Program Files", "dotnet", "sdk", "NuGetFallbackFolder", "microsoft.aspnetcore.mvc.formatters.json", "2.0.2", "lib", "netstandard2.0", "Microsoft.AspNetCore.Mvc.Formatters.Json.dll")
            });
        }

        [Fact]
        public void FoundPathsShouldBeReturnedAsPortableExecutionReferences()
        {
            var processExecutorMock = new Mock<IProcessExecutor>(MockBehavior.Strict);
            var metadataReferenceProviderMock = new Mock<IMetadataReferenceProvider>(MockBehavior.Strict);

            processExecutorMock.SetupProcessMockToReturn(@"  AnotherExampleProject -> C:\Repos\ExampleTestProject\ExampleTestProject\Bin\Debug\netcoreapp2.0\AnotherExampleProject.dll
  ExampleProject -> C:\Repos\ExampleProject\ExampleProject\Bin\Debug\netcoreapp2.0\ExampleProject.dll
  C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.netcore.app\2.0.0\ref\netcoreapp2.0\Microsoft.CSharp.dll;C:\Program Files\dotnet\sdk\NuGetFallbackFolder\microsoft.aspnetcore.mvc.formatters.json\2.0.2\lib\netstandard2.0\Microsoft.AspNetCore.Mvc.Formatters.Json.dll");

            metadataReferenceProviderMock.Setup(x => x.CreateFromFile(It.IsAny<string>())).Returns(() => null);

            string project = Path.Combine("C:", "ProjectFolder", "ExampleProject", "ExampleProject.Test.csproj");

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

            processExecutorMock.SetupProcessMockToReturn(@"C:\ProjectFolder\ExampleProject\ExampleProject.Test.csproj : error MSB4057: The target ""PrintReferences"" does not exist in the project.", 1);

            string project = Path.Combine("C:", "ProjectFolder", "ExampleProject", "ExampleProject.Test.csproj");

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

            processExecutorMock.SetupProcessMockToReturn(@"  MGA.Contracts -> C:\Mrak\IS-MarkW\Product\MGA.Contracts\MGA.Contracts\bin\Debug\netcoreapp2.0\MGA.Contracts.dll
  MGA.MaandstaatKeuringsFrontend.Domain -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Domain\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Domain.dll
  MGA.MaandstaatKeuringsFrontend.DomainServices -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.DomainServices\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.DomainServices.dll
  MGA.MaandstaatKeuringsFrontend.DAL -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.DAL\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.DAL.dll
  MGA.MaandstaatKeuringsFrontend.Infrastructure -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Infrastructure\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Infrastructure.dll
  MGA.MaandstaatKeuringsFrontend.Facade -> C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Facade\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Facade.dll
  C:\Mrak\IS-MarkW\Product\MGA.Contracts\MGA.Contracts\bin\Debug\netcoreapp2.0\MGA.Contracts.dll;C:\Mrak\IS-MarkW\Product\MGA.MaandstaatKeuringsFrontend\MGA.MaandstaatKeuringsFrontend.Facade\bin\Debug\netcoreapp2.0\MGA.MaandstaatKeuringsFrontend.Facade.dll;");

            metadataReferenceProviderMock.Setup(x => x.CreateFromFile(It.IsAny<string>()))
                .Returns(() => null)
                .Callback((string reference) => foundReferences.Add(reference));

            string project = Path.Combine("C:", "ProjectFolder", "ExampleProject", "ExampleProject.Test.csproj");

            var target = new AssemblyReferenceResolver(processExecutorMock.Object, metadataReferenceProviderMock.Object);

            var result = target.ResolveReferences(Path.GetDirectoryName(project), Path.GetFileName(project), "MGA.MaandstaatKeuringsFrontend.Facade").ToList();

            // the project under test should be skipped, even when it is in the nuget packages list
            foundReferences.ShouldNotContain(
                Path.Combine("C:", "Mrak", "IS-MarkW", "Product",
                "MGA.MaandstaatKeuringsFrontend", "MGA.MaandstaatKeuringsFrontend.Facade", "bin",
                "Debug", "netcoreapp2.0", "MGA.MaandstaatKeuringsFrontend.Facade.dll"));
        }

        [Fact]
        public void AssemblyReferenceResolver_PathWithDashShouldBeAllowed()
        {
            var target = new AssemblyReferenceResolver(null, null);
            var path = Path.Combine("U:", "source", "IS-StefanK", "InfoSupport.BestuurdersCoach",
                "InfoSupport.BestuurdersCoach.MessageBus", "bin", "Debug", "netstandard2.0",
                "InfoSupport.BestuurdersCoach.MessageBus.dll");

            // The string should be split at " -> " and not at "-"
            var result = target.GetReferencePathsFromOutput(new Collection<string>() { $@"InfoSupport.BestuurdersCoach.MessageBus -> {path}" });

            result.Single().ShouldBe(path);
        }


        [Fact]
        public void AssemblyReferenceResolver_PathShouldEndWithdllExtension()
        {
            var target = new AssemblyReferenceResolver(null, null);

            // The path should not end with .exe
            var result = target.GetAssemblyPathsFromOutput(
                Path.Combine("U:", "source", "IS-StefanK", "InfoSupport.BestuurdersCoach",
                "InfoSupport.BestuurdersCoach.MessageBus", "bin", "Debug", "netstandard2.0",
                "InfoSupport.BestuurdersCoach.MessageBus.exe"));

            result.ShouldBeEmpty();
        }
    }
}
