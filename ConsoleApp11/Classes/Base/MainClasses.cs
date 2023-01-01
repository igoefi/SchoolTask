using System;
using System.Collections.Generic;
using System.Linq;

namespace School.Classes
{
    #region People
    [Serializable()]
    public abstract class Human
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
    public class Student : Human, IComparable<Student>
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
    public class Teacher : Human
    {
        public Dictionary<string, List<Class>> Lessons { get; } = new Dictionary<string, List<Class>>();
        public int MainClassNum { get; private set; }
        internal Teacher(string name, string surname, int mainClassNum) : base(name, surname)
            => MainClassNum = mainClassNum;

        public Dictionary<string, float> GetAllMarks()
        {
            if(Lessons.Count == 0) return null;

            var marks = new Dictionary<string, float>();
            foreach (string lesson in Lessons.Keys)
            {
                float rate = 0;
                float classCount = 0;

                for (int classNum = 0; classNum < Lessons.Count; classNum++)
                {
                    var classMarks = Lessons[lesson][classNum].GetAverageRatingOfClass(lesson);
                    if (classMarks == null) continue;

                    rate += classMarks.Sum() / classMarks.Count();
                    classCount++;
                }
                float finalMark = rate / classCount;
                if (classCount > 0)
                    marks.Add(lesson, finalMark);
                else
                    marks.Add(lesson, 0);
            }
            return marks;
        }


        public void AddClassOrLesson(string lesson, Class lessonClass)
        {
            Lessons.TryGetValue(lesson, out List<Class> list);
            if (list == null)
                Lessons.Add(lesson, new List<Class>());

            if (lessonClass != null)
                Lessons[lesson].Add(lessonClass);
        }
        public void DeleteLesson(string lesson, Class schClass)
        {
            Lessons.TryGetValue(lesson, out List<Class> list);
            list?.Remove(schClass);
        }
        public void DeleteLesson(string lesson)
        {
            Lessons.TryGetValue(lesson, out List<Class> list);
            if (list != null)
                Lessons.Remove(lesson);
        }
    }
    #endregion

    [Serializable()]
    public class Class : IComparable<Class>
    {
        public string Name { get; private set; }

        public List<Student> Students { get; private set; } = new List<Student>();
        private readonly Dictionary<string, Teacher> _lessonsTeachers = new Dictionary<string, Teacher>();

        internal Class(string name) =>
            Name = name;

        public void AddStudent(Student student)
        {
            Students.Add(student);
            if (Students.Count > 1)
            {
                var lessons = Students[0].LessonsMark.ToDictionary(entry => entry.Key, entry => entry.Value);

                foreach (List<float> marks in lessons.Values)
                    for (int i = 0; i < marks.Count; i++)
                        marks[i] = 0;

                student.SetMarks(lessons);
            }
            else
            {
                var lessons = new Dictionary<string, List<float>>();
                foreach (string lessonName in _lessonsTeachers.Keys)
                {
                    lessons.Add(lessonName, new List<float>());
                }
                student.SetMarks(lessons);
            }
        }
        public void DeleteStudent(Student student) => Students.Remove(student);

        public void AddDeleteLesson(string nameLesson, Teacher teacher, bool isAdd, out string exemption)
        {
            exemption = null;
            _lessonsTeachers.TryGetValue(nameLesson, out Teacher checkTeacher);
            if (checkTeacher == null)
            {
                if (!isAdd)
                {
                    exemption = $"Урок {nameLesson} не ведётся в классе {Name}";
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
                    exemption = $"Учитель по предмету {nameLesson} сменён с учителя {checkTeacher.Surname} " +
                        $"{checkTeacher.Name} на учителя {teacher.Surname} {teacher.Name}";

                    _lessonsTeachers[nameLesson] = teacher;
                    return;
                }
                if (checkTeacher != teacher)
                {
                    exemption = $"Урок {nameLesson} в классе {Name} ведёт {checkTeacher.Surname} " +
                        $"{checkTeacher.Name}, а не {teacher.Surname} {teacher.Name}";
                    return;
                }

                _lessonsTeachers.Remove(nameLesson);
                foreach (Student student in Students)
                {
                    student.DeleteLesson(nameLesson);
                }
            }
        }

        public Dictionary<string, List<float>> GetAverageRatingOfClass()
        {
            if (Students.Count == 0) return null;
            if (_lessonsTeachers.Count == 0) return null;

            var marks = new Dictionary<string, List<float>>();

            foreach (string lessons in _lessonsTeachers.Keys)
            {
                var marksList = new List<float>();

                for (int quarter = 0;
                    quarter < Students.First().LessonsMark[lessons].Count(); quarter++)
                {
                    float rating = 0;
                    int studentCount = 0;
                    foreach (Student student in Students)
                    {
                        float mark = student.GetAverangeMarkOfLession(lessons);
                        if (mark == 0) continue;

                        studentCount++;
                        rating += student.GetMarks(lessons)[quarter];
                    }

                    marksList.Add(rating / studentCount);
                }

                marks.Add(lessons, marksList);
            }

            return marks;
        }

        public List<float> GetAverageRatingOfClass(string lesson)
        {
            if (Students.Count == 0) return null;
            if (_lessonsTeachers.Count == 0) return null;
            _lessonsTeachers.TryGetValue(lesson, out Teacher teacher);
            if (teacher == null) return null;

            var marksList = new List<float>();

            for (int quarter = 0;
                quarter < Students.First().LessonsMark[lesson].Count(); quarter++)
            {
                float rating = 0;
                int studentCount = 0;
                foreach (Student student in Students)
                {
                    float mark = student.GetAverangeMarkOfLession(lesson);
                    if (mark == 0) continue;

                    studentCount++;
                    rating += student.GetMarks(lesson)[quarter];
                }

                marksList.Add(rating / studentCount);
            }
            return marksList;
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
