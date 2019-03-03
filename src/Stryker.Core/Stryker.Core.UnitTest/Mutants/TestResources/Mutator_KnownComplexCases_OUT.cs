namespace TestCases
{
    // list syntax construction that are difficult to mutate
    class ComplexCases
    {
        
        private string text = Stryker.ActiveMutationHelper.ActiveMutation==1?"Some" + "":Stryker.ActiveMutationHelper.ActiveMutation==0?""+ "Text":"Some" + "Text";

        // attributes must be constant at build time => no possible mutation
        [Obsolete("thismustnotbemutated")]
        // default parameter must be constant at build time => no posible mutation
        private void SomeMthod(bool option = true)
        {
if(Stryker.ActiveMutationHelper.ActiveMutation==2){            // for statement can only me mutated through if(s)
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
            for (var i = 0; Stryker.ActiveMutationHelper.ActiveMutation==4?i <= 10:Stryker.ActiveMutationHelper.ActiveMutation==3?i > 10:i < 10; i++)
            {
                var x=1;
if(Stryker.ActiveMutationHelper.ActiveMutation==5){                x--;
}else{                x++;
}                // should not be mutated (string concatenation)
                var test = Stryker.ActiveMutationHelper.ActiveMutation==7?"first" + "":Stryker.ActiveMutationHelper.ActiveMutation==6?""+ "second":"first" + "second";
if(Stryker.ActiveMutationHelper.ActiveMutation==8){                // complex mutation pattern
                x /=x+2;
}else{                // complex mutation pattern
                x *=Stryker.ActiveMutationHelper.ActiveMutation==9?x-2:x+2;
}            }
}        }
    }
}
