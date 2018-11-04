using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Stryker.Core.Logging;

namespace Stryker.Core.Reporters.Progress
{
    public interface IConsoleOneLineLoggerFactory
    {
        IConsoleOneLineLogger Create();
    }

    public class ConsoleOneLineLoggerFactory : IConsoleOneLineLoggerFactory
    {
        private LoggerFactory _factory { get; set; }

        public ConsoleOneLineLoggerFactory()
        {
            _factory = new LoggerFactory();
            _factory.AddSerilog(new LoggerConfiguration()
                .WriteTo.Console(outputTemplate: "{Message:lj}{NewLine}")
                .CreateLogger());
        }

        public IConsoleOneLineLogger Create()
        {
            return new ConsoleOneLineLogger(_factory.CreateLogger<ConsoleOneLineLogger>());
        }
    }
}