namespace MtpParallelAttributionMini;

public static class MemoizedHelper
{
    private static string? _cache;

    public static string Get(bool reload = false)
    {
        if (reload)
        {
            _cache = null;
        }

        _cache ??= Build();
        return _cache;
    }

    private static string Build()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("first");
        sb.Append("-");
        sb.Append("second");
        return sb.ToString();
    }
}
