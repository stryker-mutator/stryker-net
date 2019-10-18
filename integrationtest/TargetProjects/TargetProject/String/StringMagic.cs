namespace ExampleProject.String
{
    public class StringMagic
    {
        public string AddTwoStrings(string first, string second)
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

        private bool Demo(out string test)
        {
            test = "toto";
            return true;
        }

        private bool IsNullOrEmpty(string myString)
        {
            if ((string.IsNullOrEmpty(myString)))
            {
                return true;
            }
            return false;
        }
    }
}
