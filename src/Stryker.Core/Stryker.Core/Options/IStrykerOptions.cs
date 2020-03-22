using Microsoft.CodeAnalysis.CSharp;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Reporters;
using Stryker.Core.TestRunners;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Stryker.Core.Options
{
    public interface IStrykerOptions
    {
        int AdditionalTimeoutMS { get; }
        string BasePath { get; }
        int ConcurrentTestrunners { get; }
        DashboardReporterOptions DashboardReporterOptions { get; }
        bool DevMode { get; }
        bool DiffEnabled { get; }
        IEnumerable<Mutator> ExcludedMutations { get; }
        IEnumerable<FilePattern> FilePatterns { get; }
        string GitSource { get; }
        IEnumerable<Regex> IgnoredMethods { get; }
        LanguageVersion LanguageVersion { get; }
        LogOptions LogOptions { get; }
        string OptimizationMode { get; set; }
        OptimizationFlags Optimizations { get; }
        string OutputPath { get; }
        string ProjectUnderTestNameFilter { get; }
        IEnumerable<Reporter> Reporters { get; }
        string SolutionPath { get; }
        IEnumerable<string> TestProjects { get; set; }
        TestRunner TestRunner { get; set; }
        Threshold Thresholds { get; }
    }
}