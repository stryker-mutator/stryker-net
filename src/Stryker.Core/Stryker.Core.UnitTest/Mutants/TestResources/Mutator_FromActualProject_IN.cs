namespace Test 
{
    using System;
    public static class CharCheckExtensions
    {
        public static ICheckLink<ICodeCheck<T>> LastsLessThan<T>(
            this ICodeCheck<T> check,
            double threshold,
            TimeUnit timeUnit)
            where T : RunTrace
        {
            var durationThreshold = new Duration(threshold, timeUnit);

            ExtensibilityHelper.BeginCheck(check).
                SetSutName("code").
                CheckSutAttributes(sut =>  new Duration(sut.ExecutionTime, timeUnit), "execution time").
                FailWhen((sut) => sut > durationThreshold, "The {checked} was too high.").
                DefineExpectedValue(durationThreshold, "less than", "more than").
                OnNegate("The {checked} was too low.").
                EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {

            return ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => !IsALetter(sut), "The {0} is not a letter.").
                OnNegate("The {0} is a letter whereas it must not.").
                EndCheck();
        }

    }
}