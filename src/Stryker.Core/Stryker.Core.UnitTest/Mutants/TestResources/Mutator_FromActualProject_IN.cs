namespace Test 
{
    using System;
    public static class CharCheckExtensions
    {
        
        private static void ImplementContains<T>(ICheckLogic<IEnumerable<T>> checker, IEnumerable<T> otherEnumerable)
        {
            IList<object> notFoundValues = null;
            checker
                .FailWhen((sut) => sut == null && otherEnumerable != null, "The {0} is null and thus, does not contain the given expected value(s).")
                .DefineExpectedValues(otherEnumerable, otherEnumerable.Count())
                .Analyze((sut, _) => notFoundValues = ExtractNotFoundValues(sut, otherEnumerable))
                .FailWhen((_) => notFoundValues.Any(), string.Format("The {{0}} does not contain the expected value(s):" + Environment.NewLine + "\t{0}", notFoundValues.ToStringProperlyFormatted().DoubleCurlyBraces()))
                .OnNegate("The {0} contains all the given values whereas it must not.")
                .EndCheck();
        }
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