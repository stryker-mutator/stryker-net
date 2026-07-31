using System.Collections.Generic;

namespace Stryker.TestRunner;

public static class ExternalEnvironmentVariables
{
    public static void Add(IDictionary<string, string?> environmentVariables)
    {
        // Disable DiffEngine so that approval tests frameworks such as https://github.com/VerifyTests/Verify
        // or https://github.com/approvals/ApprovalTests.Net (which both use DiffEngine under the hood)
        // don't launch a diffing tool GUI on each failed test.
        // See https://github.com/VerifyTests/DiffEngine#disable-for-a-machineprocess
        environmentVariables["DiffEngine_Disabled"] = "true";
        // Disable copying the command to accept the received version to the clipboard when using Verify
        // See https://github.com/VerifyTests/Verify/blob/main/docs/clipboard.md#for-a-machine
        environmentVariables["Verify_DisableClipboard"] = "true";
    }
}
