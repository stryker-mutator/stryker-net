using Stryker.Configuration.Mutants;

namespace Stryker.Configuration.Reporters.Html.RealTime;

public interface IRealTimeMutantHandler
{
    public int Port { get; }

    /// <summary>
    /// Opens the Server Sent Event endpoint for the mutation report to listen to.
    /// </summary>
    public void OpenSseEndpoint();

    /// <summary>
    /// Closes the Server Sent Event endpoint.
    /// </summary>
    public void CloseSseEndpoint();

    public void SendMutantTestedEvent(IReadOnlyMutant testedMutant);
}
