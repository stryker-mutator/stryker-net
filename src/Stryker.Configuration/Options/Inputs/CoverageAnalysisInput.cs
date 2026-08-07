using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Utilities.Logging;

namespace Stryker.Configuration.Options.Inputs;

public class CoverageAnalysisInput : Input<string>
{
    public override string Default => "perTest";

    protected override string Description
    {
        get
        {
            var result = new StringBuilder("Use coverage info to speed up execution. Possible values are: ");
            result.AppendJoin(", ", _possibleValues.Keys).AppendLine(".");
            foreach (var possibleValue in _possibleValues)
            {
                result.AppendLine($"\t- {possibleValue.Key}: {possibleValue.Value.description}");
            }
            return result.ToString();
        }
    }

    private readonly Dictionary<string, (OptimizationModes mode, string description)> _possibleValues = new(StringComparer.OrdinalIgnoreCase)
    {
        ["off"] = (OptimizationModes.None,
            "Coverage data is not captured. Every mutant is tested against all test. Slowest, use in case of doubt."),
        ["perTest"] =
            (OptimizationModes.CoverageBasedTest, "Capture mutations covered by each test. Mutations are tested against covering tests (or flagged NoCoverage if no test cover them). Fastest option."),
        ["all"] =
            (OptimizationModes.SkipUncoveredMutants, "Capture the list of mutations covered by some test. Test only these mutations, other are flagged as NoCoverage. Fast option."),
        ["perTestInIsolation"] =
            (OptimizationModes.CaptureCoveragePerTest | OptimizationModes.CoverageBasedTest, "'perTest' but coverage of each test is captured in isolation. Increase coverage accuracy at the expense of a slow init phase."),
    };

    public OptimizationModes Validate(TestRunner testRunner = TestRunner.VsTest, ILogger<CoverageAnalysisInput>? logger = null)
    {
        var value = (SuppliedInput ?? Default).ToLowerInvariant();
        if (!_possibleValues.TryGetValue(value, out var entry))
        {
            throw new InputException(
                $"Incorrect coverageAnalysis option ({SuppliedInput}). The options are [{string.Join(", ", _possibleValues.Keys)}].");
        }

        var mode = entry.mode;

        // MTP captures per-test coverage by restarting the test host between tests (the host's
        // ProcessExit flushes the covered set), so coverage is always captured in isolation.
        // 'perTest' (process reuse) is not yet available for MTP, so promote it to
        // 'perTestInIsolation'. Reuse is tracked as a follow-up (see PR #3516).
        if (testRunner == TestRunner.MicrosoftTestPlatform
            && mode.HasFlag(OptimizationModes.CoverageBasedTest)
            && !mode.HasFlag(OptimizationModes.CaptureCoveragePerTest))
        {
            // Warn only when the user explicitly asked for 'perTest'; stay silent on the default.
            if (SuppliedInput != null)
            {
                logger ??= ApplicationLogging.LoggerFactory.CreateLogger<CoverageAnalysisInput>();
                logger.LogWarning(
                    "The Microsoft Test Platform runner captures per-test coverage in isolation; 'perTest' "
                    + "(process reuse) is not yet available and has been upgraded to 'perTestInIsolation'. "
                    + "Process reuse for MTP is planned as a follow-up.");
            }

            mode |= OptimizationModes.CaptureCoveragePerTest;
        }

        return mode;
    }
}
