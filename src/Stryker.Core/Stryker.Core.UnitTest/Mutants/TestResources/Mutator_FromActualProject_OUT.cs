namespace Test 
{
    using System;
    public static class CharCheckExtensions
    {
        
        // Following code lines have been extracted from NFluent source code (www.n-fluent.net)
        private static void ImplementContains<T>(ICheckLogic<IEnumerable<T>> checker, IEnumerable<T> otherEnumerable)
        {
            IList<object> notFoundValues = null;
            checker
                .FailWhen((sut) => (Stryker.MutantControl.IsActive(2)?sut == null || otherEnumerable != null:(Stryker.MutantControl.IsActive(0)?sut != null :sut == null )&& (Stryker.MutantControl.IsActive(1)?otherEnumerable == null:otherEnumerable != null)), (Stryker.MutantControl.IsActive(3)?"":"The {0} is null and thus, does not contain the given expected value(s)."))
                .DefineExpectedValues(otherEnumerable, otherEnumerable.Count())
                .Analyze((sut, _) => notFoundValues = ExtractNotFoundValues(sut, otherEnumerable))
                .FailWhen((_) => notFoundValues.Any(), string.Format((Stryker.MutantControl.IsActive(4)?"":"The {{0}} does not contain the expected value(s):" )+ Environment.NewLine + (Stryker.MutantControl.IsActive(5)?"":"\t{0}"), notFoundValues.ToStringProperlyFormatted().DoubleCurlyBraces()))
                .OnNegate((Stryker.MutantControl.IsActive(6)?"":"The {0} contains all the given values whereas it must not."))
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
                SetSutName((Stryker.MutantControl.IsActive(7)?"":"code")).
                CheckSutAttributes(sut =>  new Duration(sut.ExecutionTime, timeUnit), (Stryker.MutantControl.IsActive(8)?"":"execution time")).
                FailWhen((sut) => (Stryker.MutantControl.IsActive(10)?sut >= durationThreshold:(Stryker.MutantControl.IsActive(9)?sut < durationThreshold:sut > durationThreshold)), (Stryker.MutantControl.IsActive(11)?"":"The {checked} was too high.")).
                DefineExpectedValue(durationThreshold, (Stryker.MutantControl.IsActive(12)?"":"less than"), (Stryker.MutantControl.IsActive(13)?"":"more than")).
                OnNegate((Stryker.MutantControl.IsActive(14)?"":"The {checked} was too low.")).
                EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {
            return ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => (Stryker.MutantControl.IsActive(15)?IsALetter(sut):!IsALetter(sut)), (Stryker.MutantControl.IsActive(16)?"":"The {0} is not a letter.")).
                OnNegate((Stryker.MutantControl.IsActive(17)?"":"The {0} is a letter whereas it must not.")).
                EndCheck();
        }
    }
}