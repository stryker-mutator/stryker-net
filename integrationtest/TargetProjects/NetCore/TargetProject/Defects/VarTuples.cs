// https://github.com/stryker-mutator/stryker-net/issues/269

using System.Threading.Tasks;

public class VarTuples
{
    public async Task<(int, int)> ExampleBugMethod()
    {
        var (one, two) = (1, 2);

        return (one, two);
    }
}
