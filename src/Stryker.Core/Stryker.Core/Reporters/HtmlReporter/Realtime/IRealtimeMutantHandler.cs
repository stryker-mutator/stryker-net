using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters.HtmlReporter.Realtime;

public interface IRealtimeMutantHandler
{
    /// <summary>
    /// Opens the Server Sent Event endpoint for the mutation report to listen to.
    /// </summary>
    public void OpenSseEndpoint();

    /// <summary>
    /// Closes the Server Sent Event endpoint.
    /// </summary>
    public void CloseSseEndpoint();

    public void SendMutantResultEvent(IReadOnlyMutant testedMutant);
}
