using System;

namespace Stryker.Core.Testing
{
    public class Chalk : IChalk
    {
        public void Red(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.ResetColor();
        }

        public void Green(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.ResetColor();
        }

        public void Yellow(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(text);
            Console.ResetColor();
        }
        public void DarkGray(string text)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write(text);
            Console.ResetColor();
        }

        public void Default(string text)
        {
            Console.Write(text);
        }
    }
}
