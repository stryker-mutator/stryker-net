using System.Diagnostics;

namespace TargetProject.StrykerFeatures;
public class UseAssert
{
    private int _counter = 0;

    public void IncrementCounter()
    {
        _counter++;
        Debug.Assert(_counter > 0, "Counter should be greater than 0");
    }

}
