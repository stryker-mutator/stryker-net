using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Core.InjectedHelpers;

namespace Stryker.Core.UnitTest.InjectedHelpers;

/// <summary>
/// Covers the coverage file the injected <c>MutantControl</c> writes for the MTP runner.
/// A test host loads one copy of the helper per mutated assembly, and every copy reads the same
/// STRYKER_COVERAGE_FILE environment variable, so the copies must not write to the same file.
/// The helpers are compiled and loaded here (once per assembly name) to reproduce that situation.
/// </summary>
[TestClass]
public class MutantControlCoverageFileTests : TestBase
{
    private const string CoverageFileEnvironmentVariable = "STRYKER_COVERAGE_FILE";
    private const string EpochFileEnvironmentVariable = "STRYKER_COVERAGE_EPOCH_FILE";

    private readonly List<Assembly> _loadedHelpers = new();

    [TestMethod]
    public void DisableLoadedHelpers_ShouldStopAHelperFromWritingItsCoverageFileAgain()
    {
        // Assembly.Load keeps a helper in the default load context for the life of this process, and its
        // static constructor registered a process-exit flush against the path it cached then. Deleting the
        // file is not enough: the handler would write it again when the test process exits, leaving a file
        // per test run behind. Cleanup has to disarm the helper, which is what this asserts.
        var coverageFileName = $"stryker-coverage-test-{Guid.NewGuid():N}.txt";
        var previousEnvironmentValue = Environment.GetEnvironmentVariable(CoverageFileEnvironmentVariable);

        try
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, coverageFileName);

            var assembly = CompileMutantControl("DisarmedAssembly");
            RegisterCoverage(assembly, 3);
            FlushCoverage(assembly);
            FindCoverageFiles(coverageFileName).Count.ShouldBe(1, "setup: the helper should have written its file");

            DisableLoadedHelpers();
            foreach (var file in FindCoverageFiles(coverageFileName))
            {
                File.Delete(file);
            }

            // Stands in for the process-exit handler, which calls exactly this
            FlushCoverage(assembly);

