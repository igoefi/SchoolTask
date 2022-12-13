﻿using System;
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
            LessonsMark.TryGetValue(lessonName, out List<float> marks);
            if (marks == null)
                LessonsMark.Add(lessonName, new List<float>());
        }

        public void DeleteLesson(string name) => LessonsMark.Remove(name);

        #region GetSet
        public List<float> GetMarksOfLession(string lessonName)
        {
            if (LessonsMark[lessonName] != null)
                return LessonsMark[lessonName];
            return null;

        }

        public float GetAverangeMarkOfLession(string lessonName)
        {
            List<float> marks = LessonsMark[lessonName];
            if (marks != null)
                return marks.Sum() / marks.Count;

            return 0;
        }

        public List<float> GetMarks(string lesson)
        {
            if (LessonsMark[lesson] == null)
                return null;
            return LessonsMark[lesson];
        }

        public void SetMarks(Dictionary<string, List<float>> lessons) =>
            LessonsMark = lessons;
        #endregion

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
            _lessons.TryGetValue(lesson, out List<Class> list);
            if (list == null)
                _lessons.Add(lesson, new List<Class>());

            if (lessonClass != null)
                _lessons[lesson].Add(lessonClass);
        }
        public void DeleteLesson(string lesson, Class schClass)
        {
            _lessons.TryGetValue(lesson, out List<Class> list);
            if(list != null)
                list.Remove(schClass);
        }
        public void DeleteLesson(string lesson)
        {
            _lessons.TryGetValue(lesson, out List<Class> list);
            if (list != null)
                _lessons.Remove(lesson);
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

                foreach (List<float> marks in lessons.Values)
                    for(int i = 0; i < marks.Count; i++)
                        marks[i] = 0;

                student.SetMarks(lessons);
            }
            else
            {
                var lessons = new Dictionary<string, List<float>>();
                foreach(string lessonName in _lessonsTeachers.Keys)
                {
                    lessons.Add(lessonName, new List<float>());
                }
                student.SetMarks(lessons);
            }
        }
        public void DeleteStudent(Student student) => Students.Remove(student);

        public void AddDeleteLesson(string nameLesson, Teacher teacher, bool isAdd)
        {
            _lessonsTeachers.TryGetValue(nameLesson, out Teacher checkTeacher);
            if (checkTeacher == null)
            {
                if (!isAdd)
                {
                    Console.WriteLine($"Урок {nameLesson} не ведётся в классе {Name}");
                    return;
                }

                _lessonsTeachers.Add(nameLesson, teacher);
                foreach (Student student in Students)
                {
                    student.AddLesson(nameLesson);
                }
            }
            else
            {
                if (isAdd)
                {
                    Console.WriteLine($"Учитель по предмету {nameLesson} сменён с учителя {checkTeacher.Surname} {checkTeacher.Name} на учителя {teacher.Surname} {teacher.Name}");
                    _lessonsTeachers[nameLesson] = teacher;
                    return;
                }
                if(checkTeacher != teacher)
                {
                    Console.WriteLine($"Урок {nameLesson} в классе {Name} ведёт {checkTeacher.Surname} {checkTeacher.Name}, а не {teacher.Surname} {teacher.Name}");
                    Console.WriteLine("По этой причине операция отменена");
                    return;
                }

                _lessonsTeachers.Remove(nameLesson);
                foreach (Student student in Students)
                {
                    student.DeleteLesson(nameLesson);
                }
            }
        }


        public float GetAverageRatingOsClass(string lessonName, uint quarter)
        {
            float rating = 0;
            int studentCount = 0;
            foreach (Student student in Students)
            {
                float mark = student.GetMarks(lessonName)[(int)quarter];
                if (mark != 0)
                {
                    studentCount++;
                    rating += student.GetMarks(lessonName)[(int)quarter];
                }
            }
            return rating / studentCount;
        }
        public float GetAverageRatingOsClass(string lessonName)
        {
            float rating = 0;
            int studentCount = 0;
            foreach (Student student in Students)
            {
                float mark = student.GetAverangeMarkOfLession(lessonName);
                if (mark != 0)
                {
                    studentCount++;
                    rating += mark;
                }
            }
            return rating / studentCount;
        }
        public List<string> GetLessons()
        {
            return _lessonsTeachers.Keys.ToList();
        }

        public int CompareTo(Class other)
        {
            return Name.CompareTo(other.Name);
        }
    }
}
