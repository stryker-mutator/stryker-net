using Microsoft.Extensions.Logging;
using Stryker.Core.Logging;

namespace Stryker.Core.Reporters.Progress
{
    public interface IConsoleOneLineLoggerFactory
    {
        IConsoleOneLineLogger Create();
    }

    public class ConsoleOneLineLoggerFactory : IConsoleOneLineLoggerFactory
    {
        public IConsoleOneLineLogger Create()
        {
            return new ConsoleOneLineLogger(ApplicationLogging.LoggerFactory.CreateLogger<ConsoleOneLineLogger>());
        }
    }
}