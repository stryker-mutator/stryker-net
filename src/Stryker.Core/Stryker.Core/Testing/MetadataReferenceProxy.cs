using Microsoft.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;

namespace Stryker.Core.Testing
{
    public interface IMetadataReferenceProvider
    {
        PortableExecutableReference CreateFromFile(string path);
    }
    
    [ExcludeFromCodeCoverage]
    public class MetadataReferenceProvider : IMetadataReferenceProvider
    {
        public PortableExecutableReference CreateFromFile(string path)
        {
            return MetadataReference.CreateFromFile(path);
        }
    }
}
