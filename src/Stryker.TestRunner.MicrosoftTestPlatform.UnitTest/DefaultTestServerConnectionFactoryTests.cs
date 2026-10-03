using Shouldly;

namespace Stryker.TestRunner.MicrosoftTestPlatform.UnitTest;

[TestClass]
public class DefaultTestServerConnectionFactoryTests
{
    [TestMethod]
    public void BuildCommand_ShouldUseDotnetForDll()
    {
        var (targetFilePath, arguments) = DefaultTestServerConnectionFactory.BuildCommand(@"C:\tests\TestProject.dll", 1234);

        targetFilePath.ShouldBe("dotnet");
        arguments.ShouldBe([@"C:\tests\TestProject.dll", "--server", "--client-port", "1234"]);
    }

    [TestMethod]
    [DataRow(@"C:\tests\TestProject.exe")]
    [DataRow(@"C:\tests\TestProject.EXE")]
    public void BuildCommand_ShouldRunExeDirectly(string assembly)
    {
        var (targetFilePath, arguments) = DefaultTestServerConnectionFactory.BuildCommand(assembly, 1234);

        targetFilePath.ShouldBe(assembly);
        arguments.ShouldBe(["--server", "--client-port", "1234"]);
    }
}
