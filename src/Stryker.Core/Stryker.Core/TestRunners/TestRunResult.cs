namespace Stryker.Core.TestRunners
{
    public class TestRunResult
    {
        public bool Success { get; set; }
        public string ResultMessage { get; set; }
        public int TotalNumberOfTests { get; set; }
    }
}