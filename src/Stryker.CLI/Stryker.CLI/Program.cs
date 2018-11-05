using System;
using Stryker.Core;
using Stryker.Core.Exceptions;

namespace Stryker.CLI
{
    public class Program
    {
        static int Main(string[] args)
        {
            try
            {
                var stryker = new StrykerRunner();
                var app = new StrykerCLI(stryker);
                return app.Run(args);
            }
            catch(StrykerInputException strEx)
            {
                Chalk.Yellow("Stryker.NET failed to mutate your project. For more information see the logs below:");
                Console.WriteLine(strEx.ToString());
                return 1;
            }
            catch(Exception)
            {
                return 1;
            }
        }
    }
}
