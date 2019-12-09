using Shouldly;
using Stryker.DataCollector;
using System;
using System.Diagnostics;
using System.Threading;
using System.Xml;
using Xunit;

namespace Stryker.Core.UnitTest.TestRunners
{
    public class CoverageCaptureTests
    {
        private static bool WaitFor(object lck, Func<bool> predicate, int timeout)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < timeout)
            {
                lock (lck)
                {
                    if (predicate())
                    {
                        return true;
                    }
                    Monitor.Wait(lck, (int)Math.Max(0, timeout - stopwatch.ElapsedMilliseconds));
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
                server.RaiseNewClientEvent += (x, e) =>
                {
                    lock (lck)
                    {
                        count++;
                        Monitor.Pulse(lck);
                    }
                };
                using (var client = CommunicationChannel.Client(server.PipeName))
                {
                    client.IsConnected.ShouldBeTrue("First client could not connect to server pipe");
                    WaitFor(lck, () => count == 1, 1000).ShouldBeTrue("First client connection event was not raised");
                }
                using (var client = CommunicationChannel.Client(server.PipeName))
                {
                    client.IsConnected.ShouldBeTrue("Second client could not connect to server pipe");
                    WaitFor(lck, () => count == 2, 1000).ShouldBeTrue("First client connection event was not raised");
                }
            }
        }

        [Fact]
        public void CanSend()
        {
            using (var server = new CommunicationServer("test"))
            {
                var lck = new object();
                var message = string.Empty;
                CommunicationChannel serverSide = null;
                server.Listen();
                server.RaiseNewClientEvent += (o, e) =>
                {
                    lock (lck)
                    {
                        serverSide = e.Client;
                        serverSide.RaiseReceivedMessage += (o2, msg) =>
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
                    client.IsConnected.ShouldBeTrue("Client could not connect to server pipe");
                    WaitFor(lck, () => serverSide != null, 1000).ShouldBeTrue("Client connection event was not raised");
                    client.Start();
                    client.RaiseReceivedMessage += (o, msg) =>
                    {
                        client.SendText(msg);
                    };
                    serverSide.SendText("it works");
                    WaitFor(lck, () => !string.IsNullOrEmpty(message), 1000).ShouldBeTrue("Client message received event was not raised");
                    message.ShouldBe("it works");
                }
            }
        }

        [Fact]
        public void CanParseConfiguration()
        {
            var referenceConf="<Parameters><Environment name=\"ActiveMutant\" value=\"1\"/></Parameters>";
            var node = new XmlDocument();

            node.LoadXml(referenceConf);

            node.ChildNodes.Count.ShouldBe(1);
            var coolChild = node.GetElementsByTagName("Parameters");
            coolChild[0].Name.ShouldBe("Parameters");
            var envVars = node.GetElementsByTagName("Environment");

            envVars.Count.ShouldBe(1);

//            coolChild.ChildNodes.Count.ShouldBe(1);
//            coolChild = coolChild.ChildNodes[0];
//            coolChild.Name.ShouldBe("Environment");
//            coolChild.Attributes.GetNamedItem("name").ShouldNotBeNull();
//            coolChild.Attributes.GetNamedItem("name").Value.ShouldBe("ActiveMutant");

        }
    }
}
