using System.IO.Pipes;
using System.Text;
using System.Threading;
using Stryker.Core.Coverage;
using Xunit;

namespace Stryker.Core.UnitTest.Coverage
{
    public class CoverageChannelShould
    {
        private string received;

        private object lck = new object();
        private object lckMessage = new object();

        [Fact]
        public void OfferConnection()
        {
            using (var channel = new CoverageServer("myChannel1"))
            {

                channel.Listen();
                using (var client = new NamedPipeClientStream(".", channel.PipeName, PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly))
                {
                    client.Connect(1);
                    Assert.True(client.IsConnected);
                }
            }

        }

        [Fact]
        public void OfferConnectionEvent()
        {
            using (var channel = new CoverageServer("myChannel2"))
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

                using (var client = new NamedPipeClientStream(".", channel.PipeName, PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly))
                {
                    client.Connect(1);
                    Assert.True(client.IsConnected);
                    lock (channel)
                    {
                        if (!notified)
                        {
                            Monitor.Wait(channel, 10);
                        }
                        Assert.True(notified);
                    }
                }

            }
        }

        [Fact]
        public void NotifyReceivedMessage()
        {
            using (var channel = new CoverageServer("myChannel3"))
            {
                var notified = false;
                channel.RaiseNewClientEvent += (o, args) => notified = true;
                channel.Listen();
                using (var client = new NamedPipeClientStream(".", channel.PipeName, PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly))
                {
                    client.Connect(1);
                    Assert.True(client.IsConnected);
                    channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;

                    var message = "hello";
                    client.Write(Encoding.Unicode.GetBytes(message));
                    ExpectThisMessage(message);
                }
            }
        }

        [Fact]
        public void NotifyReceivedMessageFromMoreThanOneClient()
        {
            using (var channel = new CoverageServer("myChannel4"))
            {
                var notified = false;
                channel.RaiseNewClientEvent+=(o, args) => notified = true;
                channel.Listen();
                using (var client = new NamedPipeClientStream(".", channel.PipeName, PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly))
                    using(var otherClient= new NamedPipeClientStream(".", channel.PipeName, PipeDirection.InOut,
                    PipeOptions.Asynchronous | PipeOptions.CurrentUserOnly))
                {
                    client.Connect(1);
                    Assert.True(client.IsConnected);
                    channel.RaiseReceivedMessage += Channel_RaiseReceivedMessage;
                    otherClient.Connect(1);
                    var message = "hello";
                    client.Write(Encoding.Unicode.GetBytes(message));

                    ExpectThisMessage(message);

                    otherClient.Write(Encoding.Unicode.GetBytes("world"));
                    ExpectThisMessage("world");
                }
                Assert.True(notified);
            }
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