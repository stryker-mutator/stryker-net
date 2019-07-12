﻿using Stryker.Core;
using Stryker.Core.Exceptions;
using Stryker.Core.Testing;
using System;

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
            catch (StrykerInputException strEx)
            {
                new Chalk().Yellow("Stryker.NET failed to mutate your project. For more information see the logs below:");
                Console.WriteLine(strEx.ToString());
                return 1;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
