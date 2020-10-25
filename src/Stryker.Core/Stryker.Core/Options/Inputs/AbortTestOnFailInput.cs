namespace Stryker.Core.Options.Inputs
{
    public class AbortTestOnFailInput : SimpleStrykerInput<bool>
    {
        static AbortTestOnFailInput()
        {
            HelpText = @"Abort unit testrun as soon as any one unit test fails. This can reduce the overall running time.";
            DefaultInput = true;
            DefaultValue = new AbortTestOnFailInput(DefaultInput).Value;
        }

        public override StrykerInput Type => StrykerInput.AbortTestOnFail;

        public AbortTestOnFailInput(bool? abortTestOnFail)
        {
            if (abortTestOnFail is { })
            {
                Value = abortTestOnFail.Value;
            }
        }
    }
}
