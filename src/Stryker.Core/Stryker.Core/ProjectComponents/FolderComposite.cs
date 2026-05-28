using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.ProjectComponents;

public class FolderComposite : ProjectComponent, IFolderComposite
{
    private readonly List<ProjectComponent> _children = [];
    private readonly List<SyntaxTree> _compilationSyntaxTrees = [];

    public IEnumerable<IReadOnlyProjectComponent> Children => _children;

    public override IEnumerable<IMutant> Mutants
    {
        get => Children.SelectMany(x => x.Mutants);
        set => throw new NotSupportedException("Folders do not contain mutants.");
    }

    /// <summary>
    /// Add a syntax tree to this folder that is needed in compilation but should not be mutated
    /// </summary>
    public void AddCompilationSyntaxTree(SyntaxTree syntaxTree) => _compilationSyntaxTrees.Add(syntaxTree);
    public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => _compilationSyntaxTrees.Union(ChildCompilationSyntaxTree);
    private IEnumerable<SyntaxTree> ChildCompilationSyntaxTree => _children.SelectMany(c => c.CompilationSyntaxTrees);
    public override IEnumerable<SyntaxTree> MutatedSyntaxTrees => _children.SelectMany(c => c.MutatedSyntaxTrees);

    public void Add(IProjectComponent child)
    {
        if (child is not ProjectComponent projectComponent)
        {
            // accepts only same type
            throw new ArgumentException("Only ProjectComponent instances can be added to the solution.");
        }

        projectComponent.Parent = this;
        _children.Add(projectComponent);
    }

    public void AddRange(IEnumerable<IProjectComponent> children)
    {
        foreach (var child in children)
        {
            Add(child);
        }
    }

    public override IEnumerable<IFileLeaf> GetAllFiles() => Children.SelectMany(x => x.GetAllFiles());
    public override void Display()
    {
        // only walk this branch of the tree if it belongs to the source project, otherwise we have nothing to display.
        if (MutatedSyntaxTrees.Any())
        {
            DisplayFolder(this);

            foreach (var child in Children)
            {
                child.DisplayFile = DisplayFile;
                child.DisplayFolder = DisplayFolder;
                child.Display();
            }
        }
    }
}
