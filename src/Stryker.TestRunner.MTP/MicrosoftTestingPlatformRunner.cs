using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions;
using Stryker.Abstractions.Options;
using Stryker.Abstractions.Testing;
using Stryker.TestRunner.Results;
using Stryker.TestRunner.Tests;
using Stryker.Utilities.Logging;

namespace Stryker.TestRunner.MTP;

/// <summary>
/// Microsoft Testing Platform runner implementation.
/// Basic implementation that runs tests using dotnet test with MTP-enabled test projects.
/// </summary>
public class MicrosoftTestingPlatformRunner : ITestRunner
{
    private readonly ILogger _logger;
    private readonly IStrykerOptions _options;
    private readonly Dictionary<string, MtpTestDescription> _discoveredTests = new();
    private readonly Dictionary<string, HashSet<string>> _testsPerAssembly = new();

    public MicrosoftTestingPlatformRunner(IStrykerOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = ApplicationLogging.LoggerFactory.CreateLogger<MicrosoftTestingPlatformRunner>();
        _logger.LogInformation("Microsoft Testing Platform runner initialized. This is an experimental feature.");
    }

    public bool DiscoverTests(string assembly)
    {
        try
        {
            _logger.LogInformation("Discovering tests in assembly: {Assembly}", assembly);
            
            if (!File.Exists(assembly))
            {
                _logger.LogWarning("Assembly not found: {Assembly}", assembly);
                return false;
            }

            // Run dotnet test with list-tests to discover tests
            var result = RunDotnetTest(assembly, "--list-tests", timeout: 30000);
            
            if (result.ExitCode != 0)
            {
                _logger.LogWarning("Test discovery failed for {Assembly} with exit code {ExitCode}", assembly, result.ExitCode);
                return false;
            }

            // Parse the output to extract test names
            var testNames = ParseListTestsOutput(result.Output);
            
            _testsPerAssembly[assembly] = new HashSet<string>();
            
            foreach (var testName in testNames)
            {
                var testId = Guid.NewGuid().ToString();
                var testDesc = new MtpTestDescription(testId, testName, assembly);
                _discoveredTests[testId] = testDesc;
                _testsPerAssembly[assembly].Add(testId);
            }

            _logger.LogInformation("Discovered {Count} tests in {Assembly}", testNames.Count, assembly);
            return testNames.Count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error discovering tests in {Assembly}", assembly);
            return false;
        }
    }

    public ITestSet GetTests(IProjectAndTests project)
    {
        var testIds = new List<string>();
        
        foreach (var assembly in project.GetTestAssemblies())
        {
            if (_testsPerAssembly.TryGetValue(assembly, out var tests))
            {
                testIds.AddRange(tests);
            }
        }

        return new MtpTestSet(_discoveredTests.Where(t => testIds.Contains(t.Key)).Select(t => t.Value));
    }

