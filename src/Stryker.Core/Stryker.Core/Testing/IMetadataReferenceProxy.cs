using Microsoft.CodeAnalysis;

namespace Stryker.Core.Testing
{
    public interface IMetadataReferenceProvider
    {
        PortableExecutableReference CreateFromFile(string path);
    }
}