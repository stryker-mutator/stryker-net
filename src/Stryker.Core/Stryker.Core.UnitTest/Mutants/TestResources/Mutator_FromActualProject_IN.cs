namespace Test 
{
    using System;
    public static class CharCheckExtensions
    {
        [Obsolete("thismustnotbemutated")]
        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {
            var x=1;
            x *=x+2;
            return ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => !IsALetter(sut), "The {0} is not a letter.").
                OnNegate("The {0} is a letter whereas it must not.").
                EndCheck();
        }

        public static string ChainCalls(string check)
        {
            return check.Replace("ab", "cd")
                .Replace("12", "34")
                .PadLeft(12)
                .Replace("12", "34")
                .Normalize();
        }
    }
}