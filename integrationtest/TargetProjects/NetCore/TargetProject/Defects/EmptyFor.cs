// https://github.com/stryker-mutator/stryker-net/issues/412

namespace TargetProject.Defects;

public class EmptyFor
{
    public void DoIt()
    {
        for (var x = 0; ; x++)
            ;
    }
}
