extern alias TheLog;

using System.Diagnostics;
using TheLog::Serilog.Configuration;

namespace TargetProject.StrykerFeatures;
public class UseAssert
{
    private int _counter = 0;

    public void IncrementCounter()
    {
        // this is only to check for alias support
        var test = new BatchingOptions();
        _counter++;
        Debug.Assert(_counter > 0, "Counter should be greater than 0");
    }

}
