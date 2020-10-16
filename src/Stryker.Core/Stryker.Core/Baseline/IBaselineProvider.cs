using Stryker.Core.Reporters.Json;
using System.Threading.Tasks;

namespace Stryker.Core.Baseline
{
    public interface IBaselineProvider
    {
        Task<JsonReport> Load(string version);
        Task Save(JsonReport report, string version);
    }
}
