using Moq;
using Stryker.CLI.Clients;
using Stryker.CLI.Logging;
using Stryker.Core;
using Xunit;

namespace Stryker.CLI.UnitTest;

public class StrykerCLIInitCommandTests
{
    private readonly StrykerCli _target;
    private readonly Mock<IStrykerRunner> _strykerRunnerMock = new(MockBehavior.Strict);
    private readonly Mock<IStrykerNugetFeedClient> _nugetClientMock = new(MockBehavior.Strict);
    private readonly Mock<ILoggingInitializer> _loggingInitializerMock = new();

    public StrykerCLIInitCommandTests()
    {
        _target = new StrykerCli(_strykerRunnerMock.Object, null, _loggingInitializerMock.Object, _nugetClientMock.Object);
    }

    [Theory]
    [InlineData("init")]
    [InlineData("init", "--config-file", "\"test.json\"")]
    [InlineData("init", "-f", "\"test.json\"")]
    public void Init(params string[] argName)
    {
        _target.Run(argName);

        _strykerRunnerMock.VerifyAll();
    }
}
