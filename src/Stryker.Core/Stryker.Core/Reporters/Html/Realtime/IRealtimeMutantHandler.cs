namespace Stryker.Core.Reporters.Html.Realtime;
using Stryker.Core.Mutants;

public interface IRealtimeMutantHandler
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
