using System.Text.RegularExpressions;
using DotNet.Globbing;
using Microsoft.CodeAnalysis.CSharp;
using Serilog.Events;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Logging;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class StrykerOptionsTests
    {
        [Fact]
        public void ShouldCopyValues()
        {
            var target = new StrykerOptions()
            {
                AdditionalTimeoutMS = 1,
                AzureFileStorageSas = "sas",
                AzureFileStorageUrl = "url",
                BaselineProvider = Core.Baseline.Providers.BaselineProvider.AzureFileStorage,
                BasePath = "C:/",
                Concurrency = 4,
                DashboardApiKey = "key",
                DashboardUrl = "url",
                DevMode = true,
                Since = true,
                DiffIgnoreFilePatterns = new[] { new FilePattern(Glob.Parse("**"), true, null) },
                ExcludedMutations = new[] { Mutator.Bitwise },
                FallbackVersion = "main",
                IgnoredMethods = new[] { new Regex("") },
                LanguageVersion = LanguageVersion.Latest,
                LogOptions = new LogOptions
                {
                    LogLevel = LogEventLevel.Verbose,
                    LogToFile = true
                },
                ModuleName = "module",
                Mutate = new[] { new FilePattern(Glob.Parse("**"), true, null) },
                MutationLevel = MutationLevel.Complete,
                OptimizationMode = OptimizationModes.DisableBail,
                OutputPath = "output",
                ProjectName = "name",
                ProjectUnderTestName = "project",
                ProjectVersion = "version"
            };

            var result = target.Copy("C://Dev//Test", "test", new[] { "project1" });

            result.AdditionalTimeoutMS.ShouldBe(1);
        }
    }
}
