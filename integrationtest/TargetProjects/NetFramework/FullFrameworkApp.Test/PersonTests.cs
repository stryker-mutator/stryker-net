using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FullFrameworkApp.Test
{
    [TestClass]
    public class PersonTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var person = new Person() { Age = 10 };

            person.Aged();

            Assert.AreEqual(person.Age, 11);
        }
    }
}
