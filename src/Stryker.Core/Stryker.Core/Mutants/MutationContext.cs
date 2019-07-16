using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Stryker.Core.Mutants
{
    internal class MutationContext
    {
        public bool InStaticValue { get; set; }
    }
}