    public ITestRunResult InitialTest(IProjectAndTests project)
    {
        _logger.LogInformation("Running initial test for project");
        
        try
        {
            var startTime = DateTime.UtcNow;
            var allTests = GetTests(project);
            var failedTests = new List<string>();
            var executedTests = new List<string>();
            var allMessages = new List<string>();

            foreach (var assembly in project.GetTestAssemblies())
            {
                if (!_testsPerAssembly.ContainsKey(assembly))
                {
                    _logger.LogWarning("No tests discovered for assembly: {Assembly}", assembly);
                    continue;
                }

                // Run all tests in the assembly
                var result = RunDotnetTest(assembly, "", timeout: _options.AdditionalTimeout + 60000);
                allMessages.Add(result.Output);
                
                var assemblyTestIds = _testsPerAssembly[assembly];
                executedTests.AddRange(assemblyTestIds);

                if (result.ExitCode != 0)
                {
                    // Parse output to determine which tests failed
                    var failedTestNames = ParseFailedTests(result.Output);
                    foreach (var failedName in failedTestNames)
                    {
                        var failedTest = _discoveredTests.Values.FirstOrDefault(t => t.Name == failedName && assemblyTestIds.Contains(t.Id));
                        if (failedTest != null)
                        {
                            failedTests.Add(failedTest.Id);
                        }
                    }
                }
            }

            var duration = DateTime.UtcNow - startTime;
            var executedTestIds = new TestIdentifierList(executedTests);
            var failedTestIds = new TestIdentifierList(failedTests);
            
            return new TestRunResult(
                _discoveredTests.Values,
                executedTestIds,
                failedTestIds,
                TestIdentifierList.NoTest(),
                failedTests.Any() ? $"{failedTests.Count} test(s) failed" : "All tests passed",
                allMessages,
                duration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error running initial tests");
            return new TestRunResult(false, $"Initial test run failed: {ex.Message}");
        }
    }

    public IEnumerable<ICoverageRunResult> CaptureCoverage(IProjectAndTests project)
    {
        _logger.LogWarning("Coverage capture is not yet fully implemented for MTP runner");
        // Return empty coverage for now
        return Enumerable.Empty<ICoverageRunResult>();
    }

    public ITestRunResult TestMultipleMutants(IProjectAndTests project, ITimeoutValueCalculator timeoutCalc, 
        IReadOnlyList<IMutant> mutants, ITestRunner.TestUpdateHandler update)
    {
        _logger.LogInformation("Testing {Count} mutants", mutants.Count);
        
        try
        {
            var startTime = DateTime.UtcNow;
            var allTests = GetTests(project);
            var failedTests = new List<string>();
            var executedTests = new List<string>();

            foreach (var assembly in project.GetTestAssemblies())
            {
                if (!_testsPerAssembly.ContainsKey(assembly))
                {
                    continue;
                }

                var timeout = timeoutCalc?.DefaultTimeout ?? 60000;
                var result = RunDotnetTest(assembly, "", timeout: timeout);
                
                var assemblyTestIds = _testsPerAssembly[assembly];
                executedTests.AddRange(assemblyTestIds);

                if (result.ExitCode != 0)
                {
                    var failedTestNames = ParseFailedTests(result.Output);
                    foreach (var failedName in failedTestNames)
                    {
                        var failedTest = _discoveredTests.Values.FirstOrDefault(t => t.Name == failedName && assemblyTestIds.Contains(t.Id));
                        if (failedTest != null)
                        {
                            failedTests.Add(failedTest.Id);
                        }
                    }
                }
            }

            var duration = DateTime.UtcNow - startTime;
            var executedTestIds = new TestIdentifierList(executedTests);
            var failedTestIds = new TestIdentifierList(failedTests);
            
            return new TestRunResult(
                _discoveredTests.Values,
                executedTestIds,
                failedTestIds,
                TestIdentifierList.NoTest(),
                failedTests.Any() ? $"{failedTests.Count} test(s) failed" : "All tests passed",
                Array.Empty<string>(),
                duration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing mutants");
            return new TestRunResult(false, $"Mutant testing failed: {ex.Message}");
        }
    }

    private ProcessResult RunDotnetTest(string assembly, string additionalArgs, int timeout)
    {
        var projectDir = Path.GetDirectoryName(assembly);
        var projectFile = Directory.GetFiles(projectDir, "*.csproj").FirstOrDefault();
        
        if (string.IsNullOrEmpty(projectFile))
        {
            _logger.LogError("No project file found in directory: {Directory}", projectDir);
            return new ProcessResult { ExitCode = -1, Output = "No project file found" };
        }

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"test \"{projectFile}\" {additionalArgs} --no-build",
            WorkingDirectory = projectDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        var output = new System.Text.StringBuilder();
        var error = new System.Text.StringBuilder();

        process.OutputDataReceived += (sender, e) => { if (e.Data != null) output.AppendLine(e.Data); };
        process.ErrorDataReceived += (sender, e) => { if (e.Data != null) error.AppendLine(e.Data); };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        if (!process.WaitForExit(timeout))
        {
            _logger.LogWarning("Test process timed out after {Timeout}ms", timeout);
            try { process.Kill(); } catch { }
            return new ProcessResult { ExitCode = -1, Output = "Process timed out", TimedOut = true };
        }

        var allOutput = output.ToString() + error.ToString();
        return new ProcessResult { ExitCode = process.ExitCode, Output = allOutput };
    }

    private List<string> ParseListTestsOutput(string output)
    {
        var tests = new List<string>();
        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            // Test names typically start with namespace or are indented
            if (trimmed.Contains('.') && !trimmed.StartsWith("Test run") && !trimmed.StartsWith("Passed!"))
            {
                tests.Add(trimmed);
            }
        }
        
        return tests;
    }

    private List<string> ParseFailedTests(string output)
    {
        var failedTests = new List<string>();
        var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            if (line.Contains("Failed") || line.Contains("✗") || line.Contains("[FAIL]"))
            {
                // Try to extract test name from the line
                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    if (part.Contains('.') && !part.Contains("ms"))
                    {
                        failedTests.Add(part.Trim());
                        break;
                    }
                }
            }
        }
        
