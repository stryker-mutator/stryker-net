using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker;

internal sealed class MutantContext: IDisposable
{
    [ThreadStatic] private static int depth;

    public MutantContext()
    {
        depth++;
    }

    public void Dispose()
    {
        depth--;
    }

    public static bool InStatic()
    {
        return depth > 0;
    }

    public static T TrackValue<T>(Func<T> builder)
    {
        using (MutantContext context = new MutantContext())
        {
            return builder();
        }
    }
}
