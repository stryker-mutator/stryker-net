using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.DataCollector;
using Xunit;

namespace Stryker.Core.UnitTest.CodeInjection
{
    public class CodeInjectionShould
    {
        // this lock must be taken to ensure tests are seralized, as MutantControl must be a static class (for injection in tested assembly)
        private static object serializer = new object();
        [Fact]
        void CompileInAnEmptyProject()
        {
            // make sure the required assemblies are loaded
            using (var server = new CommunicationServer("test"))
            {
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var system = new List<MetadataReference>();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic && assembly.FullName.Contains("System"))
                {
                    system.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }
            var injected = Core.Compiling.CodeInjection.MutantHelpers;
            var compiler = CSharpCompilation.Create("pseudo.dll",
                syntaxTrees: injected,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, allowUnsafe: true), 
                references: system);

            // first try compiling
            var ms = new MemoryStream();
            var emitResult = compiler.Emit(ms);
            Assert.True(emitResult.Success);
        }

        [Fact]
        void IdentifyCoveredMutant()
        {
            lock (serializer)
            {
                var activeMutation = Environment.GetEnvironmentVariable("ActiveMutation");
                var pipeName = Environment.GetEnvironmentVariable(MutantControl.EnvironmentPipeName);

                if (!string.IsNullOrEmpty(activeMutation) || !string.IsNullOrEmpty(pipeName))
                {
                    // not reentrant
                    return;
                }

                var lck = new object();

                using (var server = new CommunicationServer("test"))
                {
                    var message = string.Empty;
                    server.Listen();
                    server.RaiseReceivedMessage += (sender, args) =>
                    {
                        lock (lck)
                        {
                            message = args;
                            Monitor.Pulse(lck);
                        }
                    };
                    Environment.SetEnvironmentVariable("ActiveMutation", "12");
                    Environment.SetEnvironmentVariable(MutantControl.EnvironmentPipeName, server.PipeName);

                    MutantControl.InitCoverage();
                    Assert.True(MutantControl.IsActive(12));
                    Assert.False(MutantControl.IsActive(11));
                    MutantControl.DumpState();
                    WaitOnLck(lck, () => !string.IsNullOrEmpty(message), 100);

                    Assert.Equal("12,11", message);
                }

                Environment.SetEnvironmentVariable("ActiveMutation", activeMutation);
                Environment.SetEnvironmentVariable(MutantControl.EnvironmentPipeName, pipeName);
                MutantControl.InitCoverage();
            }
        }

        static bool WaitOnLck(object lck, Func<bool> predicate, int timeout)
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

        [Fact]
        void IdentifyMutantsPerTest()
        {
            lock (serializer)
            {
                var activeMutation = Environment.GetEnvironmentVariable("ActiveMutation");
                var pipeName = Environment.GetEnvironmentVariable(MutantControl.EnvironmentPipeName);
                if (!string.IsNullOrEmpty(activeMutation) || !string.IsNullOrEmpty(pipeName))
                {
                    // not reentrant
                    return;
                }

                var lck = new object();

                using (var server = new CommunicationServer("test"))
                {
                    string message = null;
                    CommunicationChannel testProcess = null;
                    server.RaiseReceivedMessage += (sender, args) =>
                    {
                        lock (lck)
                        {
                            message = args;
                            Monitor.Pulse(lck);
                        }
                    };
                    server.RaiseNewClientEvent += (o, args) =>
                    {
                        lock (lck)
                        {
                            testProcess = args.Client;
                            Monitor.Pulse(lck);
                        }
                    };
                    server.Listen();
                    Environment.SetEnvironmentVariable(MutantControl.EnvironmentPipeName, server.PipeName);
                    testProcess = null;
                    MutantControl.InitCoverage();
                    WaitOnLck(lck, () => testProcess != null, 400);

                    Assert.NotNull(testProcess);

                    Assert.False(MutantControl.IsActive(12));
                    testProcess.SendText("DUMP");

                    WaitOnLck(lck, () => !string.IsNullOrEmpty(message), 200);

                    Assert.NotNull(message);
                    Assert.Equal("12", message);
                    message = string.Empty;
                    Assert.False(MutantControl.IsActive(11));
                    MutantControl.DumpState();

                    WaitOnLck(lck, () => !string.IsNullOrEmpty(message), 200);

                    Assert.NotNull(message);
                    Assert.Equal("11", message);
                }

                Environment.SetEnvironmentVariable("ActiveMutation", activeMutation);
                Environment.SetEnvironmentVariable(MutantControl.EnvironmentPipeName, pipeName);
                MutantControl.InitCoverage();
            }
        }
    }
}