            FindCoverageFiles(coverageFileName).ShouldBeEmpty(
                "a disarmed helper must not write its coverage file again");
        }
        finally
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, previousEnvironmentValue);
            DisableLoadedHelpers();
            foreach (var file in FindCoverageFiles(coverageFileName))
            {
                File.Delete(file);
            }
        }
    }

    [TestMethod]
    public void FlushCoverage_ShouldWriteOneFilePerMutatedAssembly_WhenSeveralShareATestHost()
    {
        var coverageFileName = $"stryker-coverage-test-{Guid.NewGuid():N}.txt";
        var previousEnvironmentValue = Environment.GetEnvironmentVariable(CoverageFileEnvironmentVariable);

        try
        {
            // The runner passes the file name, the helper resolves it against the temp directory
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, coverageFileName);

            var firstAssembly = CompileMutantControl("MutatedAssemblyOne");
            var secondAssembly = CompileMutantControl("MutatedAssemblyTwo");

            // In coverage mode IsActive registers the mutant instead of activating it
            RegisterCoverage(firstAssembly, 1);
            RegisterCoverage(secondAssembly, 2);

            FlushCoverage(firstAssembly);
            FlushCoverage(secondAssembly);

            var writtenFiles = FindCoverageFiles(coverageFileName);

            writtenFiles.Count.ShouldBe(2,
                "each mutated assembly must write its own coverage file, otherwise the last flush overwrites the others");

            var contents = writtenFiles.Select(File.ReadAllText).ToList();
            contents.ShouldContain(content => ParseCoveredMutants(content).Contains(1),
                "the coverage of the first mutated assembly must survive");
            contents.ShouldContain(content => ParseCoveredMutants(content).Contains(2),
                "the coverage of the second mutated assembly must survive");
        }
        finally
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, previousEnvironmentValue);
            DisableLoadedHelpers();
            foreach (var file in FindCoverageFiles(coverageFileName))
            {
                File.Delete(file);
            }
        }
    }

    [TestMethod]
    public void FlushCoverage_ShouldWriteAFileTheRunnerCanFind_WhenASingleAssemblyIsMutated()
    {
        var coverageFileName = $"stryker-coverage-test-{Guid.NewGuid():N}.txt";
        var previousEnvironmentValue = Environment.GetEnvironmentVariable(CoverageFileEnvironmentVariable);

        try
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, coverageFileName);

            var assembly = CompileMutantControl("SingleMutatedAssembly");
            RegisterCoverage(assembly, 7);
            FlushCoverage(assembly);

            var writtenFiles = FindCoverageFiles(coverageFileName);

            writtenFiles.Count.ShouldBe(1);
            // The runner looks for files whose name starts with the name it handed out, minus the
            // extension, and ends with that extension
            var writtenFileName = Path.GetFileName(writtenFiles[0]);
            writtenFileName.ShouldStartWith(Path.GetFileNameWithoutExtension(coverageFileName));
            writtenFileName.ShouldEndWith(Path.GetExtension(coverageFileName));
            ParseCoveredMutants(File.ReadAllText(writtenFiles[0])).ShouldContain(7);
        }
        finally
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, previousEnvironmentValue);
            DisableLoadedHelpers();
            foreach (var file in FindCoverageFiles(coverageFileName))
            {
                File.Delete(file);
            }
        }
    }

    [TestMethod]
    public void EpochRelay_ShouldFlushEveryMutatedAssemblyIndependently_WhenSeveralShareATestHost()
    {
        // Per-test coverage: after each test the runner bumps the request half of the epoch relay and
        // waits for the ack half before reading coverage. Each mutated assembly's copy of the helper runs
        // its own relay, so each needs its own relay file: with one shared file the first copy to ack
        // releases the runner while the other copy has not flushed, and that assembly's coverage for the
        // test ends up attributed to the next one.
        var coverageFileName = $"stryker-coverage-test-{Guid.NewGuid():N}.txt";
        var epochFileName = $"stryker-epoch-test-{Guid.NewGuid():N}.txt";
        var previousCoverage = Environment.GetEnvironmentVariable(CoverageFileEnvironmentVariable);
        var previousEpoch = Environment.GetEnvironmentVariable(EpochFileEnvironmentVariable);

        try
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, coverageFileName);
            Environment.SetEnvironmentVariable(EpochFileEnvironmentVariable, epochFileName);

            var firstAssembly = CompileMutantControl("EpochAssemblyOne");
            var secondAssembly = CompileMutantControl("EpochAssemblyTwo");

            RegisterCoverage(firstAssembly, 11);
            RegisterCoverage(secondAssembly, 22);

            var relayFiles = FindCoverageFiles(epochFileName);
            relayFiles.Count.ShouldBe(2,
                "each mutated assembly must relay through its own epoch file so the runner can wait for all of them");
            relayFiles.ShouldNotContain(Path.Combine(Path.GetTempPath(), epochFileName),
                "the path handed out by the runner is not itself a relay");

            // Stand in for the runner: request epoch 1 from every relay, then wait for all acks
            foreach (var relayFile in relayFiles)
            {
                WriteEpoch(relayFile, request: 1);
            }

            WaitUntil(() => relayFiles.All(relay => ReadEpochAck(relay) == 1), TimeSpan.FromSeconds(10))
                .ShouldBeTrue("every relay should acknowledge the epoch it was asked to flush");

            var coverageByFile = FindCoverageFiles(coverageFileName)
                .Select(file => ParseCoveredMutants(File.ReadAllText(file)))
                .ToList();

            coverageByFile.ShouldContain(covered => covered.Contains(11));
            coverageByFile.ShouldContain(covered => covered.Contains(22));
        }
        finally
        {
            Environment.SetEnvironmentVariable(CoverageFileEnvironmentVariable, previousCoverage);
            Environment.SetEnvironmentVariable(EpochFileEnvironmentVariable, previousEpoch);
            // Releases the relay threads' mapping on the epoch files, so they can be deleted below
            DisableLoadedHelpers();
            foreach (var file in FindCoverageFiles(coverageFileName).Concat(FindCoverageFiles(epochFileName)))
            {
                File.Delete(file);
            }
        }
    }

    private static bool WaitUntil(Func<bool> condition, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            if (condition())
            {
                return true;
            }
            Thread.Sleep(5);
        }

        return condition();
    }

    private static void WriteEpoch(string relayFilePath, int request)
    {
        using var stream = new FileStream(relayFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        using var mmf = MemoryMappedFile.CreateFromFile(stream, null, 8, MemoryMappedFileAccess.ReadWrite, HandleInheritability.None, leaveOpen: true);
        using var accessor = mmf.CreateViewAccessor(0, 8, MemoryMappedFileAccess.ReadWrite);
        accessor.Write(0, request);
        accessor.Flush();
    }

    private static int ReadEpochAck(string relayFilePath)
    {
        using var stream = new FileStream(relayFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var mmf = MemoryMappedFile.CreateFromFile(stream, null, 8, MemoryMappedFileAccess.Read, HandleInheritability.None, leaveOpen: true);
        using var accessor = mmf.CreateViewAccessor(0, 8, MemoryMappedFileAccess.Read);
        return accessor.ReadInt32(4);
    }

    /// <summary>
    /// Disarms every helper this test loaded, and is also called from each test before it deletes files.
    /// <see cref="Assembly.Load(byte[])"/> keeps a helper in the default load context for the life of this
    /// process, so a helper outlives the test that loaded it: its static constructor registered a
    /// process-exit flush against the coverage path it cached then, and its relay thread holds a memory
    /// mapping on its epoch file. Left alone, the exit handler rewrites a coverage file the test already
    /// deleted, and the mapping keeps the epoch file undeletable, so every run would leave files behind.
    /// Clearing the cached paths makes the flush a no-op, and releasing the mapping frees the epoch file.
    /// </summary>
    [TestCleanup]
    public void DisableLoadedHelpers()
    {
        foreach (var mutantControl in _loadedHelpers.Select(GetMutantControl))
        {
            // Stop the relay thread from touching the mapping before it is released; it swallows the
            // exceptions of a disposed accessor, but there is no reason to make it race for them
            SetStaticField(mutantControl, "_epochMmfReady", false);
            SetStaticField(mutantControl, "_epochMmfFailed", true);
            DisposeStaticField(mutantControl, "_epochAccessor");
            DisposeStaticField(mutantControl, "_epochMmf");

            // FlushCoverageToFile returns without writing when it has no path, which is what the
            // process-exit handler ends up calling
            SetStaticField(mutantControl, "_cachedCoverageFilePath", string.Empty);
            SetStaticField(mutantControl, "_cachedEpochFilePath", string.Empty);
        }

        _loadedHelpers.Clear();
    }

    private static void SetStaticField(Type mutantControl, string name, object value) =>
        mutantControl.GetField(name, BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, value);

    private static void DisposeStaticField(Type mutantControl, string name)
    {
        var field = mutantControl.GetField(name, BindingFlags.NonPublic | BindingFlags.Static)!;
        (field.GetValue(null) as IDisposable)?.Dispose();
        // The helper roots these with a plain object sentinel rather than null
        field.SetValue(null, new object());
    }

    private static List<string> FindCoverageFiles(string coverageFileName) =>
        Directory.GetFiles(
                Path.GetTempPath(),
                Path.GetFileNameWithoutExtension(coverageFileName) + "*" + Path.GetExtension(coverageFileName))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();

    private static IReadOnlyCollection<int> ParseCoveredMutants(string content) =>
        content.Split(';')[0]
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToList();

    private static void RegisterCoverage(Assembly assembly, int mutantId) =>
        GetMutantControl(assembly).GetMethod("IsActive")!.Invoke(null, new object[] { mutantId });

    private static void FlushCoverage(Assembly assembly) =>
        GetMutantControl(assembly).GetMethod("FlushCoverageToFile")!.Invoke(null, null);

    private static Type GetMutantControl(Assembly assembly) =>
        assembly.GetTypes().Single(type => type.Name == "MutantControl");

    /// <summary>
    /// Compiles the injected helpers into their own assembly, as Stryker does for every mutated
    /// project, and loads it. Each assembly gets its own copy of the helper statics.
    /// </summary>
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

        // Tracked so cleanup can disarm it: this assembly stays in the default load context for the life
        // of the process, and its statics outlive the test that loaded it (see DisableLoadedHelpers)
        var helper = Assembly.Load(peStream.ToArray());
        _loadedHelpers.Add(helper);

        return helper;
    }
}
