using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Stryker.Core.Coverage;

namespace Stryker.Core.MutationTest
{
    public class CoverageServer : IDisposable
    {
        private CommunicationServer coverageServer;
        private List<int> ranMutants;
        private object lck = new object();

        public IList<int> RanMutants => ranMutants;

        public string PipeName => coverageServer.PipeName;

        public CoverageServer()
        {
            coverageServer = new CommunicationServer("Coverage");
            coverageServer.RaiseReceivedMessage += (sender, args) =>
            {
                var tmp = args.Split(',').Select(int.Parse);
                lock (lck)
                {
                    ranMutants = tmp.ToList();
                    Monitor.Pulse(lck);
                }
            };
            coverageServer.Listen();
        }

        public bool WaitReception(int timeoutInMs = 100)
        {
            lock (lck)
            {
                if (ranMutants == null)
                {
                    // wait a bit to see if we receive the report
                    Monitor.Wait(lck, 100);
                }
            }

            return ranMutants != null;
        }

        public void Dispose()
        {
            coverageServer?.Dispose();
        }
    }
}
