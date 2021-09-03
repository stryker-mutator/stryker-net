using System;
using Crayon;
using Stryker.Core;
using Stryker.Core.Exceptions;

namespace Stryker.CLI
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                var app = new StrykerCli();
                return app.Run(args);
            }
            catch (InputException strEx)
            {
                Output.Yellow("Stryker.NET failed to mutate your project. For more information see the logs below:");
                Console.WriteLine(strEx.ToString());
                return ExitCodes.OtherError;
            }
        }
    }
}
