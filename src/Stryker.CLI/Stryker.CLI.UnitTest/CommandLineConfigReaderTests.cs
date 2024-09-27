using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Shouldly;
using Stryker.CLI.CommandLineConfig;
using Stryker.Core.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var selectedCommand = _app.Parse(new[] { "--dev-mode" }).SelectedCommand;

            _target.ReadCommandLineConfig(selectedCommand, _inputs);

            _inputs.DevModeInput.SuppliedInput.ShouldBe(true);
        }

        [TestMethod]
        public void ShouldHandleSingleValue()
        {
            var selectedCommand = _app.Parse(new[] { "--concurrency 4" }).SelectedCommand;

            _target.ReadCommandLineConfig(selectedCommand, _inputs);

            _inputs.ConcurrencyInput.SuppliedInput.ShouldBe(4);
        }

        [TestMethod]
        public void ShouldHandleSingleOrNoValueWithNoValue()
        {
            var selectedCommand = _app.Parse(new[] { "--since" }).SelectedCommand;

            _target.ReadCommandLineConfig(selectedCommand, _inputs);

            _inputs.SinceInput.SuppliedInput.ShouldBe(true);
            _inputs.SinceTargetInput.SuppliedInput.ShouldBe(null);
        }

        [TestMethod]
        public void ShouldHandleSingleOrNoValueWithValue()
        {
            var selectedCommand = _app.Parse(new[] { "--since:test" }).SelectedCommand;

            _target.ReadCommandLineConfig(selectedCommand, _inputs);

            _inputs.SinceInput.SuppliedInput.ShouldBe(true);
            _inputs.SinceTargetInput.SuppliedInput.ShouldBe("test");
        }

        [TestMethod]
        public void ShouldHandleMultiValue()
        {
            var selectedCommand = _app.Parse(new[] { "--reporter test", "--reporter test2" }).SelectedCommand;

            _target.ReadCommandLineConfig(selectedCommand, _inputs);

            _inputs.ReportersInput.SuppliedInput.Count().ShouldBe(2);
            _inputs.ReportersInput.SuppliedInput.First().ShouldBe("test");
            _inputs.ReportersInput.SuppliedInput.Last().ShouldBe("test2");
        }
    }
}
