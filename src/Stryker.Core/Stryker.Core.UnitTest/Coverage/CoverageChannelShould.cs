using System;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Stryker.Core.Coverage;
using Stryker.Core.InjectedHelpers.Coverage;
using Xunit;

namespace Stryker.Core.UnitTest.Coverage
{
    public class CoverageChannelShould
    {
        private string received;

        private object lckMessage = new object();

        [Fact]
        public void OfferConnection()
        {
            using (var channel = new CommunicationServer("myChannel1"))
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
            using (var channel = new CommunicationServer("myChannel2"))
            {
                var notified = false;
                channel.Listen();
                channel.RaiseNewClientEvent += (o, args) =>
                {
                    lock (channel)
                    {
                        notified = true;
                        Monitor.Pulse(channel);
                    }
                };

                using (var client = CommunicationChannel.Client(channel.PipeName, 1))
                {
                    lock (channel)
                    {
                        if (!notified)
                        {
                            Monitor.Wait(channel, 50);
                        }
                        Assert.True(notified);
                    }
                }
            }
        }

        [Fact]
        public void NotifyReceivedMessage()
        {
            using (var channel = new CommunicationServer("myChannel3"))
            {
                channel.Listen();
                channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;
                using (var client = CommunicationChannel.Client(channel.PipeName, 1))
                {

                    var message = "hello";
                    client.SendText(message);
                    ExpectThisMessage(message);
                }
            }
        }

        [Fact]
        public void SupportDuplex()
        {
            using (var channel = new CommunicationServer("myChannel3"))
            {
                CommunicationChannel session = null;
                string response = string.Empty;
                channel.RaiseNewClientEvent += (o, e) =>
                {
                    lock (channel)
                    {
                        session = e.Client;
                        Monitor.Pulse(channel);
                    }
                };
                channel.Listen();
                channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;
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
                    var message = "hello";
                    client.SendText(message);
                    ExpectThisMessage(message);
                    lock (channel)
                    {
                        if (session == null)
                        {
                            Monitor.Wait(10);
                        }
                    }
                    Assert.NotNull(session);
                    session.SendText("world");
                    lock (client)
                    {
                        if (string.IsNullOrEmpty(response))
                        {
                            Monitor.Wait(client, 10);
                        }
                    }
                    Assert.Equal("world", response);
                }
            }
        }

        [Fact]
        public void NotifyReceivedMessageFromMoreThanOneClient()
        {
            using (var channel = new CommunicationServer("myChannel4"))
            {
                var notified = false;
                channel.RaiseNewClientEvent+=(o, args) => notified = true;
                channel.Listen();
                using (var client = new NamedPipeClientStream(".", channel.PipeName, PipeDirection.InOut,
                    PipeOptions.Asynchronous))
                    using(var otherClient= new NamedPipeClientStream(".", channel.PipeName, PipeDirection.InOut,
                    PipeOptions.Asynchronous))
                {
                    client.Connect(5);
                    Assert.True(client.IsConnected);
                    channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;
                    otherClient.Connect(5);
                    var message = "hello";
                    SendText(client, message);

                    ExpectThisMessage(message);

                    SendText(client, "world");
                    ExpectThisMessage("world");
                }
                Assert.True(notified);
            }
        }

        private static void SendText(NamedPipeClientStream client, string message)
        {
            var buffer = Encoding.Unicode.GetBytes(message);
            client.Write(BitConverter.GetBytes(buffer.Length));
            client.Write(buffer);
        }

        private void ExpectThisMessage(string message)
        {
            lock (lckMessage)
            {
                if (received != message)
                {
                    Monitor.Wait(lckMessage, 5);
                }
            }

            Assert.Equal(message, received);
        }

        private void Channel_RaiseReceivedMessage(object sender, string args)
        {
            lock (lckMessage)
            {
                received = args;
                Monitor.Pulse(lckMessage);
            }
        }
    }

}