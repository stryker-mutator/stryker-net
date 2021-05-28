using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Stryker.Core.Exceptions;
using Stryker.Core.Options.Inputs;
using Xunit;

namespace Stryker.Core.UnitTest.Options.Inputs
{
    public class BasePathInputTests
    {
        [Fact]
        public void ShouldAllowExistingDir()
        {
            var target = new BasePathInput { SuppliedInput = "C:/MyDir/" };
            var fileSystemMock = new MockFileSystem();
            fileSystemMock.AddDirectory("C:/MyDir/");

            var result = target.Validate(fileSystemMock);

            result.ShouldBe("C:/MyDir/");
        }

        [Fact]
        public void ShouldThrowOnNonexistingDir()
        {
            var target = new BasePathInput { SuppliedInput = "C:/MyDir/" };
            var fileSystemMock = new MockFileSystem();

            var exception = Should.Throw<InputException>(() => target.Validate(fileSystemMock));

            exception.Message.ShouldBe("Base path must exist.");
        }

        [Fact]
        public void ShouldThrowOnNull()
        {
            var target = new BasePathInput { SuppliedInput = null };
            var fileSystemMock = new MockFileSystem();

            var exception = Should.Throw<InputException>(() => target.Validate(fileSystemMock));

            exception.Message.ShouldBe("Base path cannot be null.");
        }
    }
}
