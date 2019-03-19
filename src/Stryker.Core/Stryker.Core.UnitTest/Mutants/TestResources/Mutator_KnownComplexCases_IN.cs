
namespace TestCases
{
    // list syntax construction that are difficult to mutate
    class ComplexCases
    {

        private void GoodLuck()
        {
            await SendRequest(url, HttpMethod.Get, (request) =>
            {
                request.Headers.Add("Accept", "application/json; version=1");
                request.Headers.TryAddWithoutValidation("Date", datum);
            }, ensureSuccessStatusCode: false);
        }

        private string text = "Some" + "Text";
        // const can't me mutated (need to be const at build time)
        private const int x = 1 + 2;
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

            for (var j = 0;; j++)
            {
                break:
            }
        }
    }
}
