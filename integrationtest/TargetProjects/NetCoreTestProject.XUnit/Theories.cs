using System;
using System.Collections.Generic;
using TargetProject;
using Xunit;

namespace ExampleProject.XUnit
{
    public class Theories
    {
        // explore the various ways to have test cases
        // explicit
        [Theory]
        [InlineData(29, "No")]
        [InlineData(32, "Yes")]
        public void TestAgeExplicit(int age, string expired)
        {
            var sut = new Student{Age = age};
            Assert.Equal(expired, sut.IsExpired());
        }

        // indirect
        public static IEnumerable<object[]> TupleSource()
        {
            yield return new object[]{29, "No"};
            yield return new object[]{31, "Yes"};
        }

        public static IEnumerable<object[]> ObjectSource()
        {
            yield return new object[] {new Student {Age = 32}};
            yield return new object[] {new Student {Age = 42}};
        }

        [Theory]
        [MemberData(nameof(TupleSource))]
        public void TestAgeIndirectTuple(int age, string expired)
        {
            var sut = new Student{Age = age};
            Assert.Equal(expired, sut.IsExpired());
        }

        [Theory]
        [MemberData(nameof(ObjectSource))]
        // all test cases refer to the same test
        public void TestAgeIndirectObject(Student sut)
        {
            Assert.Equal("Yes", sut.IsExpired());
        }

        public static IEnumerable<object[]> RandomSource()
        {
            var rnd = new Random();
            for (var i = 0; i < 10; i++)
            {
                Console.Error.WriteLine($"Random{i}");
                yield return  new []{(object) rnd.Next()};
            }
        }

        [Theory(DisplayName = "test", Skip = "Run Explicitly")]
        [MemberData(nameof(RandomSource))]
        public void TestRandom(int x)
        {
            Assert.True(x % 2 == 0);
        }
    }
}
