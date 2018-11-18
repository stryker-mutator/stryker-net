using Microsoft.Extensions.Logging;
using System;

namespace Stryker.Core.Reporters.Progress
{
    public interface IConsoleOneLineLogger
    {
        void StartLog(string text, params object[] args);
        void ReplaceLog(string text, params object[] args);
    }

    public class ConsoleOneLineLogger : IConsoleOneLineLogger
    {
        private int _cursorTop;

        private readonly ILogger _logger;

        public ConsoleOneLineLogger(ILogger logger)
        {
            _logger = logger;
        }

        public void StartLog(string text, params object[] args)
        {
            _cursorTop = Console.CursorTop;
            _logger.LogInformation(text, args);
        }

        public void ReplaceLog(string text, params object[] args)
        {
            var currentCursorTop = Console.CursorTop;
            var currentCursorLeft = Console.CursorLeft;

            Console.SetCursorPosition(0, _cursorTop);
            ClearLine();

            Console.SetCursorPosition(0, _cursorTop);
            _logger.LogInformation(text, args);

            Console.SetCursorPosition(currentCursorLeft, currentCursorTop);
        }

        private void ClearLine()
        {
            _logger.LogInformation(new string(' ', Console.WindowWidth));
        }
    }
}