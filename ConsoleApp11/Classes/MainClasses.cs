using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace School.Classes
{
    #region People
    [Serializable()]
    internal abstract class Human 
    {
        public string Name { get; private set; }
        public string Surname { get; private set; }

        internal Human(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }
    }

    [Serializable()]
    internal class Student : Human, IComparable<Student> 
    {
        public Dictionary<string, List<float>> LessonsMark { get; private set; } = new Dictionary<string, List<float>>();
        internal Student(string name, string surname) : base(name, surname) { }

        public void AddLesson(string lessonName)
        {
            if (LessonsMark[lessonName] == null)
                LessonsMark.Add(lessonName, new List<float>());
        }

        public void RateAndRerateLession(string lessonName, float mark, uint quarter)
        {
            if (LessonsMark[lessonName] == null)
                return;

            float realMark = mark > 2 ? mark < 5 ? mark : 5 : 2;
            List<float> lessionRate = LessonsMark[lessonName];
            if (quarter - 1 == lessionRate.Count)
                lessionRate.Add(realMark);
            else
                lessionRate[(int)quarter - 1] = realMark;
        }

        public void DeleteLesson(string name) => LessonsMark.Remove(name);
        public List<float> GetMarks(string lesson)
        {
            if (LessonsMark[lesson] == null)
                return null;
            return LessonsMark[lesson];
        }

        public void SetMarks(Dictionary<string, List<float>> lessons)
        {

        }

        public int CompareTo(Student other)
        {
            string fullName = Surname + Name;
            return fullName.CompareTo(other.Surname + other.Name);
        }
    }

    [Serializable()]
    internal class Teacher : Human
    {
        private Dictionary<string, List<Class>> _lessons = new Dictionary<string, List<Class>>();
        public uint _mainClassNum { get; private set; }
        internal Teacher(string name, string surname, uint mainClassNum) : base(name, surname) 
            => _mainClassNum= mainClassNum;

        public void AddClassOrLesson(string lesson, Class lessonClass)
        {
            if (lessonClass == null) return;

            if (_lessons[lesson] == null)
                _lessons.Add(lesson, new List<Class>());

            _lessons[lesson].Add(lessonClass);
        }
    }
    #endregion

    [Serializable()]
    internal class Class : IComparable<Class> 
    {
        public string Name { get; private set; }

        public List<Student> Students { get; private set; } = new List<Student>();
        private Dictionary<string, Teacher> _lessonsTeachers = new Dictionary<string, Teacher>();

        internal Class(string name)
        {
            Name = name;
        }

        public void AddStudent(Student student)
        {
            Students.Add(student);
            if (Students.Count > 1)
            {
                var lessons = Students[0].LessonsMark.ToDictionary(entry => entry.Key, entry => entry.Value);
                Students.Last();
            }

        }
        public void DeleteStudent(Student student) => Students.Remove(student);

        public void AddLesson(string nameLesson, Teacher teacher) =>
            _lessonsTeachers.Add(nameLesson, teacher);
        public void DeleteLession(string nameLesson) =>
            _lessonsTeachers.Remove(nameLesson);

        public float GetAverageRating(string lessonName, uint quarter)
        {
            float rating = 0;
            foreach(Student student in Students)
            {
                rating += student.GetMarks(lessonName)[(int)quarter];
            }
            return rating / Students.Count;
        }

        public int CompareTo(Class other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
