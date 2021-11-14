namespace Stryker.Core.MutationTest
{
    using Options;
    using Reporters;

    public interface IMutationTestProcessProvider
    {
        IMutationTestProcess Provide(MutationTestInput mutationTestInput, IReporter reporter, IMutationTestExecutor mutationTestExecutor, StrykerOptions options);
    }
}
