using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Shouldly;
using Stryker.Core.InjectedHelpers;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using Xunit;

namespace Stryker.Core.UnitTest.InjectedHelpers
{
    public class InjectedHelperTests : TestBase
    {
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
            var codeInjection = new CodeInjection();

            foreach (var helper in codeInjection.MutantHelpers)
            {
                syntaxes.Add(CSharpSyntaxTree.ParseText(helper.Value, new CSharpParseOptions(languageVersion: version),
                    helper.Key));
            }

            var compilation = CSharpCompilation.Create("dummy.dll",
                syntaxes,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                references: references);

            compilation.GetDiagnostics().ShouldNotContain(diag => diag.Severity == DiagnosticSeverity.Error,
                $"errors :{string.Join(Environment.NewLine, compilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).Select(diag => $"'{diag.GetMessage()}' at {diag.Location.SourceTree.FilePath}, {diag.Location.GetLineSpan().StartLinePosition.Line}:{diag.Location.GetLineSpan().StartLinePosition.Character}"))}");
        }
    }
}
