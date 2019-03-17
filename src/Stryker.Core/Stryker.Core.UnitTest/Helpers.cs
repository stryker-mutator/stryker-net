using System;
using System.Diagnostics;
using System.Threading;

namespace Stryker.Core.UnitTest
{
    internal static class Helpers
    {
        static public bool WaitOnLck(object lck, Func<bool> predicate, int timeout)
        {
            var watch = new Stopwatch();
            lock (lck)
            {
                while (!predicate() && watch.ElapsedMilliseconds < timeout)
                {
                    Monitor.Wait(lck, Math.Max(0, (int)(timeout - watch.ElapsedMilliseconds)));
                }
            }

            return predicate();
        }

    }
}
