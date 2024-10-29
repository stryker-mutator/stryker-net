using System;
using System.Collections.Generic;
using System.Linq;
using Stryker.Abstractions.Mutants;
using Stryker.Abstractions.ProjectComponents;

namespace Stryker.Core.ProjectComponents;

public class Solution : ProjectComponent, IFolderComposite
{
    private readonly IList<IReadOnlyProjectComponent> _children = new List<IReadOnlyProjectComponent>();

    public IEnumerable<IReadOnlyProjectComponent> Children => _children;

    public override IEnumerable<IMutant> Mutants
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
