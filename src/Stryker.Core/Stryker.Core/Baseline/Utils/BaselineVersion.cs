using Stryker.Abstractions.Exceptions;
using Stryker.Abstractions.Options;
using Stryker.Core.Baseline.Providers;

namespace Stryker.Core.Baseline.Utils;

public static class BaselineVersion
{
    /// <summary>
    /// The version this run stores its own baseline report under.
    /// </summary>
    public static string Current(IStrykerOptions options, IGitInfoProvider gitInfoProvider)
    {
        if (!string.IsNullOrWhiteSpace(options.ProjectVersion))
        {
            return options.ProjectVersion;
        }

        if (!gitInfoProvider.IsRepository)
        {
            throw new InputException("Could not determine the version to store the baseline under because no git branch was found. Please set the project version option.");
        }

        var branchName = gitInfoProvider.GetCurrentBranchName();

        if (string.IsNullOrWhiteSpace(branchName))
        {
            throw new InputException("Could not determine the version to store the baseline under because no git branch was found. Please set the project version option.");
        }

        return branchName;
    }

    /// <summary>
    /// The version the baseline report is loaded from. Defaults to the current version, which makes the run incremental.
    /// </summary>
    public static string Compare(IStrykerOptions options, IGitInfoProvider gitInfoProvider) =>
        string.IsNullOrWhiteSpace(options.BaselineCompareVersion)
            ? Current(options, gitInfoProvider)
            : options.BaselineCompareVersion;
}
