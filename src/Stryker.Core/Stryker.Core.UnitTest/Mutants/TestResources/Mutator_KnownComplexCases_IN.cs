
namespace TestCases
{
    // list syntax construction that are difficult to mutate
    class ComplexCases
    {
        
        private string text = "Some" + "Text";

        // attributes must be constant at build time => no possible mutation
        [Obsolete("thismustnotbemutated")]
        // default parameter must be constant at build time => no posible mutation
        private void SomeMthod(bool option = true)
        {
            // for statement can only me mutated through if(s)
            for (var i = 0; i < 10; i++)
            {
                var x=1;
                x++;
                // should not be mutated (string concatenation)
                var test = "first" + "second";
                // complex mutation pattern
                x *=x+2;
            }
        }
    }
}
