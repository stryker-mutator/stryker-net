using System.Threading.Tasks;

namespace Stryker.Core.DiffProviders
{
    public interface IDiffProvider
    {
        DiffResult ScanDiff();
    }
}