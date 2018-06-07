using Shouldly;
using Stryker.Core.Reporters;
using System;
using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ReporterFactoryTests
    {
        [Theory]
        [InlineData("RapportOnly", typeof(ConsoleRapportReporter))]
        [InlineData("Console", typeof(BroadcastReporter))]
        [InlineData("someRandomName", typeof(BroadcastReporter))]
        [InlineData("", typeof(BroadcastReporter))]
        public void ReporterFactory_NameShouldCreateJustReporterType(string name, Type type)
        {
            var result = ReporterFactory.Create(name);
            result.ShouldBeOfType(type);
        }
    }
}
