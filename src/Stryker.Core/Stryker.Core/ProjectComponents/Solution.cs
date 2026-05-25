using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Stryker.Abstractions;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.ProjectComponents;

public class Solution : ProjectComponent, IFolderComposite
{
    private readonly IList<ProjectComponent> _children = new List<ProjectComponent>();

    public IEnumerable<IReadOnlyProjectComponent> Children => _children;

    public override IEnumerable<IMutant> Mutants
    {
        get => Children.SelectMany(x => x.Mutants);
        set => throw new NotSupportedException("Folders do not contain mutants.");
    }

    public void Add(IProjectComponent child)
    {
        if (child is not ProjectComponent projectComponent)
        {
            return;
        }
        _children.Add(projectComponent);
    }

    public void AddRange(IEnumerable<IProjectComponent> children)
    {
        foreach (var child in children)
        {
            Add(child);
        }
    }

    public override void Display()
    {
        foreach (var child in Children)
        {
            child.DisplayFile = DisplayFile;
            child.DisplayFolder = DisplayFolder;
            child.Display();
        }
    }

    public override IEnumerable<SyntaxTree> CompilationSyntaxTrees => _children.SelectMany(c => c.CompilationSyntaxTrees);
    public override IEnumerable<SyntaxTree> MutatedSyntaxTrees=> _children.SelectMany(c => c.MutatedSyntaxTrees);

    public override IEnumerable<IFileLeaf> GetAllFiles() => Children.SelectMany(x => x.GetAllFiles());
}
