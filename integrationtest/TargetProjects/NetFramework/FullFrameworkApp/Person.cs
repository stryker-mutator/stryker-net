
using System.Collections.Generic;

namespace FullFrameworkApp
{
    public class Person
    {
        public int Age { get; set; }

        public bool Older(Person otherPerson)
        {
            return Age > otherPerson.Age;
        }

        public bool Younger(Person otherPerson)
        {
            return Age < otherPerson.Age;
        }

        public bool SameAge(Person otherPerson)
        {
            return Age == otherPerson.Age;
        }

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
                for (int i = 0; i < 10; i++)
                {
                    yield return new Person { Age = i };
                }
            }
        }
    }
}
