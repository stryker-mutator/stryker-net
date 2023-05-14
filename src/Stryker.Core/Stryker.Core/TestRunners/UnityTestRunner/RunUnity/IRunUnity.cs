using System;
using System.Xml.Linq;
using Stryker.Core.Options;

namespace Stryker.Core.TestRunners.UnityTestRunner.RunUnity;

public interface IRunUnity : IDisposable
{
    public void ReloadDomain(StrykerOptions strykerOptions, string projectPath,
        string additionalArgumentsForCli = null);

    public XDocument RunTests(StrykerOptions strykerOptions, string projectPath,
        string additionalArgumentsForCli = null, string helperNamespace = null, string activeMutantId = null);
}
