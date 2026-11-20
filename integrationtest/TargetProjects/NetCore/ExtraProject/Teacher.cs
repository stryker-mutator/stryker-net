namespace ExtraProject
{
    public class Teacher
    {
        public int Lessons { get; set; } = 0;

        public void AddLesson()
        {
            // Goes through a second mutated assembly on purpose, so that one test covers mutated code in
            // both of them: that is when a test host holds two copies of the injected MutantControl which
            // both have coverage to report for the same test.
            Lessons = ExtraLibrary.Lesson.Next(Lessons);
        }
    }
}
