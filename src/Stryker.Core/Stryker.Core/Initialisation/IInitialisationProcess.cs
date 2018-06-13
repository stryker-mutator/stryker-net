using Stryker.Core.MutationTest;
using Stryker.Core.Options;

namespace Stryker.Core.Initialisation
{
    public interface IInitialisationProcess
    {
        MutationTestInput Initialize(ValidatedStrykerOptions options);
    }
}