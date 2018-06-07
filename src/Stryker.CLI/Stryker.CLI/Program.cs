using Stryker.Core;

namespace Stryker.CLI
{
    public class Program
    {
        static void Main(string[] args)
        {
            var stryker = new StrykerRunner();
            var app = new StrykerCLI(stryker);
            app.Run(args);
        }
    }
}
