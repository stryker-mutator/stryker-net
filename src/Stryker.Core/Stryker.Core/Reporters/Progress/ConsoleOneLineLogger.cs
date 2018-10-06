using System;

namespace Stryker.Core.Reporters.Progress
{
    public class ConsoleOneLineLogger : IConsoleOneLineLogger
    {
        private int _cursorTop;

        private readonly object _mutex = new object();

        public void StartLog(string text)
        {
            lock (_mutex)
            {
                _cursorTop = Console.CursorTop;
                Console.Write(text + Environment.NewLine);
            }
        }

        public void ReplaceLog(string text)
        {
            lock (_mutex)
            {
                var currentCursorTop = Console.CursorTop;
                var currentCursorLeft = Console.CursorLeft;

                Console.SetCursorPosition(0, _cursorTop);
                ClearLine();

                Console.SetCursorPosition(0, _cursorTop);
                Console.Write(text + Environment.NewLine);

                Console.SetCursorPosition(currentCursorLeft, currentCursorTop);
            }
        }

        private static void ClearLine()
        {
            Console.Write(new string(' ', Console.WindowWidth));
        }
    }
}