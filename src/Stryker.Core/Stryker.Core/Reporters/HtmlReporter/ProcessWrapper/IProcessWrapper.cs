using System.Diagnostics;

namespace Stryker.Core.Reporters.HtmlReporter.ProcessWrapper
{
    public interface IProcessWrapper
    {
        public Process Start(string filePath);
    }
}
