namespace Stryker.Core.Baseline.Providers;
using Stryker.Core.Reporters.Json;
using System.Threading.Tasks;

public interface IBaselineProvider
{
    Task<JsonReport> Load(string version);
    Task Save(JsonReport report, string version);
}
