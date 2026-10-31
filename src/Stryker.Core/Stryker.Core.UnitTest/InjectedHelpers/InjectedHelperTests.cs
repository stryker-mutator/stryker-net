using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.UnitTest.InjectedHelpers;
[TestClass]
public class InjectedHelperTests : TestBase
{
    [TestMethod]
    [DataRow(LanguageVersion.CSharp2)]
    [DataRow(LanguageVersion.CSharp3)]
    [DataRow(LanguageVersion.CSharp4)]
    [DataRow(LanguageVersion.CSharp5)]
    [DataRow(LanguageVersion.CSharp6)]
    [DataRow(LanguageVersion.CSharp7)]
    [DataRow(LanguageVersion.CSharp7_1)]
    [DataRow(LanguageVersion.CSharp7_2)]
    [DataRow(LanguageVersion.CSharp7_3)]
    [DataRow(LanguageVersion.CSharp8)]
    [DataRow(LanguageVersion.CSharp9)]
    [DataRow(LanguageVersion.CSharp10)]
    [DataRow(LanguageVersion.CSharp11)]
    [DataRow(LanguageVersion.CSharp12)]
    [DataRow(LanguageVersion.CSharp13)]
    [DataRow(LanguageVersion.CSharp14)]
    [DataRow(LanguageVersion.Default)]
    [DataRow(LanguageVersion.Latest)]
    [DataRow(LanguageVersion.LatestMajor)]
    [DataRow(LanguageVersion.Preview)]
    public void InjectHelpers_ShouldCompile_ForAllLanguageVersions(LanguageVersion version)
    {
        PerformBasicBuild(new CSharpParseOptions(languageVersion: version), false);
    }

    [TestMethod]
    [DataRow(LanguageVersion.CSharp8)]
    [DataRow(LanguageVersion.CSharp9)]
    [DataRow(LanguageVersion.CSharp10)]
    [DataRow(LanguageVersion.CSharp11)]
    [DataRow(LanguageVersion.CSharp12)]
    [DataRow(LanguageVersion.CSharp13)]
    [DataRow(LanguageVersion.CSharp14)]
    [DataRow(LanguageVersion.Default)]
    [DataRow(LanguageVersion.Latest)]
    [DataRow(LanguageVersion.LatestMajor)]
    [DataRow(LanguageVersion.Preview)]
    public void InjectHelpers_ShouldCompile_ForAllLanguageVersionsWithNullableOptions(LanguageVersion version)
    {
        // MutantControl maps the mutant-id file via MemoryMappedFile for the MTP runner; touch the type
        // so its defining assembly is loaded before the snapshot below and can be referenced.
        _ = typeof(System.IO.MemoryMappedFiles.MemoryMappedFile);

        PerformBasicBuild(new CSharpParseOptions(languageVersion: version), true);
    }

    private static void PerformBasicBuild(CSharpParseOptions cSharpParseOptions, bool nullableContextOptionsEnabled)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        var needed = new[] { ".CoreLib", ".Runtime", "System.IO.MemoryMappedFiles"};
        var references = (from assembly in assemblies where needed.Any(x => assembly.FullName.Contains(x)) select MetadataReference.CreateFromFile(assembly.Location)).Cast<MetadataReference>().ToList();

        var codeInjection = new CodeInjection();

        var syntaxes = codeInjection.MutantHelpers.Select(helper => CSharpSyntaxTree.ParseText(helper.Value, cSharpParseOptions, helper.Key)).ToList();

        var compilation = CSharpCompilation.Create("dummy.dll",
            syntaxes,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: nullableContextOptionsEnabled ? NullableContextOptions.Enable : NullableContextOptions.Disable,
                generalDiagnosticOption: ReportDiagnostic.Error),
            references: references);

        compilation.GetDiagnostics().ShouldNotContain(diag => diag.Severity == DiagnosticSeverity.Error,
            $"errors :{string.Join(Environment.NewLine, compilation.GetDiagnostics().Where(x => x.Severity == DiagnosticSeverity.Error).Select(diag => $"{diag.Id}: '{diag.GetMessage()}' at {diag.Location.SourceTree.FilePath}, {diag.Location.GetLineSpan().StartLinePosition.Line + 1}:{diag.Location.GetLineSpan().StartLinePosition.Character}"))}");
    }
}
