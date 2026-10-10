namespace MtpParallelAttributionMini;

public static class StaticOnlyHelper
{
    public static readonly string Value = Build();

    private static string Build() => "alpha" + "-" + "beta";
}