namespace Stryker.Core.ProjectComponents
{
    public class ReadOnlyFileLeaf : ReadOnlyProjectComponent
    {
        private readonly FileLeaf _projectComponent;

        public ReadOnlyFileLeaf(FileLeaf projectComponent) : base(projectComponent)
        {
            _projectComponent = projectComponent;
        }

        public override void Display(int depth)
        {
            DisplayFile(depth, this);
        }

        public string SourceCode => _projectComponent.SourceCode;
    }
}
