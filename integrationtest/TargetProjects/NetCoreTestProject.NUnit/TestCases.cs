using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NUnit.Framework;
using TargetProject;

namespace NetCoreTestProject.NUnit
{
    [TestFixture]
    internal class TestCases
    {
        // explore the various ways to have test cases
        // explicit
        [Test]
        [TestCase(29, "No")]
        [TestCase(32, "Yes")]
        public void TestAgeExplicit(int age, string expired)
        {
            var sut = new Student{Age = age};
            Assert.AreEqual(expired, sut.IsExpired());
        }

        // indirect
        private static IEnumerable<(int, string)> TupleSource()
        {
            yield return (29, "No");
            yield return (31, "Yes");
        }

        private static IEnumerable<Student> ObjectSource()
        {
            yield return new Student {Age = 32};
            yield return new Student {Age = 42};
        }

        [Test]
        [TestCaseSource(nameof(TupleSource))]
        public void TestAgeIndirectTuple((int, string) test)
        {
            var (AgeAsync, ExpiredAsync) = test;
            var sut = new Student{Age = AgeAsync};
            Assert.AreEqual(ExpiredAsync, sut.IsExpired());
        }

        [Test]
        [TestCaseSource(nameof(ObjectSource))]
        // all test cases refer to the same test
        public void TestAgeIndirectObject(Student sut)
        {
            Assert.AreEqual("Yes", sut.IsExpired());
        }

        private static IEnumerable<int> RandomSource()
        {
            var rnd = new Random();
            for (var i = 0; i < 10; i++)
            {
                Console.Error.WriteLine($"Random{i}");
                yield return rnd.Next();
            }
        }

        [Test]
        [TestCaseSource(nameof(RandomSource))]
        [Ignore("Run explicitly")]
        public void TestRandom(int x)
        {
            Assert.IsTrue(x % 2 == 0);
        }
    }
}
