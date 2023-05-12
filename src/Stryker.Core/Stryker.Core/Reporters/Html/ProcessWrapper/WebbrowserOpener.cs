namespace Stryker.Core.Reporters.Html.ProcessWrapper;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public class WebbrowserOpener : IWebbrowserOpener
{
    public Process Open(string path)
    {
        var processInfo = new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        };

        return Process.Start(processInfo);
    }
}
