using System;

namespace TargetProject
{
    public class Person
    {
        public int Age { get; set; }

        public Person() : this(true)
        {

        }

        public Person(bool adult)
        {
            if (adult == true)
            {
                Age = 18;
            } else
            {
                Age = 16;
            }
        }

        public void BirthDay()
        {
            Age++;
        }
    }
}
