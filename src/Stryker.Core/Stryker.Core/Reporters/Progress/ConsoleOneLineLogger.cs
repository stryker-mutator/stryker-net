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
        private bool _hasConsole;
        private int _cursorTop;

        private readonly ILogger _logger;

        public ConsoleOneLineLogger(ILogger logger)
        {
            _logger = logger;
            _hasConsole = EnvironmentHasConsole();
        }

        public void StartLog(string text, params object[] args)
        {
            if (_hasConsole)
            {
                _cursorTop = Console.CursorTop;
            }
            _logger.LogInformation(text, args);
        }

        public void ReplaceLog(string text, params object[] args)
        {
            int currentCursorTop = 0;
            int currentCursorLeft = 0;
            if (_hasConsole)
            {
                currentCursorTop = Console.CursorTop;
                currentCursorLeft = Console.CursorLeft;

                Console.SetCursorPosition(0, _cursorTop);
                ClearLine();

                Console.SetCursorPosition(0, _cursorTop);
            }

            _logger.LogInformation(text, args);

            if (_hasConsole)
            {
                Console.SetCursorPosition(currentCursorLeft, currentCursorTop);
            }
        }

        private void ClearLine()
        {
            _logger.LogInformation(new string(' ', Console.WindowWidth));
        }

        private bool EnvironmentHasConsole()
        {
            try
            {
                var t = Console.CursorTop;
                return true;
            }
            catch (System.IO.IOException)
            {
                return false;
            }
        }
    }
}
