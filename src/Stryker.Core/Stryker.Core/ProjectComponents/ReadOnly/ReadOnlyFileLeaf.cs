namespace Stryker.Core.ProjectComponents
{
    public class ReadOnlyFileLeaf : ReadOnlyProjectComponent
    {
        private readonly IFileLeaf _projectComponent;

        public string SourceCode => _projectComponent.SourceCode;

        public ReadOnlyFileLeaf(IFileLeaf projectComponent) : base(projectComponent)
        {
            _projectComponent = projectComponent;
        }

        public override void Display()
        {
            DisplayFile(this);
        }

    }
}
