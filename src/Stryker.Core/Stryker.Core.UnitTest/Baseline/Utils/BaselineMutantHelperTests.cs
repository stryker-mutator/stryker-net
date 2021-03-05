using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Moq;
using Shouldly;
using Stryker.Core.Baseline.Utils;
using Stryker.Core.Reporters.Json;
using Xunit;

namespace Stryker.Core.UnitTest.Baseline.Utils
{
    public class BaselineMutantHelperTests
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
                Location = new JsonMutantLocation(new JsonMutantPosition
                {
                    Column = 17,
                    Line = 17
                },
                new JsonMutantPosition
                {
                    Column = 62,
                    Line = 17
                }),
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
                Location = new JsonMutantLocation(new JsonMutantPosition
                {
                    Column = 13,
                    Line = 24
                },
                new JsonMutantPosition
                {
                    Column = 38,
                    Line = 26
                }),
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
                Location = new JsonMutantLocation(new JsonMutantPosition
                {
                    Column = 30,
                    Line = 34
                },
                new JsonMutantPosition
                {
                    Column = 34,
                    Line = 34
                }),
            };

            var target = new BaselineMutantHelper();

            // Act
            var result = target.GetMutantSourceCode(source, jsonMutant);

            // Assert
            result.ShouldBe("\"\\n\"");

        }
    }
}
