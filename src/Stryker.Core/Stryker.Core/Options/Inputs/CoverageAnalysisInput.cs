using System;
using System.Collections.Generic;
using System.Text;
using Stryker.Core.Exceptions;

namespace Stryker.Core.Options.Inputs;

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

    public OptimizationModes Validate()
    {
        var value = (SuppliedInput ?? Default).ToLower();
        if (_possibleValues.ContainsKey(value))
        {
            return _possibleValues[value].mode;
        }

        throw new InputException(
            $"Incorrect coverageAnalysis option ({SuppliedInput}). The options are [{string.Join(", ", _possibleValues.Keys)}].");
    }
}
