namespace TargetProject
{
    public class Student
    {
        public int StudentID { get; set; }
        public string? StudentName { get; set; }
        public int Age { get; set; }
        public int StandardID { get; set; }
        private readonly bool isExpired = false;

        public string IsExpired()
        {
            return isExpired ? "" : "1";
        }
    }
}
