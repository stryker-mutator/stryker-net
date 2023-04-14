namespace ExampleProject
{
    public class TestClassB
    {
        [Fact]
        public int Fibinacci(int len)
        {
            return Fibonacci(0, 1, 1, len);
        }

        [Fact]
        public int Fibonacci(int a, int b, int counter, int len)
        {
            if (counter <= len)
            {
                Console.Write("{0}", a);
                return Fibonacci(b, a + b, counter + 1, len);
            }
            return 0;
        }

        [Fact]
        public string LoremIpsum()
        {
            return @"Lorem Ipsum
                    Dolor Sit Amet
                    Lorem Dolor Sit";
        }

        [Fact]
        public void StringSplit()
        {
            var testString = "";

            testString.Split("\n");


        }
    }
}
