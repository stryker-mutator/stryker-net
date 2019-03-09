
namespace FullFrameworkApp
{
    public class Person
    {
        public int Age { get; set; }

        public void Aged()
        {
            if (Age > 0)
            {
                Age++;
            }
        }
    }
}
