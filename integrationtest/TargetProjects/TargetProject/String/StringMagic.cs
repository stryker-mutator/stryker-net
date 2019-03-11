namespace ExampleProject.String
{
    public class StringMagic
    {
        public static string AddTwoStrings(string first, string second)
        {
            if (first.Length > 2)
            {
                return first + second;
            }
            else
            {
                return second + first;
            }
        }

        public static bool Demo(out string test)
        {
            test = "toto";
            return true;
        }
    }
}
