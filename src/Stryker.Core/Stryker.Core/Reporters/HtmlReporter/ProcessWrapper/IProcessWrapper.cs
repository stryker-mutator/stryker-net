using System.Diagnostics;

namespace Stryker.Core.Reporters.HtmlReporter.ProcessWrapper
{
    public interface IProcessWrapper
    {
        Process StartFile(string filePath);
        Process StartUrl(string url);
    }
}
