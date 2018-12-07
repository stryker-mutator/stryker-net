using Shouldly;
using Stryker.Core.Reporters;
using Stryker.Core.Options;
using System;
using System.Collections.Generic;

using Xunit;

namespace Stryker.Core.UnitTest.Reporters
{
    public class ReporterFactoryTests
    {
        public static IEnumerable<object[]> getParameters()
        {
            yield return new object[] {new StrykerOptions(), typeof(BroadcastReporter)};
        }

        [Theory]
        [MemberData(nameof(getParameters))]
        public void ReporterFactory_NameShouldCreateJustReporterType(StrykerOptions options, Type type)
        {
            var result = ReporterFactory.Create(options);
            result.ShouldBeOfType(type);
        }
    }
}
