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
                .FailWhen((sut) => (StrykerNamespace.MutantControl.IsActive(0)?sut == null || otherEnumerable != null:(StrykerNamespace.MutantControl.IsActive(1)?sut != null :sut == null )&& (StrykerNamespace.MutantControl.IsActive(2)?otherEnumerable == null:otherEnumerable != null)), (StrykerNamespace.MutantControl.IsActive(3)?"":"The {0} is null and thus, does not contain the given expected value(s)."))
                .DefineExpectedValues(otherEnumerable, otherEnumerable.Count())
                .Analyze((sut, _) => notFoundValues = ExtractNotFoundValues(sut, otherEnumerable))
                .FailWhen((_) => notFoundValues.Any(), string.Format((StrykerNamespace.MutantControl.IsActive(4)?"":"The {{0}} does not contain the expected value(s):" )+ Environment.NewLine + (StrykerNamespace.MutantControl.IsActive(5)?"":"\t{0}"), notFoundValues.ToStringProperlyFormatted().DoubleCurlyBraces()))
                .OnNegate((StrykerNamespace.MutantControl.IsActive(6)?"":"The {0} contains all the given values whereas it must not."))
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
                SetSutName((StrykerNamespace.MutantControl.IsActive(7)?"":"code")).
                CheckSutAttributes(sut =>  new Duration(sut.ExecutionTime, timeUnit), (StrykerNamespace.MutantControl.IsActive(8)?"":"execution time")).
                FailWhen((sut) => (StrykerNamespace.MutantControl.IsActive(10)?sut >= durationThreshold:(StrykerNamespace.MutantControl.IsActive(9)?sut < durationThreshold:sut > durationThreshold)), (StrykerNamespace.MutantControl.IsActive(11)?"":"The {checked} was too high.")).
                DefineExpectedValue(durationThreshold, (StrykerNamespace.MutantControl.IsActive(12)?"":"less than"), (StrykerNamespace.MutantControl.IsActive(13)?"":"more than")).
                OnNegate((StrykerNamespace.MutantControl.IsActive(14)?"":"The {checked} was too low.")).
                EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {
            return ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => (StrykerNamespace.MutantControl.IsActive(15)?IsALetter(sut):!IsALetter(sut)), (StrykerNamespace.MutantControl.IsActive(16)?"":"The {0} is not a letter.")).
                OnNegate((StrykerNamespace.MutantControl.IsActive(17)?"":"The {0} is a letter whereas it must not.")).
                EndCheck();
        }
    }
}