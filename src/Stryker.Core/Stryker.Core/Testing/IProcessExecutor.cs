using System.Collections.Generic;

namespace Stryker.Core.Testing
{
    /// <summary>
    /// Used for mocking System.Process 
    /// </summary>
    public interface IProcessExecutor
    {
        /// <summary>
        /// Starts an process and returns the result when done. Takes an environment variable for active mutation
        /// </summary>
        /// <param name="path">The path the process will use as base path</param>
        /// <param name="application">example: dotnet</param>
        /// <param name="arguments">example: --no-build</param>
        /// <param name="activeMutationId">this value will be used to set an environment variable for the process</param>
        /// <returns>ProcessResult</returns>
        ProcessResult Start(string path, string application, string arguments, IEnumerable<KeyValuePair<string, string>> environmentVariables = null, int timeoutMS = 0);
    }
}
