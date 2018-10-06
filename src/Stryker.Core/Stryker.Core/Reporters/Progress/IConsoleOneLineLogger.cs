namespace Stryker.Core.Reporters.Progress
{
    public interface IConsoleOneLineLogger
    {
        void StartLog(string text);
        void ReplaceLog(string text);
    }
}