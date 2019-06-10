using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.Initialisation;
using Stryker.Core.InjectedHelpers;
using Stryker.Core.MutationTest;
using Xunit;

namespace Stryker.Core.UnitTest.InjectedHelpers
{
    public class InjectedHelperTests
    { 
        [Fact]
        public void InjectHelpers_ShouldCompile()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var needed = new[] {".CoreLib", ".Runtime", "System.IO.", ".Collections", ".Console" };
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

            // first try compiling
            using (var ms = new MemoryStream())
            {
                // reset the memoryStream
                var emitResult = compilation.Emit(
                    ms,
                    null,
                    win32Resources: compilation.CreateDefaultWin32Resources(
                        versionResource: true, // Important!
                        noManifest: false,
                        manifestContents: null,
                        iconInIcoFormat: null));

                emitResult.Success.ShouldBe(true);
            }
        }


    }
}
