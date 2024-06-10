// https://github.com/stryker-mutator/stryker-net/issues/642

namespace TargetProject.Defects;

public class WhileTrue
{
    public static bool Loop(bool stop)
    {
        while (true)
        {
            if (stop)
            {
                return true;
            }
        }
    }
}
