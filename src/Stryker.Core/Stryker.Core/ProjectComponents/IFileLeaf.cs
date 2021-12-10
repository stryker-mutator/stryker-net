namespace Stryker.Core.ProjectComponents
{
    public interface IReadOnlyFileLeaf<T> : IReadOnlyProjectComponent, IReadOnlyFileLeaf
    {
        T SyntaxTree { get; }

        T MutatedSyntaxTree { get; }
    }

    public interface IReadOnlyFileLeaf : IReadOnlyProjectComponent
    {
        string SourceCode { get; }
    }

    public interface IFileLeaf<T> : IReadOnlyFileLeaf<T>, IFileLeaf
    {
        new T SyntaxTree { get; set; }

        new T MutatedSyntaxTree { get; set; }
    }

    public interface IFileLeaf : IReadOnlyFileLeaf
    {
        new string SourceCode { get; set; }
    }
}
