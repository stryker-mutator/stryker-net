namespace ExampleProject
{
    public class StringMagic
    {
        public string AddTwoStrings(string first, string second)
        {
            if(first.Length > 2)
            {
                return first + second;
            } else
            {
                return second + first;
            }
        }

        private bool Demo(out string test)
        {
            test = "toto";
            return true;
        }
    }
}
