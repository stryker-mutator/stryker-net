using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.InjectedHelpers;
using Xunit;

namespace Stryker.Core.UnitTest.InjectedHelpers
{
    public class InjectedHelperTests
    { 
        [Fact]
        public void InjectHelpers_ShouldCompile()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            // it would be good to be ensure those assemblies are referenced by the build project
            var needed = new[] {".CoreLib", ".Runtime", "System.IO.Pipes", ".Collections", ".Console" };
            var references = new List<MetadataReference>();
            var hack = new NamedPipeClientStream("test");
            foreach (var assembly in assemblies)
            {
                if (needed.Any(x => assembly.FullName.Contains(x)))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            var compilation = CSharpCompilation.Create("dummy.dll",
                CodeInjection.MutantHelpers,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: references);

            var errors = compilation.GetDiagnostics();
            Assert.False(errors.Any(diag => diag.Severity == DiagnosticSeverity.Error));
        }


    }
}
