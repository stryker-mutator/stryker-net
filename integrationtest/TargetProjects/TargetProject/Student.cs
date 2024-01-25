namespace TargetProject
{
    public class Student : Person
    {
        public int StudentID { get; set; }
        public string? StudentName { get; set; }
        public int StandardID { get; set; }

        public string IsExpired()
        {
            StudentName = "";
            return Age > 30 ? "Yes" : "No";
        }

        // Stryker disable all: Because I want to
        public string IsNotExpired()
        {
            return Age < 30 ? "Yes" : "No";
        }
    }
}
