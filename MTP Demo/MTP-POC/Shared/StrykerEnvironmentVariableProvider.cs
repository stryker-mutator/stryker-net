using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;

namespace TestProject1;

public class StrykerEnvironmentVariableProvider : ITestHostEnvironmentVariableProvider
{

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public string Uid => "StrykerEnvironmentVariableProvider";
    public string Version => "1.0";
    public string DisplayName => "Stryker Environment Variable Provider";
    public string Description => "Provides environment variables for Stryker";

    public Task UpdateAsync(IEnvironmentVariables environmentVariables)
    {
        var test = environmentVariables;
        // environmentVariables["StrykerActiveMutation"] = Environment.GetEnvironmentVariable("StrykerActiveMutation") ?? string.Empty;
        return Task.CompletedTask;
    }

    public Task<ValidationResult> ValidateTestHostEnvironmentVariablesAsync(IReadOnlyEnvironmentVariables environmentVariables) => Task.FromResult(ValidationResult.Valid());
}