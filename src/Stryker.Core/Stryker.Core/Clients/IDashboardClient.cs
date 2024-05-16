using Stryker.Core.Reporters.Json.SourceFiles;
using Stryker.Core.Reporters.Json;
using System.Threading.Tasks;

namespace Stryker.Core.Clients;
public interface IDashboardClient
{
    Task<string> PublishReport(JsonReport report, string version, bool realTime = false);
    Task<JsonReport> PullReport(string version);
    Task PublishMutantBatch(JsonMutant mutant);
    Task PublishFinished();
}
