using System;
using System.Diagnostics;
using System.Threading;
using Stryker.DataCollector;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class CoverageCaptureTests
    {
        private static bool WaitFor(object lck, Func<bool> predicate, int timeout)
        {
            var start = new Stopwatch();
            start.Start();
            while (start.ElapsedMilliseconds<timeout)
            {
                lock (lck)
                {
                    if (predicate())
                    {
                        return true;
                    }
                    Monitor.Wait(lck, (int)Math.Max(0, timeout - start.ElapsedMilliseconds));
                }
            }

            return predicate();
        }
        
        [Fact]
        public void CanConnect()
        {
            using (var server = new CommunicationServer("testConnect"))
            {
                var count = 0;
                var lck = new object();
                server.Listen();
                server.RaiseNewClientEvent += (x, e) => {
                    lock (lck)
                    {
                        count++;
                        Monitor.Pulse(lck);
                    }
                };
                using (var client = CommunicationChannel.Client(server.PipeName))
                {
                    Assert.True(client.IsConnected);
                    Assert.True(WaitFor(lck, () =>  count==1, 100));
                }
                using (var client = CommunicationChannel.Client(server.PipeName))
                {
                    Assert.True(client.IsConnected);
                    Assert.True(WaitFor(lck, () =>  count==2, 100));
                }
            }
        }

        [Fact]
        public void CanSend()
        {
            using (var server = new CommunicationServer("test"))
            {
                var lck = new object();
                string message = string.Empty;
                CommunicationChannel serverSide = null;
                server.Listen();
                server.RaiseNewClientEvent += (o, e) => {
                    lock (lck)
                    {
                        serverSide = e.Client; 
                        serverSide.RaiseReceivedMessage+=(o2, msg) =>
                        {
                            lock (lck)
                            {
                                message = msg;
                                Monitor.Pulse(lck);
                            }
                        };
                        Monitor.Pulse(lck);
                    }
                };
                using (var client = CommunicationChannel.Client(server.PipeName))
                {
                    Assert.True(client.IsConnected);
                    Assert.True(WaitFor(lck, () =>  serverSide!=null, 100));
                    client.Start();
                    client.RaiseReceivedMessage += (o, msg) =>
                    {
                        client.SendText(msg);
                    };
                    serverSide.SendText("it works");
                    Assert.True(WaitFor(lck, () =>  !string.IsNullOrEmpty(message), 100));
                    Assert.Equal("it works", message);
                }
            }
        }
    }
}
