namespace Stryker.Core.ProjectComponents
{
    public interface IReadOnlyFileLeaf : IReadOnlyProjectComponent
    {
        string SourceCode { get; }

        bool IsComponentExcluded();

        bool IsMatch(MutantSpan span);
    }

    public interface IFileLeaf<T> : IFileLeaf
    {
        T SyntaxTree { get; set; }

        T MutatedSyntaxTree { get; set; }
    }

    public interface IFileLeaf : IReadOnlyFileLeaf
    {
        new string SourceCode { get; set; }
    }
}
