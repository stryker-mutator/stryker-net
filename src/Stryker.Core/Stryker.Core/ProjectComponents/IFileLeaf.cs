namespace Stryker.Core.ProjectComponents
{
    public interface IFileLeaf<T> : IFileLeaf
    {
        T SyntaxTree { get; set; }

        T MutatedSyntaxTree { get; set; }
    }
    public interface IFileLeaf : IProjectComponent
    {
        string SourceCode { get; set; }

        ReadOnlyFileLeaf ToReadOnly();
    }
}
