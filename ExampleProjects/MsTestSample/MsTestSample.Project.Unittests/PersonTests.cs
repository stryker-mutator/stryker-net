namespace MsTestSample.Project.Unittests;

[TestClass]
public class PersonTests
{
    [TestMethod]
    [DataRow(10, 11)]
    [DataRow(12, 13)]
    [DataRow(13, 14)]
    public void PersonCanAge(int ageA, int ageB)
    {
        var person = new Person() { Age = ageA };
        var olderPerson = new Person() { Age = ageB };

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