        return failedTests;
    }

    public void Dispose()
    {
        _logger.LogDebug("Disposing MicrosoftTestingPlatformRunner");
        _discoveredTests.Clear();
        _testsPerAssembly.Clear();
    }

    private class ProcessResult
    {
        public int ExitCode { get; set; }
        public string Output { get; set; } = string.Empty;
        public bool TimedOut { get; set; }
    }
}

/// <summary>
/// Test description for MTP tests
/// </summary>
internal class MtpTestDescription : IFrameworkTestDescription
{
    private readonly List<ITestResult> _initialResults = new();
    private int _subCases = 0;

    public MtpTestDescription(string id, string name, string filePath)
    {
        Id = id;
        Name = name;
        Description = new TestDescription(id, name, filePath);
        Case = new MtpTestCase(id, name, filePath);
    }

    public string Id { get; }
    public string Name { get; }
    public ITestDescription Description { get; }
    public TestFrameworks Framework => TestFrameworks.MsTest; // Default to MsTest for now
    public TimeSpan InitialRunTime => TimeSpan.FromTicks(_initialResults.Sum(r => r.Duration.Ticks));
    public int NbSubCases => _subCases;
    public ITestCase Case { get; }

    public void RegisterInitialTestResult(ITestResult result)
    {
        _initialResults.Add(result);
    }

    public void AddSubCase()
    {
        _subCases++;
    }

    public void ClearInitialResult()
    {
        _initialResults.Clear();
        _subCases = 0;
    }
}

/// <summary>
/// Test case for MTP tests
/// </summary>
internal class MtpTestCase : ITestCase
{
    public MtpTestCase(string id, string name, string filePath)
    {
        Id = id;
        Guid = Guid.TryParse(id, out var guid) ? guid : Guid.NewGuid();
        Name = name;
        FullyQualifiedName = name;
        CodeFilePath = filePath;
        Source = filePath;
        Uri = new Uri("executor://MicrosoftTestingPlatform/v1");
        LineNumber = 0;
    }

    public string Id { get; }
    public Guid Guid { get; }
    public string Name { get; }
    public string Source { get; }
    public string CodeFilePath { get; }
    public string FullyQualifiedName { get; }
    public Uri Uri { get; }
    public int LineNumber { get; }
}

/// <summary>
/// Test set for MTP tests
/// </summary>
internal class MtpTestSet : ITestSet
{
    private readonly Dictionary<string, ITestDescription> _tests = new();

    public MtpTestSet(IEnumerable<IFrameworkTestDescription> tests)
    {
        foreach (var test in tests)
        {
            _tests[test.Id] = test.Description;
        }
    }

    public int Count => _tests.Count;
    
    public ITestDescription this[string id] => _tests.TryGetValue(id, out var test) ? test : throw new KeyNotFoundException($"Test with id {id} not found");

    public void RegisterTests(IEnumerable<ITestDescription> tests)
    {
        foreach (var test in tests)
        {
            _tests[test.Id] = test;
        }
    }

    public void RegisterTest(ITestDescription test)
    {
        _tests[test.Id] = test;
    }

    public IEnumerable<ITestDescription> Extract(IEnumerable<string> ids)
    {
        return ids.Select(id => _tests.TryGetValue(id, out var test) ? test : null)
                  .Where(t => t != null)!;
    }
}
