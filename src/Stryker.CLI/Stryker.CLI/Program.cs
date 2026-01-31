using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using Stryker.Abstractions.Exceptions;
using Stryker.CLI.Infrastructure;
using Stryker.CLI.Logging;
using Stryker.Configuration;
using Stryker.Core.Infrastructure;

namespace Stryker.CLI;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            // Build DI container
            var services = new ServiceCollection()
                .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                .AddStrykerCore()
                .AddStrykerCli()
                .BuildServiceProvider();
            // ensure the logger Factory instance is shared
            ApplicationLogging.LoggerFactory = services.GetRequiredService<ILoggerFactory>();
            var app = services.GetRequiredService<StrykerCli>();
            return await app.RunAsync(args);
        }
        catch (NoTestProjectsException exception)
        {
            AnsiConsole.WriteLine(exception.Message);
            return ExitCodes.Success;
        }
        catch (InputException exception)
        {
            AnsiConsole.MarkupLine("[Yellow]Stryker.NET failed to mutate your project. For more information see the logs below:[/]");
            AnsiConsole.WriteLine(exception.ToString());
            return ExitCodes.OtherError;
        }
    }
}
