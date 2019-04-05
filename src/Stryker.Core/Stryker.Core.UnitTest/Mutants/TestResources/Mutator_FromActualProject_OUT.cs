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
                .FailWhen((sut) => (StrykerNamespace.ActiveMutationHelper.ActiveMutation==2?sut == null || otherEnumerable != null:(StrykerNamespace.ActiveMutationHelper.ActiveMutation==0?sut != null :sut == null )&& (StrykerNamespace.ActiveMutationHelper.ActiveMutation==1?otherEnumerable == null:otherEnumerable != null)), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==3?"":"The {0} is null and thus, does not contain the given expected value(s)."))
                .DefineExpectedValues(otherEnumerable, otherEnumerable.Count())
                .Analyze((sut, _) => notFoundValues = ExtractNotFoundValues(sut, otherEnumerable))
                .FailWhen((_) => notFoundValues.Any(), string.Format((StrykerNamespace.ActiveMutationHelper.ActiveMutation==4?"":"The {{0}} does not contain the expected value(s):" )+ Environment.NewLine + (StrykerNamespace.ActiveMutationHelper.ActiveMutation==5?"":"\t{0}"), notFoundValues.ToStringProperlyFormatted().DoubleCurlyBraces()))
                .OnNegate((StrykerNamespace.ActiveMutationHelper.ActiveMutation==6?"":"The {0} contains all the given values whereas it must not."))
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
                SetSutName((StrykerNamespace.ActiveMutationHelper.ActiveMutation==7?"":"code")).
                CheckSutAttributes(sut =>  new Duration(sut.ExecutionTime, timeUnit), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==8?"":"execution time")).
                FailWhen((sut) => (StrykerNamespace.ActiveMutationHelper.ActiveMutation==10?sut >= durationThreshold:(StrykerNamespace.ActiveMutationHelper.ActiveMutation==9?sut < durationThreshold:sut > durationThreshold)), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==11?"":"The {checked} was too high.")).
                DefineExpectedValue(durationThreshold, (StrykerNamespace.ActiveMutationHelper.ActiveMutation==12?"":"less than"), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==13?"":"more than")).
                OnNegate((StrykerNamespace.ActiveMutationHelper.ActiveMutation==14?"":"The {checked} was too low.")).
                EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {
            return ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => (StrykerNamespace.ActiveMutationHelper.ActiveMutation==15?IsALetter(sut):!IsALetter(sut)), (StrykerNamespace.ActiveMutationHelper.ActiveMutation==16?"":"The {0} is not a letter.")).
                OnNegate((StrykerNamespace.ActiveMutationHelper.ActiveMutation==17?"":"The {0} is a letter whereas it must not.")).
                EndCheck();
        }
    }
}