using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Core.Mutants;

namespace Stryker.Core.ProjectComponents;

public class Solution : ProjectComponent, IReadOnlyFolderComposite
{
    private readonly IList<IProjectComponent> _children = new List<IProjectComponent>();

    public IEnumerable<IProjectComponent> Children => _children;

    public override IEnumerable<Mutant> Mutants
    {
        get => Children.SelectMany(x => x.Mutants);
        set => throw new NotSupportedException("Folders do not contain mutants.");
    }

    public void Add(IProjectComponent child) => _children.Add(child);

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

    public override IEnumerable<IFileLeaf> GetAllFiles() => Children.SelectMany(x => x.GetAllFiles());
}
