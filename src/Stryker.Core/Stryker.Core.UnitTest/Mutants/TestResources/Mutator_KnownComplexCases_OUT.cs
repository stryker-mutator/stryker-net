namespace TestCases
{
    // list syntax construction that are difficult to mutate
    class ComplexCases
    {
        
        private string text = Stryker.MutantControl.IsActive(1)?"Some" + "":Stryker.MutantControl.IsActive(0)?""+ "Text":"Some" + "Text";
        // const can't me mutated (need to be const at build time)
        private const int x = 1 + 2;
        // attributes must be constant at build time => no possible mutation
        [Obsolete("thismustnotbemutated")]
        // default parameter must be constant at build time => no posible mutation
        private void SomeMthod(bool option = true)
        {
if(Stryker.MutantControl.IsActive(2)){            // for statement can only me mutated through if(s)
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
            for (var i = 0; Stryker.MutantControl.IsActive(4)?i <= 10:Stryker.MutantControl.IsActive(3)?i > 10:i < 10; i++)
            {
                var x=1;
if(Stryker.MutantControl.IsActive(5)){                x--;
}else{                x++;
}                // should not be mutated (string concatenation)
                var test = Stryker.MutantControl.IsActive(7)?"first" + "":Stryker.MutantControl.IsActive(6)?""+ "second":"first" + "second";
if(Stryker.MutantControl.IsActive(8)){                // complex mutation pattern
                x /=x+2;
}else{                // complex mutation pattern
                x *=Stryker.MutantControl.IsActive(9)?x-2:x+2;
}            }
}        }
    }
}
