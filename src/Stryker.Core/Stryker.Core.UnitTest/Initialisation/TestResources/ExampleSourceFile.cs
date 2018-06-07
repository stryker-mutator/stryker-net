using System;

namespace ExampleProject
{
    public class Recursive
    {
        public int Fibinacci(int len)
        {
            return Fibonacci(0, 1, 1, len);
        }

        private int Fibonacci(int a, int b, int counter, int len)
        {
            if (counter <= len)
            {
                Console.Write("{0}", a);
                return Fibonacci(b, a + b, counter + 1, len);
            }
            return 0;
        }
    }
}