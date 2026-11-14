using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.UnitTest.InjectedHelpers;

/// <summary>
/// The reached-mutant relay file tells the MTP runner whether the active mutant ever executed in the
/// test host, so a survivor that reused-host state hid can be retested on a fresh host.
/// </summary>
[TestClass]
public class MutantControlReachedFileTests : TestBase
{
    private readonly System.Collections.Generic.List<Assembly> _loadedHelpers = [];

    [TestCleanup]
    public void Cleanup() => DisableLoadedHelpers();

    [TestMethod]
    public void IsActive_WritesTheReachedFlagAndIdOnce()
    {
        var assembly = CompileMutantControl("ReachedFileAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        var reachedFile = Path.Combine(Path.GetTempPath(), $"stryker-reached-test-{Guid.NewGuid():N}.txt");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);
        Environment.SetEnvironmentVariable("STRYKER_REACHED_FILE", reachedFile);

        try
        {
            // Create and size both relay files the way the runner does before the host starts.
            using (var creator = new FileStream(reachedFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                creator.SetLength(2 * sizeof(int));
            }

            using (var writer = new FileStream(mutantFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                writer.SetLength(sizeof(int));

                WriteMutantId(writer, 7);
                InvokeIsActive(assembly, 7).ShouldBeTrue();

                ReadReachedFlag(reachedFile).ShouldBe(1, "the active mutant that executed must be reported as reached");
                ReadReachedMutantId(reachedFile).ShouldBe(7);

                // Repeated IsActive calls for the same mutant must not touch the file again.
                WriteReachedFlag(reachedFile, 0);
                InvokeIsActive(assembly, 7).ShouldBeTrue();
                ReadReachedFlag(reachedFile).ShouldBe(0, "the signal is written at most once per mutant id");

                WriteReachedFlag(reachedFile, 1);

                WriteMutantId(writer, 8);
                InvokeIsActive(assembly, 8).ShouldBeTrue();
                ReadReachedMutantId(reachedFile).ShouldBe(8);
                InvokeIsActive(assembly, 7).ShouldBeFalse();
                ReadReachedMutantId(reachedFile).ShouldBe(8, "a mutant that did not execute must not overwrite the signal");
            }
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            Environment.SetEnvironmentVariable("STRYKER_REACHED_FILE", null);
            ReleaseReachedMapping(assembly);
            ReleaseMutantMapping(assembly);
            foreach (var file in new[] { mutantFile, reachedFile })
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
    }

    [TestMethod]
    public void IsActive_LeavesNoSignal_WhenNoReachedFileIsConfigured()
    {
        var assembly = CompileMutantControl("ReachedFileAbsentAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        File.WriteAllBytes(mutantFile, BitConverter.GetBytes(7));
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);
        Environment.SetEnvironmentVariable("STRYKER_REACHED_FILE", null);

        try
        {
            // Must not throw and must stay functional without the optional relay file.
            InvokeIsActive(assembly, 7).ShouldBeTrue();
            InvokeIsActive(assembly, 8).ShouldBeFalse();
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            ReleaseReachedMapping(assembly);
            ReleaseMutantMapping(assembly);
            if (File.Exists(mutantFile))
            {
                File.Delete(mutantFile);
            }
        }
    }

    [TestMethod]
    public void IsActive_WritesTheReachedSignalWithoutMemoryMapping_WhenMappingIsUnavailable()
    {
        var assembly = CompileMutantControl("ReachedFileFallbackAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        var reachedFile = Path.Combine(Path.GetTempPath(), $"stryker-reached-test-{Guid.NewGuid():N}.txt");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);
        Environment.SetEnvironmentVariable("STRYKER_REACHED_FILE", reachedFile);
        SetStaticField(GetMutantControl(assembly), "_reachedMmfFailed", true);

        try
        {
            File.WriteAllBytes(mutantFile, BitConverter.GetBytes(7));
            // The runner creates and sizes the relay file before the host starts; mirror that here.
            using (var creator = new FileStream(reachedFile, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                creator.SetLength(2 * sizeof(int));
            }

            InvokeIsActive(assembly, 7).ShouldBeTrue();

            // The plain-file fallback writes the same binary pair as the mapping (flag, mutant id).
            using (var reader = new FileStream(reachedFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var bytes = new byte[2 * sizeof(int)];
                var read = reader.Read(bytes, 0, bytes.Length);
                read.ShouldBe(2 * sizeof(int));
                BitConverter.ToInt32(bytes, 0).ShouldBe(1, "the fallback writes the reached flag");
                BitConverter.ToInt32(bytes, 4).ShouldBe(7, "the fallback writes the reached mutant id");
            }
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            Environment.SetEnvironmentVariable("STRYKER_REACHED_FILE", null);
            ReleaseMutantMapping(assembly);
            foreach (var file in new[] { mutantFile, reachedFile })
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
        }
    }

    private static void ReleaseMutantMapping(Assembly assembly)
    {
        var mutantControl = GetMutantControl(assembly);
        SetStaticField(mutantControl, "_mutantMmfReady", false);
        SetStaticField(mutantControl, "_mutantMmfFailed", true);
        DisposeStaticField(mutantControl, "_mutantAccessor");
        DisposeStaticField(mutantControl, "_mutantMmf");
        SetStaticField(mutantControl, "_cachedMutantFilePath", string.Empty);
        SetStaticField(mutantControl, "_mutantFilePathCached", false);
        SetStaticField(mutantControl, "_reportedReachedMutant", int.MinValue);
    }

    private static void ReleaseReachedMapping(Assembly assembly)
    {
        var mutantControl = GetMutantControl(assembly);
        SetStaticField(mutantControl, "_reachedMmfReady", false);
        SetStaticField(mutantControl, "_reachedMmfFailed", true);
        DisposeStaticField(mutantControl, "_reachedAccessor");
        DisposeStaticField(mutantControl, "_reachedMmf");
        SetStaticField(mutantControl, "_cachedReachedFilePath", string.Empty);
        SetStaticField(mutantControl, "_reachedFilePathCached", false);
        SetStaticField(mutantControl, "_reportedReachedMutant", int.MinValue);
    }

    private static int ReadReachedFlag(string path) => ReadReachedInt(path, 0);

    private static int ReadReachedMutantId(string path) => ReadReachedInt(path, sizeof(int));

    private static int ReadReachedInt(string path, int offset)
    {
        // The mapping the host holds keeps the file open for read and write, so every observer opens it shared.
        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        var bytes = new byte[2 * sizeof(int)];
        var read = 0;
        while (read < bytes.Length)
        {
            var chunk = stream.Read(bytes, read, bytes.Length - read);
            if (chunk == 0)
            {
                break;
            }

            read += chunk;
        }

        read.ShouldBeGreaterThanOrEqualTo(2 * sizeof(int));
        return BitConverter.ToInt32(bytes, offset);
    }

    private static void WriteReachedFlag(string path, int flag)
    {
        using var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        stream.Seek(0, SeekOrigin.Begin);
        var bytes = BitConverter.GetBytes(flag);
        stream.Write(bytes, 0, bytes.Length);
        stream.Flush();
    }

    private static void WriteMutantId(FileStream stream, int mutantId)
    {
        stream.Seek(0, SeekOrigin.Begin);
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        BitConverter.TryWriteBytes(bytes, mutantId);
        stream.Write(bytes);
        stream.Flush();
    }

    private static bool InvokeIsActive(Assembly assembly, int mutantId) =>
        (bool)GetMutantControl(assembly).GetMethod("IsActive")!.Invoke(null, [mutantId])!;

    private static Type GetMutantControl(Assembly assembly) =>
        assembly.GetTypes().Single(type => type.Name == "MutantControl");

    private void DisableLoadedHelpers()
    {
        foreach (var mutantControl in _loadedHelpers.Select(GetMutantControl))
        {
            SetStaticField(mutantControl, "_reachedMmfReady", false);
            SetStaticField(mutantControl, "_reachedMmfFailed", true);
            DisposeStaticField(mutantControl, "_reachedAccessor");
            DisposeStaticField(mutantControl, "_reachedMmf");
            SetStaticField(mutantControl, "_cachedReachedFilePath", string.Empty);
            SetStaticField(mutantControl, "_reachedFilePathCached", false);
            SetStaticField(mutantControl, "_reportedReachedMutant", int.MinValue);
            SetStaticField(mutantControl, "_epochMmfReady", false);
            SetStaticField(mutantControl, "_epochMmfFailed", true);
            DisposeStaticField(mutantControl, "_epochAccessor");
            DisposeStaticField(mutantControl, "_epochMmf");
            SetStaticField(mutantControl, "_cachedCoverageFilePath", string.Empty);
            SetStaticField(mutantControl, "_cachedEpochFilePath", string.Empty);
            SetStaticField(mutantControl, "_cachedMutantFilePath", string.Empty);
            SetStaticField(mutantControl, "_mutantFilePathCached", false);
        }

        _loadedHelpers.Clear();
    }

    private static void SetStaticField(Type mutantControl, string name, object value) =>
        mutantControl.GetField(name, BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, value);

    private static void DisposeStaticField(Type mutantControl, string name)
    {
        var field = mutantControl.GetField(name, BindingFlags.NonPublic | BindingFlags.Static);
        if (field?.GetValue(null) is IDisposable disposable)
        {
            disposable.Dispose();
        }

        field?.SetValue(null, null);
    }

    private Assembly CompileMutantControl(string assemblyName)
    {
        var codeInjection = new CodeInjection();
        var syntaxTrees = codeInjection.MutantHelpers
            .Select(helper => CSharpSyntaxTree.ParseText(helper.Value, path: helper.Key))
            .ToList();

        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
            .Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries)
            .Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))
            .ToList();

        var compilation = CSharpCompilation.Create(assemblyName,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var peStream = new MemoryStream();
        var result = compilation.Emit(peStream);
        result.Success.ShouldBeTrue(
            $"the injected helpers should compile: {string.Join(Environment.NewLine, result.Diagnostics.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error))}");

        var helper = Assembly.Load(peStream.ToArray());
        _loadedHelpers.Add(helper);
        return helper;
    }
}
