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
                SetSutName(Stryker.ActiveMutationHelper.ActiveMutation==0?"":"code").
                CheckSutAttributes(sut =>  new Duration(sut.ExecutionTime, timeUnit), Stryker.ActiveMutationHelper.ActiveMutation==1?"":"execution time").
                FailWhen((sut) => Stryker.ActiveMutationHelper.ActiveMutation==3?sut >= durationThreshold:Stryker.ActiveMutationHelper.ActiveMutation==2?sut < durationThreshold:sut > durationThreshold, Stryker.ActiveMutationHelper.ActiveMutation==4?"":"The {checked} was too high.").
                DefineExpectedValue(durationThreshold, Stryker.ActiveMutationHelper.ActiveMutation==5?"":"less than", Stryker.ActiveMutationHelper.ActiveMutation==6?"":"more than").
                OnNegate(Stryker.ActiveMutationHelper.ActiveMutation==7?"":"The {checked} was too low.").
                EndCheck();

            return ExtensibilityHelper.BuildCheckLink(check);
        }

        public static ICheckLink<ICheck<char>> IsALetter(this ICheck<char> check)
        {

            return ExtensibilityHelper.BeginCheck(check).
                FailWhen(sut => Stryker.ActiveMutationHelper.ActiveMutation==8?IsALetter(sut):!IsALetter(sut), Stryker.ActiveMutationHelper.ActiveMutation==9?"":"The {0} is not a letter.").
                OnNegate(Stryker.ActiveMutationHelper.ActiveMutation==10?"":"The {0} is a letter whereas it must not.").
                EndCheck();
        }

    }
}