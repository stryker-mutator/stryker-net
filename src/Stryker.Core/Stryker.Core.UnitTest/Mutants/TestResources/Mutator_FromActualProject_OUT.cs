namespace Test 
{
    using System;
    public static class CharCheckExtensions
    {
        [Obsolete("thismustnotbemutated")]
        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {
            var x=1;
if(Stryker.ActiveMutationHelper.ActiveMutation==0){            x /=x+2;
}else{            x *=Stryker.ActiveMutationHelper.ActiveMutation==1?x-2:x+2;
}            return ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => Stryker.ActiveMutationHelper.ActiveMutation==2?IsALetter(sut):!IsALetter(sut), Stryker.ActiveMutationHelper.ActiveMutation==3?"":"The {0} is not a letter.").
                OnNegate(Stryker.ActiveMutationHelper.ActiveMutation==4?"":"The {0} is a letter whereas it must not.").
                EndCheck();
        }

        public static string ChainCalls(string check)
        {
            return check.Replace(Stryker.ActiveMutationHelper.ActiveMutation==5?"":"ab", Stryker.ActiveMutationHelper.ActiveMutation==6?"":"cd")
                .Replace(Stryker.ActiveMutationHelper.ActiveMutation==7?"":"12", Stryker.ActiveMutationHelper.ActiveMutation==8?"":"34")
                .PadLeft(12)
                .Replace(Stryker.ActiveMutationHelper.ActiveMutation==9?"":"12", Stryker.ActiveMutationHelper.ActiveMutation==10?"":"34")
                .Normalize();
        }
    }
}