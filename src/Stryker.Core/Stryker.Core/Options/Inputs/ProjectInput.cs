namespace Stryker.Core.Options.Inputs
{
    public class ProjectInput : InputDefinition<string>
    {
        protected override string Description => "The name of the project you want to mutate.";

        public override string Default => null;

        public string Validate()
        {
            return SuppliedInput;
        }
    }
}
