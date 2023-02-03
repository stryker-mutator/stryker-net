
using System.Collections.Generic;

namespace FullFrameworkApp
{
    public class Person
    {
        public int Age { get; set; }

        public bool Older(Person otherPerson) => Age > otherPerson.Age;

        public bool Younger(Person otherPerson) => Age < otherPerson.Age;

        public bool SameAge(Person otherPerson) => Age == otherPerson.Age;

        public string HelloInMyLanguage() => Translations.hello + (Age > 16 ? " friend" : " you");

        public static void Aged(Person person)
        {
            if (person.Age > 0)
            {
                person.Age++;
            }
        }

        public static IEnumerable<Person> People
        {
            get
            {
                for (var i = 0; i < 10; i++)
                {
                    yield return new Person { Age = i };
                }
            }
        }
    }
}
