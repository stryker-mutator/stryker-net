using System;
using System.Linq;
using Stryker.Core.ToolHelpers;
using Xunit;

namespace Stryker.Core.UnitTest.ToolHelpers
{
    public class VsTestHelperTests
    {
        [Fact]
        public void MicrosoftTestPlatformPortableExistsAsEmbededResourse()
        {
            var vsTestPortableBinaryExists = typeof(VsTestHelper).Assembly
                .GetManifestResourceNames()
                .Any(x => x.Contains("Microsoft.TestPlatform.Portable", StringComparison.OrdinalIgnoreCase));
            Assert.True(vsTestPortableBinaryExists);
        }
    }
}
