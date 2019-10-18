using System.Collections.Generic;
using System.Linq;

namespace TargetProject
{
    public class LinqMagic
    {
        private IList<Student> studentList = new List<Student>() {
                new Student() { StudentID = 1, StudentName = "John", Age = 18, StandardID = 1 } ,
                new Student() { StudentID = 2, StudentName = "Steve",  Age = 21, StandardID = 1 }
            };

        public void FirstExample()
        {
            var allHaveLongNames = studentList.Where(s => s.Age > 18)
                              .Select(s => s)
                              .Where(st => st.StandardID > 0)
                              .Select(s => s.StudentName)
                              .All(x => x.Length > 5);
        }

        public void SecondExample()
        {
            var teenStudentsName = from s in studentList
                                   where s.Age > 12 && s.Age < 20
                                   select new { StudentName = s.StudentName };
        }
    }
}
