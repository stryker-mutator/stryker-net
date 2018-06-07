namespace Stryker.Core.TestRunners
{
    public interface ITestRunner
    {
        TestRunResult RunAll();
        void SetActiveMutation(int? id);
    }
}
