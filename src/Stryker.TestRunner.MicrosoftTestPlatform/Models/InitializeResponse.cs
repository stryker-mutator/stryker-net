using System.Diagnostics.CodeAnalysis;

namespace Stryker.TestRunner.MicrosoftTestPlatform.Models;

[ExcludeFromCodeCoverage]
public sealed record InitializeResponse(
    ServerInfo ServerInfo,
    ServerCapabilities Capabilities);
