using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Stryker.Core.Initialisation.Buildalyzer;
using Xunit;

namespace Stryker.Core.UnitTest.ToolHelpers
{
    public class BuildalyzerHelperTests
    {
        [Theory]
        [InlineData("DEBUG;TRACE")]
        [InlineData("DEBUG;TRACE;")]
        public void ShouldGetDefineConstants(string value)
        {
            var projectAnalyzerResult = TestHelper.SetupProjectAnalyzerResult(
                    properties: new Dictionary<string, string>() {
                        { "DefineConstants", value }
                    }).Object;

            var result = projectAnalyzerResult.GetDefineConstants();

            result.Count.ShouldBe(2);
            result.First().ShouldBe("DEBUG");
            result.Last().ShouldBe("TRACE");
        }
    }
}
