namespace Stryker.Core.TestRunners
{
    public interface ITestRunner
    {
        TestRunResult RunAll(int timeoutMS = 0);
        void SetActiveMutation(int? id);
    }
}
