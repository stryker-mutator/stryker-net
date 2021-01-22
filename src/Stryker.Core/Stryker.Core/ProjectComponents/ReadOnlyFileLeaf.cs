namespace Stryker.Core.ProjectComponents
{
    public class ReadOnlyFileLeaf : ReadOnlyProjectComponent
    {
        private readonly FileLeaf _projectComponent;

        public string SourceCode => _projectComponent.SourceCode;

        public ReadOnlyFileLeaf(FileLeaf projectComponent) : base(projectComponent)
        {
            _projectComponent = projectComponent;
        }

        public override void Display()
        {
            DisplayFile(this);
        }

    }
}
