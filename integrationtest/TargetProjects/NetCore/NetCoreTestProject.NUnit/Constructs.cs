using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using TargetProject.StrykerFeatures;

namespace NetCoreTestProject.NUnit
{
    [TestFixture]
    internal class Constructs
    {
        // explore the various ways to have test cases
        // explicit
        [Test]
        [TestCase(29, "No")]
        [TestCase(32, "Yes")]
        public void TestAgeExplicit(int age, string expired)
        {
            var sut = new KilledMutants { Age = age };
            ClassicAssert.AreEqual(expired, sut.IsExpired());
        }

        // indirect
        private static IEnumerable<(int, string)> TupleSource()
        {
            Console.WriteLine("TupleSource:29");
            yield return (29, "No");
            Console.WriteLine("TupleSource:31");
            yield return (31, "Yes");
        }

        private static IEnumerable<KilledMutants> ObjectSource()
        {
            Console.WriteLine("ObjectSource:32");
            yield return new KilledMutants { Age = 32 };
            Console.WriteLine("ObjectSource:42");
            yield return new KilledMutants { Age = 42 };
        }

        [Test]
        [TestCaseSource(nameof(TupleSource))]
        public void TestAgeIndirectTuple((int, string) test)
        {
            var (AgeAsync, ExpiredAsync) = test;
            var sut = new StrykerComments { Age = AgeAsync };
            ClassicAssert.AreEqual(ExpiredAsync, sut.IsExpired());
        }

        [Test]
        [TestCaseSource(nameof(ObjectSource))]
        // all test cases refer to the same test
        public void TestAgeIndirectObject(KilledMutants sut)
        {
            Console.WriteLine($"ObjectSource test:{sut.Age}");
            ClassicAssert.AreEqual(sut.Age > 29 ? "Yes" : "No", sut.IsExpired());
        }

        private static IEnumerable<int> RandomSource()
        {
            var rnd = TestContext.CurrentContext.Random;
            for (var i = 0; i < 5; i++)
            {
                Console.WriteLine($"RandomSource:{i}");
                yield return rnd.Next();
            }
        }

        [Test]
        [TestCaseSource(nameof(RandomSource))]
        [Ignore("Run explicitly")]
        public void TestRandom(int x)
        {
            var sut = new KilledMutants { Age = 32 };
            ClassicAssert.AreEqual("Yes", sut.IsExpired());
        }
    }
}
