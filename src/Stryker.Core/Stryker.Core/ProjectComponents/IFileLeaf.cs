namespace Stryker.Core.ProjectComponents
{
    public interface IFileLeaf<T> : IProjectComponent
    {
         SyntaxTree { get; set; }

        T MutatedSyntaxTree { get; set; }
    }
}
