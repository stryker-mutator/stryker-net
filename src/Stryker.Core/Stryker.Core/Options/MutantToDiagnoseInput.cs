namespace Stryker.Core.Options
{
    public class MutantToDiagnoseInput: Input<int?>
    {
        public override int? Default { get; }
        protected override string Description => "Allows to perform a diagnosis on one problematic mutant. One should use this open when disagreeing with test result, such as a survivor that should have been killed."
            +"The mutant will be tested normally, then against all test and finally in isolation. Stryker will then provide a diagnostic based on the results";
    }
}
