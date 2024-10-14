using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using Stryker.Abstractions.Options;
using Stryker.CLI.CommandLineConfig;

namespace Stryker.CLI.UnitTest
{
    [TestClass]
    public class CommandLineConfigReaderTests
    {
        private readonly CommandLineApplication _app = new CommandLineApplication
        {
            Name = "Stryker",
            FullName = "Stryker: Stryker mutator for .Net",
            Description = "Stryker mutator for .Net",
            ExtendedHelpText = "Welcome to Stryker for .Net! Run dotnet stryker to kick off a mutation test run or run dotnet stryker init to start configuring your project."
        };
        private readonly IStrykerInputs _inputs = new StrykerInputs();
        private readonly CommandLineConfigReader _target = new();

        public CommandLineConfigReaderTests() => _target.RegisterCommandLineOptions(_app, _inputs);

        [TestMethod]
        public void ShouldHandleNoValue()
        {
            _target.ReadCommandLineConfig(new[] { "--dev-mode" }, _app, _inputs);

            _inputs.DevModeInput.SuppliedInput.ShouldBe(true);
        }

        [TestMethod]
        public void ShouldHandleSingleValue()
        {
            _target.ReadCommandLineConfig(new[] { "--concurrency 4" }, _app, _inputs);

            _inputs.ConcurrencyInput.SuppliedInput.ShouldBe(4);
        }

        [TestMethod]
        public void ShouldHandleSingleOrNoValueWithNoValue()
        {
            _target.ReadCommandLineConfig(new[] { "--since" }, _app, _inputs);

            _inputs.SinceInput.SuppliedInput.ShouldBe(true);
            _inputs.SinceTargetInput.SuppliedInput.ShouldBe(null);
        }

        [TestMethod]
        public void ShouldHandleSingleOrNoValueWithValue()
        {
            _target.ReadCommandLineConfig(new[] { "--since:test" }, _app, _inputs);

            _inputs.SinceInput.SuppliedInput.ShouldBe(true);
            _inputs.SinceTargetInput.SuppliedInput.ShouldBe("test");
        }

        [TestMethod]
        public void ShouldHandleMultiValue()
        {
            _target.ReadCommandLineConfig(new[] { "--reporter test", "--reporter test2" }, _app, _inputs);

            _inputs.ReportersInput.SuppliedInput.Count().ShouldBe(2);
            _inputs.ReportersInput.SuppliedInput.First().ShouldBe("test");
            _inputs.ReportersInput.SuppliedInput.Last().ShouldBe("test2");
        }
    }
}
