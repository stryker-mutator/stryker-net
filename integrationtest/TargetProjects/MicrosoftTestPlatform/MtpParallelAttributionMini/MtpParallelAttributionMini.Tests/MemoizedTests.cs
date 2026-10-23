using MtpParallelAttributionMini;
using Xunit;

namespace MtpParallelAttributionMini.Tests;

public class MemoizedTests
{
    [Fact]
    public void Get_returns_cached_value()
    {
        Assert.Equal("first-second", MemoizedHelper.Get(reload: true));
        Assert.Equal("first-second", MemoizedHelper.Get());
    }
}
