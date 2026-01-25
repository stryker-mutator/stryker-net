namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

public sealed record InitializeResponse(
    ServerInfo ServerInfo,
    ServerCapabilities Capabilities);
