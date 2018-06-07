using Stryker.Core.Options;

namespace Stryker.Core
{
    public interface IStrykerRunner
    {
        void RunMutationTest(StrykerOptions options);
    }
}