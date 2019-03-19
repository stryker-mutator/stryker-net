namespace TestCases
{
    // list syntax construction that are difficult to mutate
    class ComplexCases
    {

        private void GoodLuck()
        {
            await SendRequest(url, HttpMethod.Get, (request) =>
            {
                request.Headers.Add(Stryker.ActiveMutationHelper.ActiveMutation==0?"":"Accept", Stryker.ActiveMutationHelper.ActiveMutation==1?"":"application/json; version=1");
                request.Headers.TryAddWithoutValidation(Stryker.ActiveMutationHelper.ActiveMutation==2?"":"Date", datum);
            }, ensureSuccessStatusCode: Stryker.ActiveMutationHelper.ActiveMutation==3?true:false);
        }

        private string text = Stryker.ActiveMutationHelper.ActiveMutation==4?"":"Some" + Stryker.ActiveMutationHelper.ActiveMutation==5?"":"Text";
        // const can't me mutated (need to be const at build time)
        private const int x = 1 + 2;
        // attributes must be constant at build time => no possible mutation
        [Obsolete("thismustnotbemutated")]
        // default parameter must be constant at build time => no posible mutation
        private void SomeMthod(bool option = true)
        {
if(Stryker.ActiveMutationHelper.ActiveMutation==6){            // for statement can only me mutated through if(s)
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
            for (var i = 0; Stryker.ActiveMutationHelper.ActiveMutation==8?i <= 10:Stryker.ActiveMutationHelper.ActiveMutation==7?i > 10:i < 10; i++)
            {
                var x=1;
if(Stryker.ActiveMutationHelper.ActiveMutation==9){                x--;
}else{                x++;
}                // should not be mutated (string concatenation)
                var test = Stryker.ActiveMutationHelper.ActiveMutation==10?"":"first" + Stryker.ActiveMutationHelper.ActiveMutation==11?"":"second";
if(Stryker.ActiveMutationHelper.ActiveMutation==12){                // complex mutation pattern
                x /=x+2;
}else{                // complex mutation pattern
                x *=Stryker.ActiveMutationHelper.ActiveMutation==13?x-2:x+2;
}            }
}if(Stryker.ActiveMutationHelper.ActiveMutation==14){
            for (var j = 0;; j--)
            {
                break:
            }
}else{
            for (var j = 0;; j++)
            {
                break:
            }
}        }
    }
}
