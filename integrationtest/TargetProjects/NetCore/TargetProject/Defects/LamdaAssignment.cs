// https://github.com/stryker-mutator/stryker-net/issues/339

namespace TargetProject.Defects;

public class LamdaAssignment
{
    public void DoIt()
    {
        Action act = () => Console.WriteLine("A", "B", "C");
    }
}
