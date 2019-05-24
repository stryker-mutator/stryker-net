using System.ComponentModel;
using System.Threading;
using Stryker.Core.InjectedHelpers.Coverage;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class CoverageCaptureTests
    {
        [Fact]
        public void CanConnect()
        {
            using (var server = new CommunicationServer("test"))
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
                    lock (lck)
                    {
                        if (count == 0)
                        {
                            Monitor.Wait(lck, 100);
                        }
                        Assert.Equal(1, count);
                    }
                }
                using (var client = CommunicationChannel.Client(server.PipeName))
                {
                    Assert.True(client.IsConnected);
                    lock (lck)
                    {
                        if (count <2)
                        {
                            Monitor.Wait(lck, 100);
                        }
                        Assert.Equal(2, count);
                    }
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
                server.RaiseReceivedMessage += (o, msg) =>
                {
                    lock (lck)
                    {
                        message = msg;
                        Monitor.Pulse(lck);
                    }
                };
                server.RaiseNewClientEvent += (o, e) => {
                    lock (lck)
                    {
                        serverSide = e.Client; 
                        Monitor.Pulse(lck);
                    }
                };
                using (var client = CommunicationChannel.Client(server.PipeName))
                {
                    Assert.True(client.IsConnected);
                    lock (lck)
                    {
                        if (serverSide == null)
                        {
                            Monitor.Wait(lck, 100);
                        }
                    }
                    client.Start();
                    client.RaiseReceivedMessage += (o, msg) =>
                    {
                        client.SendText(msg);
                    };
                    serverSide.SendText("it works");
                    lock (lck)
                    {
                        if (string.IsNullOrEmpty(message))
                        {
                            Monitor.Wait(lck, 100);
                        }
                    }
                    Assert.Equal("it works", message);
                }
            }

        }
    }
}
