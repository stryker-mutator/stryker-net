using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class DiffIgnoreFilePatternsInputTests
    {
        [Fact]
        public void Should()
        {
            var target = new DiffIgnoreFilePatternsInput { SuppliedInput = new[] { "" } };

            var result = target.Validate();

            result.ShouldHaveSingleItem();
        }
    }
}
