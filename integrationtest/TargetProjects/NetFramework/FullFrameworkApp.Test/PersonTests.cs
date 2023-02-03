using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FullFrameworkApp.Test
{
    [TestClass]
    public class PersonTests
    {
        [TestMethod]
        public void PersonCanAge()
        {
            var person = new Person() { Age = 10 };
            var olderPerson = new Person() { Age = 11 };

            Person.Aged(person);

            Assert.IsTrue(person.SameAge(olderPerson));
        }

        [TestMethod]
        public void UseEmbeddedResources()
        {
            var person = new Person() { Age = 10 };
            Assert.AreEqual(person.HelloInMyLanguage(), "hello you");
        }
    }
}
