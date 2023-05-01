using System;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs;

public class PortInputTests : TestBase
{
    private readonly PortInput _sut;

    public PortInputTests() => _sut = new PortInput();

    [Fact]
    public void ShouldHaveCorrectHelpMessage() => _sut.HelpText.ShouldBeEquivalentTo("The port used when realtime reporting is enabled | default: '8080'");

    [Fact]
    public void ShouldThrowIfPortIsNegative()
    {
        const int NegativePort = -1;

        _sut.SuppliedInput = NegativePort;

        Should.Throw<ArgumentOutOfRangeException>(() => _sut.Validate())
            .Message
            .ShouldBeEquivalentTo("Specified argument was out of the range of valid values. (Parameter 'Port should be between range 0 - 65535')");
    }

    [Fact]
    public void ShouldThrowIfPortIsHigherThanMaximum()
    {
        const int PositivePort = ushort.MaxValue + 1;

        _sut.SuppliedInput = PositivePort;

        Should.Throw<ArgumentOutOfRangeException>(() => _sut.Validate())
            .Message
            .ShouldBeEquivalentTo("Specified argument was out of the range of valid values. (Parameter 'Port should be between range 0 - 65535')");

    }

    [Theory]
    [InlineData(0)]
    [InlineData(8081)]
    [InlineData(65535)]
    public void ShouldNotThrowIfPortIsMaximum(int port)
    {
        _sut.SuppliedInput = port;
        Should.NotThrow(() => _sut.Validate()).ShouldBeEquivalentTo(port);
    }
}
