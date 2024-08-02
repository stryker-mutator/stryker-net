using System.IO;
using Shouldly;
using Stryker.Configuration.Baseline.Utils;
using Stryker.Configuration.Reporters.Json;
using Stryker.Configuration.Reporters.Json.SourceFiles;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Stryker.Configuration.UnitTest.Baseline.Utils
{
    [TestClass]
    public class BaselineMutantHelperTests : TestBase
    {
        [TestMethod]
        public void GetMutantSourceShouldReturnMutantSource()
        {
            // Arrange
            var path = $"TestResources{Path.DirectorySeparatorChar}ExampleSourceFile.cs";

            var file = new FileInfo(path);

            var source = File.ReadAllText(file.FullName);

            var jsonMutant = new JsonMutant
            {
                Location = new Location
                {
                    Start = new Position
                    {
                        Column = 17,
                        Line = 16
                    },
                    End = new Position
                    {
                        Column = 62,
                        Line = 16
                    }
                }
            };

            var target = new BaselineMutantHelper();

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe("return Fibonacci(b, a + b, counter + 1, len);");
        }

        [TestMethod]
        public void GetMutantSourceShouldReturnMutantSource_When_Multiple_Lines()
        {
            // Arrange
            var path = $"TestResources{Path.DirectorySeparatorChar}ExampleSourceFile.cs";

            var file = new FileInfo(path);

            var source = File.ReadAllText(file.FullName);

            var jsonMutant = new JsonMutant
            {
                Location = new Location
                {
                    Start = new Position
                    {
                        Column = 13,
                        Line = 23
                    },
                    End = new Position
                    {
                        Column = 38,
                        Line = 25
                    }
                }
            };

            var target = new BaselineMutantHelper();

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe(@"return @""Lorem Ipsum
                    Dolor Sit Amet
                    Lorem Dolor Sit"";");
        }

        [TestMethod]
        public void GetMutantSource_Gets_Partial_Line()
        {
            // Arrange
            var path = $"TestResources{Path.DirectorySeparatorChar}ExampleSourceFile.cs";

            var file = new FileInfo(path);

            var source = File.ReadAllText(file.FullName);

            var jsonMutant = new JsonMutant
            {
                Location = new Location
                {
                    Start = new Position
                    {
                        Column = 30,
                        Line = 32
                    },
                    End = new Position
                    {
                        Column = 34,
                        Line = 32
                    }
                }
            };

            var target = new BaselineMutantHelper();

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe("\"\\n\"");

        }
    }
}
