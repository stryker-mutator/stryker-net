namespace Stryker.Core.TestRunners
{   
    public interface ITestRunner
    {
        TestRunResult RunAll(int? timeoutMS);
        void SetActiveMutation(int? id);
    }
}