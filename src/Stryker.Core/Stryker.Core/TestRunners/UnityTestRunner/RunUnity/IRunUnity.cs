using Stryker.Core.Options;
using Stryker.Core.Testing;

namespace Stryker.Core.TestRunners.UnityTestRunner.RunUnity;

public interface IRunUnity
{
    ProcessResult RunUnityUntilFinish(StrykerOptions strykerOptions, string projectPath, string runArgumentsForCli);
}
