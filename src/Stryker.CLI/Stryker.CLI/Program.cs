using Stryker.Core;

namespace Stryker.CLI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var stryker = new StrykerRunner();
            var app = new StrykerCLI(stryker);
            return app.Run(args);
        }
    }
}
