using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.UnitTest.InjectedHelpers;

/// <summary>
/// MTP runner publishes the active mutant id through a file the injected <see cref="MutantControl"/> reads on every
/// <c>IsActive</c> call while the test host process is reused (stryker-mutator/stryker-net#3832).
/// </summary>
[TestClass]
public class MutantControlMtpMutantFileTests : TestBase
{
    private readonly System.Collections.Generic.List<Assembly> _loadedHelpers = [];

    [TestCleanup]
    public void Cleanup() => DisableLoadedHelpers();

    [TestMethod]
    public void IsActive_ReadsLatestMutantId_WhenRunnerKeepsFileOpenForWrites()
    {
        var assembly = CompileMutantControl("MtpMutantFileAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);

        try
        {
            using (var writer = new FileStream(mutantFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                WriteMutantId(writer, 11);
                InvokeIsActive(assembly, 11).ShouldBeTrue();
                InvokeIsActive(assembly, 10).ShouldBeFalse();

                WriteMutantId(writer, 42);
                InvokeIsActive(assembly, 42).ShouldBeTrue();
                InvokeIsActive(assembly, 11).ShouldBeFalse();
            }
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            ReleaseMutantMapping(assembly);
            if (File.Exists(mutantFile))
            {
                File.Delete(mutantFile);
            }
        }
    }

    [TestMethod]
    public async Task IsActive_ReadsLatestMutantId_UnderConcurrentWrites()
    {
        var assembly = CompileMutantControl("MtpMutantFileConcurrentAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);

        try
        {
            using var writer = new FileStream(mutantFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            if (writer.Length < sizeof(int))
            {
                writer.SetLength(sizeof(int));
            }

            var writeTask = Task.Run(async () =>
            {
                for (var id = 0; id < 200; id++)
                {
                    lock (writer)
                    {
                        WriteMutantId(writer, id);
                    }

                    await Task.Yield();
                }
            });

            var readTask = Task.Run(() =>
            {
                for (var i = 0; i < 500; i++)
                {
                    InvokeIsActive(assembly, -1);
                }
            });

            await Task.WhenAll(writeTask, readTask);
            WriteMutantId(writer, 99);
            InvokeIsActive(assembly, 99).ShouldBeTrue();
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            ReleaseMutantMapping(assembly);
            if (File.Exists(mutantFile))
            {
                File.Delete(mutantFile);
            }
        }
    }

    [TestMethod]
    public void IsActive_StaysCheapOnTheHotPath()
    {
        // IsActive runs at every mutation point of the mutated code, so a per-call file or mutex access
        // turns a sub-second test session into one that exceeds the test timeout.
        const int calls = 100_000;
        var assembly = CompileMutantControl("MtpMutantFilePerfAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);

        try
        {
            using var writer = new FileStream(mutantFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            WriteMutantId(writer, 7);
            InvokeIsActive(assembly, 7).ShouldBeTrue();

            var isActive = GetMutantControl(assembly).GetMethod("IsActive")!.CreateDelegate<Func<int, bool>>();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (var i = 0; i < calls; i++)
            {
                isActive(7).ShouldBeTrue();
            }

            stopwatch.Stop();
            Console.WriteLine($"{calls} IsActive calls took {stopwatch.ElapsedMilliseconds} ms");
            stopwatch.Elapsed.ShouldBeLessThan(TimeSpan.FromSeconds(2));
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            ReleaseMutantMapping(assembly);
            if (File.Exists(mutantFile))
            {
                File.Delete(mutantFile);
            }
        }
    }

    [TestMethod]
    public void IsActive_IsFalse_WhenTheControlFileDoesNotExist()
    {
        var assembly = CompileMutantControl("MtpMutantFileMissingAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);

        try
        {
            InvokeIsActive(assembly, 0).ShouldBeFalse();
            InvokeIsActive(assembly, -1).ShouldBeFalse();
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            ReleaseMutantMapping(assembly);
        }
    }

    [TestMethod]
    public void IsActive_PicksUpTheControlFile_WhenItAppearsAfterTheFirstCall()
    {
        var assembly = CompileMutantControl("MtpMutantFileLateAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);

        try
        {
            InvokeIsActive(assembly, 5).ShouldBeFalse();

            using var writer = new FileStream(mutantFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            WriteMutantId(writer, 5);

            InvokeIsActive(assembly, 5).ShouldBeTrue();
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            ReleaseMutantMapping(assembly);
            if (File.Exists(mutantFile))
            {
                File.Delete(mutantFile);
            }
        }
    }

    [TestMethod]
    public void IsActive_IsFalse_WhenTheControlFileIsShorterThanAnId()
    {
        var assembly = CompileMutantControl("MtpMutantFileShortAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        File.WriteAllBytes(mutantFile, [1, 0]);
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);

        try
        {
            InvokeIsActive(assembly, 1).ShouldBeFalse();
            InvokeIsActive(assembly, 0).ShouldBeFalse();
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            ReleaseMutantMapping(assembly);
            File.Delete(mutantFile);
        }
    }

    [TestMethod]
    public void IsActive_ReadsTheFileDirectly_WhenMemoryMappingIsUnavailable()
    {
        var assembly = CompileMutantControl("MtpMutantFileFallbackAssembly");
        var mutantFile = Path.Combine(Path.GetTempPath(), $"stryker-mutant-test-{Guid.NewGuid():N}.txt");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", mutantFile);
        SetStaticField(GetMutantControl(assembly), "_mutantMmfFailed", true);

        try
        {
            using var writer = new FileStream(mutantFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            WriteMutantId(writer, 9);
            InvokeIsActive(assembly, 9).ShouldBeTrue();
            InvokeIsActive(assembly, 8).ShouldBeFalse();

            WriteMutantId(writer, 10);
            InvokeIsActive(assembly, 10).ShouldBeTrue();
            InvokeIsActive(assembly, 9).ShouldBeFalse();
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
            if (File.Exists(mutantFile))
            {
                File.Delete(mutantFile);
            }
        }
    }

    [TestMethod]
    public void IsActive_UsesTheEnvironmentVariable_WhenNoControlFileIsConfigured()
    {
        var assembly = CompileMutantControl("MtpMutantFileEnvironmentAssembly");
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_FILE", null);
        Environment.SetEnvironmentVariable("STRYKER_MUTANT_ID_CONTROL_VAR", "STRYKER_TEST_ACTIVE_MUTANT");
        Environment.SetEnvironmentVariable("STRYKER_TEST_ACTIVE_MUTANT", "3");

        try
        {
            InvokeIsActive(assembly, 3).ShouldBeTrue();
            InvokeIsActive(assembly, 4).ShouldBeFalse();
        }
        finally
        {
            Environment.SetEnvironmentVariable("STRYKER_MUTANT_ID_CONTROL_VAR", null);
            Environment.SetEnvironmentVariable("STRYKER_TEST_ACTIVE_MUTANT", null);
        }
    }

    private static void ReleaseMutantMapping(Assembly assembly)
    {
        // A live memory-mapped view keeps the control file locked, so release it before deleting the file.
        var mutantControl = GetMutantControl(assembly);
        SetStaticField(mutantControl, "_mutantMmfReady", false);
        SetStaticField(mutantControl, "_mutantMmfFailed", true);
        DisposeStaticField(mutantControl, "_mutantAccessor");
        DisposeStaticField(mutantControl, "_mutantMmf");
    }

    private static void WriteMutantId(FileStream stream, int mutantId)
    {
        stream.Seek(0, SeekOrigin.Begin);
        Span<byte> bytes = stackalloc byte[sizeof(int)];
        BitConverter.TryWriteBytes(bytes, mutantId);
        stream.Write(bytes);
        stream.Flush(flushToDisk: true);
    }

    private static bool InvokeIsActive(Assembly assembly, int mutantId) =>
        (bool)GetMutantControl(assembly).GetMethod("IsActive")!.Invoke(null, [mutantId])!;

    private static Type GetMutantControl(Assembly assembly) =>
        assembly.GetTypes().Single(type => type.Name == "MutantControl");

    private void DisableLoadedHelpers()
    {
        foreach (var mutantControl in _loadedHelpers.Select(GetMutantControl))
        {
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
