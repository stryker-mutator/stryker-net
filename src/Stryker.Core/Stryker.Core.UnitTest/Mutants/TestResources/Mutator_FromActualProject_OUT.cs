namespace Test 
{
    using System;
    public static class CharCheckExtensions
    {
        
        private static void ImplementContains<T>(ICheckLogic<IEnumerable<T>> checker, IEnumerable<T> otherEnumerable)
        {
            IList<object> notFoundValues = null;
            checker
                .FailWhen((sut) => Stryker.ActiveMutationHelper.ActiveMutation==2?sut == null && otherEnumerable == null:Stryker.ActiveMutationHelper.ActiveMutation==1?sut != null && otherEnumerable != null:Stryker.ActiveMutationHelper.ActiveMutation==0?sut == null || otherEnumerable != null:sut == null && otherEnumerable != null, Stryker.ActiveMutationHelper.ActiveMutation==3?"":"The {0} is null and thus, does not contain the given expected value(s).")
                .DefineExpectedValues(otherEnumerable, Stryker.ActiveMutationHelper.ActiveMutation==4?otherEnumerable.Sum():otherEnumerable.Count())
                .Analyze((sut, _) => notFoundValues = ExtractNotFoundValues(sut, otherEnumerable))
                .FailWhen((_) => notFoundValues.Any(), Stryker.ActiveMutationHelper.ActiveMutation==8?string.Format("The {{0}} does not contain the expected value(s):" + Environment.NewLine + "", notFoundValues.ToStringProperlyFormatted().DoubleCurlyBraces()):Stryker.ActiveMutationHelper.ActiveMutation==7?string.Format(""+ Environment.NewLine + "\t{0}", notFoundValues.ToStringProperlyFormatted().DoubleCurlyBraces()):string.Format("The {{0}} does not contain the expected value(s):" + Environment.NewLine + "\t{0}", notFoundValues.ToStringProperlyFormatted().DoubleCurlyBraces()))
                .OnNegate(Stryker.ActiveMutationHelper.ActiveMutation==9?"":"The {0} contains all the given values whereas it must not.")
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
                SetSutName(Stryker.ActiveMutationHelper.ActiveMutation==10?"":"code").
                CheckSutAttributes(sut =>  new Duration(sut.ExecutionTime, timeUnit), Stryker.ActiveMutationHelper.ActiveMutation==11?"":"execution time").
                FailWhen((sut) => Stryker.ActiveMutationHelper.ActiveMutation==13?sut >= durationThreshold:Stryker.ActiveMutationHelper.ActiveMutation==12?sut < durationThreshold:sut > durationThreshold, Stryker.ActiveMutationHelper.ActiveMutation==14?"":"The {checked} was too high.").
                DefineExpectedValue(durationThreshold, Stryker.ActiveMutationHelper.ActiveMutation==15?"":"less than", Stryker.ActiveMutationHelper.ActiveMutation==16?"":"more than").
                OnNegate(Stryker.ActiveMutationHelper.ActiveMutation==17?"":"The {checked} was too low.").
                EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {

            return Stryker.ActiveMutationHelper.ActiveMutation==23?ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => !IsALetter(sut), "The {0} is not a letter.").
                OnNegate("").
                EndCheck():Stryker.ActiveMutationHelper.ActiveMutation==22?ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => !IsALetter(sut), "").
                OnNegate("The {0} is a letter whereas it must not.").
                EndCheck():Stryker.ActiveMutationHelper.ActiveMutation==21?ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => IsALetter(sut), "The {0} is not a letter.").
                OnNegate("The {0} is a letter whereas it must not.").
                EndCheck():ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => !IsALetter(sut), "The {0} is not a letter.").
                OnNegate("The {0} is a letter whereas it must not.").
                EndCheck();
        }
    }
}