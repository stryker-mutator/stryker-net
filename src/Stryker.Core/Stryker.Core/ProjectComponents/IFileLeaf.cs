namespace Stryker.Core.ProjectComponents
{
    public interface IFileLeaf<T> : IProjectComponent
    {
        public T SyntaxTree { get; set; }

        T MutatedSyntaxTree { get; set; }
    }
}
