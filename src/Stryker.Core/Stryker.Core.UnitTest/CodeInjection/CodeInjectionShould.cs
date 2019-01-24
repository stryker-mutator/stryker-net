using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Compiling;
using Stryker.Core.Coverage;
using Xunit;

namespace Stryker.Core.UnitTest.CodeInjection
{
    public class CodeInjectionShould
    {
        [Fact]
        void CompileInAnEmptyProject()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var system = new List<MetadataReference>();
            foreach (var assembly in assemblies)
            {
                if (!assembly.IsDynamic)
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
            var activeMutation = Environment.GetEnvironmentVariable("ActiveMutation");
            var pipeName = Environment.GetEnvironmentVariable(MutantControl.EnvironmentPipeName);

            using (var server = new CoverageServer("test"))
            {
                var message = string.Empty;
                server.Listen();
                server.RaiseReceivedMessage += (sender, args) => message = args;
                Environment.SetEnvironmentVariable("ActiveMutation", "12");
                Environment.SetEnvironmentVariable(MutantControl.EnvironmentPipeName, server.PipeName);

                MutantControl.InitCoverage();
                Assert.True(MutantControl.IsActive(12));
                Assert.False(MutantControl.IsActive(11));

                MutantControl.DumpState();
                Assert.Equal("12,11,", message);
                
            }

            Environment.SetEnvironmentVariable("ActiveMutation", activeMutation);
            Environment.SetEnvironmentVariable(MutantControl.EnvironmentPipeName, pipeName);
            MutantControl.InitCoverage();
        }
    }
}
