using System.IO.Abstractions;
using System.Text.Json;
using System.Text.Json.Nodes;
using Buildalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Stryker.Abstractions.Exceptions;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Fsharp;

/// <summary>
/// Compiles the C# mutant-control helpers into a standalone assembly: F# cannot host C# sources, and the
/// runners find <c>&lt;ns&gt;.MutantControl</c> by reflection over loaded assemblies, wherever it lives.
/// </summary>
internal static class FsharpHelperAssembly
{
    public static string FileName(CodeInjection injector) => $"{injector.HelperNamespace}.StrykerHelpers.dll";

    public static byte[] Compile(CodeInjection injector, IAnalyzerResult sourceProject)
    {
        var references = sourceProject.References
            .Where(File.Exists)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path));
        var compilation = CSharpCompilation.Create(
            $"{injector.HelperNamespace}.StrykerHelpers",
            injector.GetHelpersSyntaxTreesToInject(CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest))
                .Append(StaticValueTrackerTree(injector, CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest))),
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        if (!result.Success)
        {
            var errors = string.Join(Environment.NewLine, result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            throw new GeneralStrykerException($"Failed to compile the Stryker helper assembly:{Environment.NewLine}{errors}");
        }

        return stream.ToArray();
    }

    /// <summary>
    /// <c>MutantContext</c> is internal to the helper assembly; this public entry point lets mutated F# code
    /// use <c>TrackValue</c> to mark the evaluation of module-level values as static.
    /// </summary>
    private static SyntaxTree StaticValueTrackerTree(CodeInjection injector, CSharpParseOptions options) =>
        CSharpSyntaxTree.ParseText(
            $"namespace {injector.HelperNamespace} {{ public static class StaticValueTracker {{ public static T Track<T>(System.Func<T> builder) => MutantContext.TrackValue(builder); }} }}",
            options, path: "StaticValueTracker.cs");

    /// <summary>
    /// Lists the helper assembly in every <c>*.deps.json</c> of a test output directory. Hosts that have a
    /// dependency manifest only load listed assemblies, so a helper that is merely copied next to the mutated
    /// assembly would not be found when the mutated code first references it.
    /// </summary>
    public static void RegisterInDependencyManifests(IFileSystem fileSystem, string directory, string helperFileName)
    {
        var name = fileSystem.Path.GetFileNameWithoutExtension(helperFileName);
        var key = $"{name}/1.0.0";
        foreach (var manifest in fileSystem.Directory.GetFiles(directory, "*.deps.json"))
        {
            if (JsonNode.Parse(fileSystem.File.ReadAllText(manifest)) is not JsonObject root)
            {
                continue;
            }

            foreach (var target in (root["targets"] as JsonObject)?.Select(t => t.Value).OfType<JsonObject>() ?? [])
            {
                target[key] = new JsonObject { ["runtime"] = new JsonObject { [helperFileName] = new JsonObject() } };
            }

            var libraries = root["libraries"] as JsonObject ?? [];
            libraries[key] = new JsonObject { ["type"] = "reference", ["serviceable"] = false, ["sha512"] = string.Empty };
            root["libraries"] = libraries;
            fileSystem.File.WriteAllText(manifest, root.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}
