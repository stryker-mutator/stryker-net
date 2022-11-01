namespace TargetProject
{
    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public Person Father { get; set; }

        public Person() : this(true)
        {

        }

        public Person(bool adult)
        {
            if (adult == true)
            {
                Age = 18;
            }
            else
            {
                Age = 16;
            }
        }

        public void BirthDay()
        {
            Age++;
        }

        //public string BestFriendName()
        //{
        //    return BestFriend?.Name;
        //}

        //public Person ThreeConditionalAccessInARow()
        //{
        //    return Father?.Father?.Father?.Father;
        //}

        public Person SimpleMemberAccessFollowedByTwoConditionalAccess()
        {
            return Father.Father?.Father?.Father;
        }

        //public string GetBestFriendsBestFriendName()
        //{
        //    return BestFriend?.BestFriendName();
        //}
    }
}
