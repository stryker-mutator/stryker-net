namespace MsTestRunnerDemo.Models;

public sealed record InitializeResponse(
    ServerInfo ServerInfo,
    ServerCapabilities Capabilities);
