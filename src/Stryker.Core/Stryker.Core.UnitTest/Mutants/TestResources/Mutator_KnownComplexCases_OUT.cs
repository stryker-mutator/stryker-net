using System;
using System.Net.Http;

namespace TestCases
{
    // list syntax construction that are difficult to mutate
    class ComplexCases
    {

        private async Task GoodLuck()
        {
            await SendRequest(url, HttpMethod.Get, (request) =>
            {
                request.Headers.Add((StrykerNamespace.MutantControl.IsActive(0)?"":"Accept"), (StrykerNamespace.MutantControl.IsActive(1)?"":"application/json; version=1"));
                request.Headers.TryAddWithoutValidation((StrykerNamespace.MutantControl.IsActive(2)?"":"Date"), datum);
            }, ensureSuccessStatusCode: (StrykerNamespace.MutantControl.IsActive(3)?true:false));
        }

        private string text = (StrykerNamespace.MutantControl.IsActive(4)?"":"Some" )+ (StrykerNamespace.MutantControl.IsActive(5)?"":"Text");
        // const can't me mutated (need to be const at build time)
        private const int x = 1 + 2;
        // attributes must be constant at build time => no possible mutation
        [Obsolete("thismustnotbemutated")]
        // default parameter must be constant at build time => no posible mutation
        private void SomeMthod(bool option = true)
        {
            // empty for are tricky
            for (;;)
            {
                int x = (StrykerNamespace.MutantControl.IsActive(6)?1 - 2:1 + 2);
            }
if(StrykerNamespace.MutantControl.IsActive(7)){            // for statement can only me mutated through if(s)
            for (var i = 0; i < 10; i--)
            {
                var x=1;
                x++;
                // should not be mutated (string concatenation)
                var test = "first" + "second";
                // complex mutation pattern
                x *=x+2;
            }
}else{            // for statement can only me mutated through if(s)
            for (var i = 0; (StrykerNamespace.MutantControl.IsActive(9)?i <= 10:(StrykerNamespace.MutantControl.IsActive(8)?i > 10:i < 10)); i++)
            {
                var x=1;
if(StrykerNamespace.MutantControl.IsActive(10)){                x--;
}else{                x++;
}                // should not be mutated (string concatenation)
                var test = (StrykerNamespace.MutantControl.IsActive(11)?"":"first" )+ (StrykerNamespace.MutantControl.IsActive(12)?"":"second");
if(StrykerNamespace.MutantControl.IsActive(13)){                // complex mutation pattern
                x /=x+2;
}else{                // complex mutation pattern
                x *=(StrykerNamespace.MutantControl.IsActive(14)?x-2:x+2);
}            }
}if(StrykerNamespace.MutantControl.IsActive(15)){
            for (var j = 0;; j--)
            {
                break;
            }
}else{
            for (var j = 0;; j++)
            {
                break;
            }
}        }
    }
}
