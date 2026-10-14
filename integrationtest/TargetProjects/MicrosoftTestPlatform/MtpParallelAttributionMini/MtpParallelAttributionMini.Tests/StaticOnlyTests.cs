using MtpParallelAttributionMini;
using Xunit;

namespace MtpParallelAttributionMini.Tests;

public class StaticOnlyTests
{
    [Fact]
    public void Value_IsBuiltFromAllParts() => Assert.Equal("alpha-beta", StaticOnlyHelper.Value);
}