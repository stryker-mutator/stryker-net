namespace TargetProject
{
    public class Student
    {
        public int StudentID { get; set; }
        public string? StudentName { get; set; }
        public int Age { get; set; }
        public int StandardID { get; set; }

        public string IsExpired()
        {
            return Age > 30 ? "Yes" : "No";
        }
    }
}
