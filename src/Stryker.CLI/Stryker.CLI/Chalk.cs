using System;

namespace Stryker.CLI
{
    public static class Chalk
    {
        public static void Blue(string text)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void Red(string text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void Green(string text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void Gray(string text)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(text);
            Console.ResetColor();
        }

        public static void Yellow(string text)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
