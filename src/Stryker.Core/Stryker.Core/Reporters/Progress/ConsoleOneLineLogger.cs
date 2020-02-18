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
        private readonly bool _hasConsole;
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
            var currentCursorTop = 0;
            var currentCursorLeft = 0;
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
                if (Console.BufferWidth > 0)
                {
                    currentCursorLeft = Math.Min(currentCursorLeft, Console.BufferWidth - 1);
                }

                if (Console.BufferHeight > 0)
                {
                    currentCursorTop = Math.Min(currentCursorTop, Console.BufferHeight - 1);
                }
                Console.SetCursorPosition(currentCursorLeft, currentCursorTop);
            }
        }

        private void ClearLine()
        {
            _logger.LogInformation(new string(' ', Console.WindowWidth));
        }

        private static bool EnvironmentHasConsole()
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
