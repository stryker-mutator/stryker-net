using Spectre.Console;
using Stryker.Core;
using Stryker.Core.Exceptions;

namespace Stryker.CLI;

public static class Program
{
    public static int Main(string[] args)
    {
        try
        {
            var app = new StrykerCli();
            return app.Run(args);
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
