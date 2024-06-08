// https://github.com/stryker-mutator/stryker-net/issues/287

namespace TargetProject.Defects
{
    public class OutVariable
    {
        public string UnsupportedSyntax(string a, string b)
        {
            // mutating this statement triggers the error
            if (Demo(out var test))
            {
                return a + b;
            }
            return test;
        }
        private bool Demo(out string test)
        {
            test = "dummy";
            return true;
        }
    }
}
