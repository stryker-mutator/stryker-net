// https://github.com/stryker-mutator/stryker-net/issues/250

namespace TargetProject.Defects
{
    public class ShouldNotPlaceLinqMutant
    {
        public int SomeMethodWithOutParameter(out int Any)
        {
            Any = 1;
            return 4;
        }
    }
}
