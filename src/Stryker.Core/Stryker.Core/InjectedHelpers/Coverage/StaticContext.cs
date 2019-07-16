using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker
{
    public sealed class StaticContext: IDisposable
    {
        [ThreadStatic] private static int depth;

        public StaticContext()
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
    }
}
