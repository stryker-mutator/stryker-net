using System.IO;
using Shouldly;
using Stryker.Core.Baseline.Utils;
using Stryker.Core.Reporters.Json;
using Stryker.Core.Reporters.Json.SourceFiles;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline.Utils
{
    public class BaselineMutantHelperTests : TestBase
    {
        [Fact]
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
                        Line = 17
                    },
                    End = new Position
                    {
                        Column = 62,
                        Line = 17
                    }
                }
            };

            var target = new BaselineMutantHelper();

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe("return Fibonacci(b, a + b, counter + 1, len);");
        }

        [Fact]
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
                        Line = 24
                    },
                    End = new Position
                    {
                        Column = 38,
                        Line = 26
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

        [Fact]
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
                        Line = 34
                    },
                    End = new Position
                    {
                        Column = 34,
                        Line = 34
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
