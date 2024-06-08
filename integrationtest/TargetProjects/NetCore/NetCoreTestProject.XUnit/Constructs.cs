using System;
using System.Collections.Generic;
using TargetProject.StrykerFeatures;
using Xunit;
using Xunit.Abstractions;

namespace ExampleProject.XUnit
{
    public class Constructs
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public Constructs(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        // explore the various ways to have test cases
        // explicit
        [Theory]
        [InlineData(29, false)]
        [InlineData(30, true)]
        public void TestAgeExplicit(int age, bool expired)
        {
            var sut = new KilledMutants { Age = age};
            _testOutputHelper.WriteLine($"Tuplesource test:{sut.Age}");
            Assert.Equal(expired, sut.IsExpiredBool());
        }

        // indirect
        public static IEnumerable<object[]> TupleSource()
        {
            Console.WriteLine("TupleSource:29");
            yield return new object[]{29, "No"};
            Console.WriteLine("TupleSource:31");
            yield return new object[]{31, "Yes"};
        }

        public static IEnumerable<object[]> ObjectSource()
        {
            Console.WriteLine("ObjectSource:32");
            yield return new object[] {new KilledMutants { Age = 32}};
            Console.WriteLine("ObjectSource:42");
            yield return new object[] {new KilledMutants { Age = 42}};
        }

        public static IEnumerable<object[]> TheSource()
        {
            var student = new KilledMutants { Age = 22 };
            Console.WriteLine("ObjectSource:22");
            yield return new object[] { student.IsExpired(), "No" };
            student = new KilledMutants { Age = 42 };
            Console.WriteLine("ObjectSource:42");
            yield return new object[] { student.IsExpired(), "Yes" };
        }

        [Theory]
        [MemberData(nameof(TupleSource))]
        public void TestAgeIndirectTuple(int age, string expired)
        {
            var sut = new KilledMutants { Age = age};
            _testOutputHelper.WriteLine($"Tuplesource test:{ sut.Age }");
            Assert.Equal(expired, sut.IsExpired());
        }

        [Theory]
        [MemberData(nameof(TheSource))]
        public void TestAgeIndirectFullSource(string actual, string expected)
        {
            _testOutputHelper.WriteLine($"TheSource test:{ expected }");
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(ObjectSource))]
        // all test cases refer to the same test
        public void TestAgeIndirectObject(KilledMutants sut)
        {
            _testOutputHelper.WriteLine($"ObjectSource test:{ sut.Age }");
            Assert.Equal("Yes", sut.IsExpired());
        }

        public static IEnumerable<object[]> RandomSource()
        {
            var rnd = new Random();
            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"Random{i}");
                yield return  new []{(object) rnd.Next()};
            }
        }

        [Theory(DisplayName = "test", Skip = "Run Explicitly")]
        [MemberData(nameof(RandomSource))]
        public void TestRandom(int x)
        {
            _testOutputHelper.WriteLine($"Randomsource test: {x}");
        }
    }
}
