using System;
using System.Threading;
using Shouldly;
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
                    Helpers.WaitOnLck(lck, () => notified, 100).ShouldBeTrue();
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
                    client.IsConnected.ShouldBeTrue();
                    
                    Helpers.WaitOnLck(lck, () => notified, 100).ShouldBeTrue();

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
                    Helpers.WaitOnLck(lck, () => receivedMsg == message, 100);
                }
            }
        }

        private static bool WaitForMessage(object lck, Func<string> receiver, string message)
        {
            return Helpers.WaitOnLck(lck, () => receiver() == message, 100);
        }
    }
}