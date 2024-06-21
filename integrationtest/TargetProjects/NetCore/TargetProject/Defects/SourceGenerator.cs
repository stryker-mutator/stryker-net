// https://github.com/stryker-mutator/stryker-net/issues/1413

namespace TargetProject.Defects;

public class  ExecuteSourceGenerated
{
    public int Execute(int one, int two)
    {
        // call into a generated method
        return GeneratedNamespace.GeneratedClass.GeneratedMath(one, two);
    }
}
