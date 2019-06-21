using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.InjectedHelpers;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
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
            var needed = new[] { ".CoreLib", ".Runtime", "System.IO.Pipes", ".Collections", ".Console" };
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

        [Theory]
        [InlineData(LanguageVersion.CSharp2)]
        [InlineData(LanguageVersion.CSharp3)]
        [InlineData(LanguageVersion.CSharp4)]
        [InlineData(LanguageVersion.CSharp5)]
        [InlineData(LanguageVersion.CSharp6)]
        [InlineData(LanguageVersion.CSharp7)]
        [InlineData(LanguageVersion.CSharp7_1)]
        [InlineData(LanguageVersion.CSharp7_2)]
        [InlineData(LanguageVersion.CSharp7_3)]
        [InlineData(LanguageVersion.CSharp8)]
        [InlineData(LanguageVersion.Default)]
        [InlineData(LanguageVersion.Latest)]
        [InlineData(LanguageVersion.LatestMajor)]
        [InlineData(LanguageVersion.Preview)]
        public void InjectHelpers_ShouldCompile_ForAllLanguageVersions(LanguageVersion version)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var needed = new[] { ".CoreLib", ".Runtime", "System.IO.Pipes", ".Collections", ".Console" };
            var references = new List<MetadataReference>();
            var hack = new NamedPipeClientStream("test");
            foreach (var assembly in assemblies)
            {
                if (needed.Any(x => assembly.FullName.Contains(x)))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            var syntaxes = new List<SyntaxTree>();

            foreach (var syntax in CodeInjection.MutantHelpers)
            {
                syntaxes.Add(CSharpSyntaxTree.ParseText(syntax.GetText(), new CSharpParseOptions(languageVersion: version)));
            }

            var compilation = CSharpCompilation.Create("dummy.dll",
                syntaxes,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: references);

            var errors = compilation.GetDiagnostics();
            Assert.False(errors.Any(diag => diag.Severity == DiagnosticSeverity.Error), string.Join("\n", errors.Where(diag => diag.Severity == DiagnosticSeverity.Error).Select(e => e.GetMessage())));
        }
    }
}
