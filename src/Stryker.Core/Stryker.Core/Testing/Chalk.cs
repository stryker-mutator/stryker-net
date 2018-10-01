using System;

namespace Stryker.Core.Testing
{
    public interface IChalk
    {
        void Red(string text);
        void Yellow(string text);
        void Green(string text);
        void DarkGray(string text);
        void Default(string text);
    }
    
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
