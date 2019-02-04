using System;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Serialization;
using Stryker.Core.Coverage;
using Stryker.Core.InjectedHelpers.Coverage;
using Xunit;

namespace Stryker.Core.UnitTest.Coverage
{
    public class CoverageChannelShould
    {
        [Fact]
        public void OfferConnection()
        {
            using (var channel = new CommunicationServer("CanConnect"))
            {
                channel.Listen();
                using (var client = CommunicationChannel.Client(channel.PipeName, 1))
                {
                }
            }

        }

        [Fact]
        public void OfferConnectionEvent()
        {
            var lck = new object();
            using (var channel = new CommunicationServer("ConnectionEvent"))
            {
                var notified = false;
                channel.RaiseNewClientEvent += (o, args) =>
                {
                    lock (lck)
                    {
                        notified = true;
                        Monitor.Pulse(lck);
                    }
                };
                channel.Listen();

                using (var client = CommunicationChannel.Client(channel.PipeName, 1))
                {
                    lock (lck)
                    {
                        if (!notified)
                        {
                            Monitor.Wait(lck, 100);
                        }
                        Assert.True(notified);
                    }
                }
            }
        }

        [Fact]
        public void NotifyReceivedMessage()
        {
            using (var channel = new CommunicationServer("NotifyMessage"))
            {
                var receivedMsg = string.Empty;
                var lck = new object();
                channel.Listen();
                channel.RaiseReceivedMessage += (o, e) =>
                {
                    lock (lck)
                    {
                        receivedMsg = e;
                        Monitor.Pulse(lck);
                    }
                };
                using (var client = CommunicationChannel.Client(channel.PipeName, 1))
                {

                    var message = "hello";
                    client.SendText(message);
                    WaitForMessage(lck, ()=> receivedMsg, message);
                }
            }
        }

        [Fact]
        public void SupportDuplex()
        {
            object serverLck = new object();
            using (var channel = new CommunicationServer("Duplex"))
            {
                CommunicationChannel session = null;
                string response = string.Empty;
                channel.RaiseNewClientEvent += (o, e) =>
                {
                    lock (serverLck)
                    {
                        session = e.Client;
                        Monitor.Pulse(serverLck);
                    }
                };
                channel.Listen();
                channel.RaiseReceivedMessage += (o, msg) =>
                {
                    lock (serverLck)
                    {
                        response = msg;
                        Monitor.Pulse(serverLck);
                    }
                };
                using (var client = CommunicationChannel.Client(channel.PipeName, 1))
                {
                    client.RaiseReceivedMessage += (o, msg) =>
                    {
                        lock (client)
                        {
                            response = msg;
                            Monitor.Pulse(client);
                        }
                    };
                    client.Start();
                    Assert.True(client.IsConnected);
                    lock (serverLck)
                    {
                        if (session == null)
                        {
                            Monitor.Wait(serverLck, 100);
                        }
                    }
                    Assert.NotNull(session);
                    var message = "hello";
                    client.SendText(message);
                    WaitForMessage(serverLck, ()=>response, message);
                    response = string.Empty;
                    message = "world";
                    session.SendText(message);
                    WaitForMessage(client, ()=>response, message);
                }
            }
        }

        [Fact]
        public void NotifyReceivedMessageFromMoreThanOneClient()
        {
            using (var channel = new CommunicationServer("myChannel4"))
            {
                var lck = new object();
                var notified = false;
                channel.RaiseNewClientEvent+=(o, args) =>
                {
                    lock (lck)
                    {
                        notified = true;
                        Monitor.Pulse(lck);
                    }
                };
                channel.Listen();
                using (var client = CommunicationChannel.Client(channel.PipeName))
                    using(var otherClient= CommunicationChannel.Client(channel.PipeName))
                {
                    var receivedMsg = string.Empty;
                    Assert.True(client.IsConnected);
                    lock (lck)
                    {
                        if (!notified)
                        {
                            Monitor.Wait(lck, 100);
                            Assert.True(notified);
                        }

                        notified = false;
                    }

                    channel.RaiseReceivedMessage += (o, e) =>
                    {
                        lock (lck)
                        {
                            receivedMsg = e;
                            Monitor.Pulse(lck);
                        }
                    };
                    var message = "hello";
                    client.SendText(message);

                    WaitForMessage(lck, ()=>receivedMsg, message);
                    message = "world";
                    otherClient.SendText(message);
                    lock (lck)
                    {
                        if (receivedMsg != message)
                        {
                            Monitor.Wait(lck ,100);
                        }

                        Assert.Equal(message, receivedMsg);
                    }
                }
            }
        }

        private static void WaitForMessage(object lck, Func<string> receiver, string message)
        {
            lock (lck)
            {
                if (receiver() != message)
                {
                    Monitor.Wait(lck, 100);
                }

                Assert.Equal(message, receiver());
            }
        }

    }

}