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
        private Dictionary<string, List<float>> _lessons = new Dictionary<string, List<float>>();
        internal Student(string name, string surname) : base(name, surname) { }

        public void AddOrRateLesson(string lessonName, float mark)
        {
            if (_lessons[lessonName] == null)
                _lessons.Add(lessonName, new List<float>());

            float realMark = mark > 2 ? mark < 5 ? mark : 5 : 2;
            _lessons[lessonName].Add(realMark);
        }

        public void DeleteLesson(string name) => _lessons.Remove(name);
        public List<float> GetMarks(string lesson)
        {
            return _lessons[lesson];
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

        public void AddStudent(Student student) => Students.Add(student);
        public void DeleteStudent(Student student) => Students.Remove(student);

        public void AddLesson(string nameLesson, Teacher teacher) =>
            _lessonsTeachers.Add(nameLesson, teacher);
        public void DeleteLession(string nameLesson) =>
            _lessonsTeachers.Remove(nameLesson);

        public float GetAverageRating(string lessonName)
        {
            float rating = 0;
            foreach(Student student in Students)
            {
                rating += student.GetMarks(lessonName).Last();
            }
            return rating / Students.Count;
        }

        public int CompareTo(Class other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
