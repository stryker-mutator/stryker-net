using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Stryker.Core.UnitTest.InjectedHelpers
{
    public class InjectedHelperTests
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
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var sourceFile = File.ReadAllText(Path.Combine(currentDirectory, "../../../../Stryker.Core/Mutants/ActiveMutationHelper.cs"));

            var needed = new[] { ".CoreLib", ".Runtime", "System.IO.Pipes", ".Collections", ".Console" };
            var references = new List<MetadataReference>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                if (needed.Any(x => assembly.FullName.Contains(x)))
                {
                    references.Add(MetadataReference.CreateFromFile(assembly.Location));
                }
            }

            var compilation = CSharpCompilation.Create("dummy.dll",
               new List<SyntaxTree> { CSharpSyntaxTree.ParseText(sourceFile, new CSharpParseOptions(languageVersion: version)) },
               options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
               references: references);

            var errors = compilation.GetDiagnostics();
            Assert.False(errors.Any(diag => diag.Severity == DiagnosticSeverity.Error));
        }


    }
}