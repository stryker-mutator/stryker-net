using Stryker.Core.Reporters.Json;
using System.Threading.Tasks;

namespace Stryker.Core.Baseline.Providers
{
    public interface IBaselineProvider
    {
        Task<JsonReport> Load(string version);
        Task Save(JsonReport report, string version);
    }
}
