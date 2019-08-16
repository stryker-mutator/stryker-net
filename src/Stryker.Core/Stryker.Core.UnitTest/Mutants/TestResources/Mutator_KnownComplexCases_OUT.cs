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

        private bool Move()
        {
            return (StrykerNamespace.MutantControl.IsActive(6)?false:true);
        }

        private void DummyLoop()
        {
            while ((StrykerNamespace.MutantControl.IsActive(7)?!this.Move():this.Move()))
            {
                int x = (StrykerNamespace.MutantControl.IsActive(8)?2 - 3:2 + 3);
            }
        }

        // attributes must be constant at build time => no possible mutation
        [Obsolete("thismustnotbemutated")]
        // default parameter must be constant at build time => no posible mutation
        private void SomeMthod(bool option = true)
        {
            // empty for are tricky
            for (;;)
            {
                int x = (StrykerNamespace.MutantControl.IsActive(9)?1 - 2:1 + 2);
            }
if(StrykerNamespace.MutantControl.IsActive(10)){            // for statement can only me mutated through if(s)
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
            for (var i = 0; (StrykerNamespace.MutantControl.IsActive(12)?i <= 10:(StrykerNamespace.MutantControl.IsActive(11)?i > 10:i < 10)); i++)
            {
                var x=1;
if(StrykerNamespace.MutantControl.IsActive(13)){                x--;
}else{                x++;
}                // should not be mutated (string concatenation)
                var test = (StrykerNamespace.MutantControl.IsActive(14)?"":"first" )+ (StrykerNamespace.MutantControl.IsActive(15)?"":"second");
if(StrykerNamespace.MutantControl.IsActive(16)){                // complex mutation pattern
                x /=x+2;
}else{                // complex mutation pattern
                x *=(StrykerNamespace.MutantControl.IsActive(17)?x-2:x+2);
}            }
}if(StrykerNamespace.MutantControl.IsActive(18)){
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
