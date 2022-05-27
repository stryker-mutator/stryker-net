using System.Text.RegularExpressions;
using DotNet.Globbing;
using Microsoft.CodeAnalysis.CSharp;
using Serilog.Events;
using Shouldly;
using Stryker.Core.Mutators;
using Stryker.Core.Options;
using Xunit;

namespace Stryker.Core.UnitTest.Options
{
    public class StrykerOptionsTests : TestBase
    {
        [Fact]
        public void ShouldCopyValues()
        {
            var target = new StrykerOptions()
            {
                AdditionalTimeout = 1,
                AzureFileStorageSas = "sas",
                AzureFileStorageUrl = "url",
                BaselineProvider = Core.Baseline.Providers.BaselineProvider.AzureFileStorage,
                ProjectPath = "C:/",
                Concurrency = 4,
                DashboardApiKey = "key",
                DashboardUrl = "url",
                DevMode = true,
                Since = true,
                DiffIgnoreChanges = new[] { new FilePattern(Glob.Parse("**"), true, null) },
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

            var result = target.Copy("C://Dev//Test", "C://Dev", "test", new[] { "project1" });

            result.AdditionalTimeout.ShouldBe(1);
            result.ProjectPath.ShouldBe("C://Dev//Test");
            result.WorkingDirectory.ShouldBe("C://Dev");
        }
    }
}
