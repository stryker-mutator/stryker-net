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
                .FailWhen((sut) => Stryker.ActiveMutationHelper.ActiveMutation==2?sut == null || otherEnumerable != null:Stryker.ActiveMutationHelper.ActiveMutation==0?sut != null :sut == null && Stryker.ActiveMutationHelper.ActiveMutation==1?otherEnumerable == null:otherEnumerable != null, Stryker.ActiveMutationHelper.ActiveMutation==3?"":"The {0} is null and thus, does not contain the given expected value(s).")
                .DefineExpectedValues(otherEnumerable, otherEnumerable.Count())
                .Analyze((sut, _) => notFoundValues = ExtractNotFoundValues(sut, otherEnumerable))
                .FailWhen((_) => notFoundValues.Any(), string.Format(Stryker.ActiveMutationHelper.ActiveMutation==4?"":"The {{0}} does not contain the expected value(s):" + Environment.NewLine + Stryker.ActiveMutationHelper.ActiveMutation==5?"":"\t{0}", notFoundValues.ToStringProperlyFormatted().DoubleCurlyBraces()))
                .OnNegate(Stryker.ActiveMutationHelper.ActiveMutation==6?"":"The {0} contains all the given values whereas it must not.")
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
                SetSutName(Stryker.ActiveMutationHelper.ActiveMutation==7?"":"code").
                CheckSutAttributes(sut =>  new Duration(sut.ExecutionTime, timeUnit), Stryker.ActiveMutationHelper.ActiveMutation==8?"":"execution time").
                FailWhen((sut) => Stryker.ActiveMutationHelper.ActiveMutation==10?sut >= durationThreshold:Stryker.ActiveMutationHelper.ActiveMutation==9?sut < durationThreshold:sut > durationThreshold, Stryker.ActiveMutationHelper.ActiveMutation==11?"":"The {checked} was too high.").
                DefineExpectedValue(durationThreshold, Stryker.ActiveMutationHelper.ActiveMutation==12?"":"less than", Stryker.ActiveMutationHelper.ActiveMutation==13?"":"more than").
                OnNegate(Stryker.ActiveMutationHelper.ActiveMutation==14?"":"The {checked} was too low.").
                EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {
            return ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => Stryker.ActiveMutationHelper.ActiveMutation==15?IsALetter(sut):!IsALetter(sut), Stryker.ActiveMutationHelper.ActiveMutation==16?"":"The {0} is not a letter.").
                OnNegate(Stryker.ActiveMutationHelper.ActiveMutation==17?"":"The {0} is a letter whereas it must not.").
                EndCheck();
        }
    }
}