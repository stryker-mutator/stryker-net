using Stryker.Core.Mutants;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stryker.Core.ProjectComponents
{
    public interface IFileLeaf<T> : IReadOnlyInputComponent
    {
        public T SyntaxTree { get; set; }

        public T MutatedSyntaxTree { get; set; }
    }
}
