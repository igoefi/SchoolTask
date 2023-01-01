using School.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11.Classes
{
    [Serializable()]
    public class Arrays
    {
        public List<Class> _classes = new List<Class>();
        public List<Teacher> _teachers = new List<Teacher>();

        #region Add&DeleteSomeone
        public void AddDeleteClass(string name, bool isAdd, out string exemption)
        {
            exemption = null;
            if (FindClass(name) != null)
            {
                if (!isAdd)
                    _classes.Remove(FindClass(name));
                else
                    exemption = "Этот класс уже существует";

                return;
            }

            if (isAdd)
                _classes.Add(new Class(name));
            else
                exemption = "Этого класса не существует";
            _classes.Sort();
        }

        public void AddDeleteStudent(string className, string studentName, string studentSurname, bool isAdd, out string exemption)
        {
            exemption = null;
            Class schClass = FindClass(className);
            if (schClass == null)
            {
                exemption = "Класс не найден";
                return;
            }
            if (FindStudent(schClass.Name, studentName, studentSurname) != null && isAdd)
            {
                exemption = $"Студент {studentSurname} {studentName} уже находится в этом классе";
                return;
            }

            if (isAdd)
            {
                schClass.AddStudent(new Student(studentName, studentSurname));
            }
            else
            {
                schClass.DeleteStudent(FindStudent(schClass, studentName, studentSurname));
            }
            schClass.Students.Sort();
        }

        public void AddDeleteLesson(string className, string lesson, string nameTeacher, string surnameTeacher, bool isAdd, out string exemption)
        {
            Class schClass = FindClass(className);
            Teacher teacher = FindTeacher(nameTeacher, surnameTeacher);
            if (teacher == null)
            {
                exemption = $"Учителя {surnameTeacher} {nameTeacher} нет в базе данных";
                return;
            }
            if (schClass == null)
            {
                exemption = $"Класса под номером {className} не существует. Вы можете создать его";
                return;
            }

            schClass.AddDeleteLesson(lesson, teacher, isAdd, out exemption);
            if (exemption != null) return;
            if (isAdd)
                teacher.AddClassOrLesson(lesson, schClass);
            else
                teacher.DeleteLesson(lesson, schClass);
        }
        public void AddDeleteLesson(string lesson, string nameTeacher, string surnameTeacher, bool isAdd, out string exemption)
        {
            exemption = null;
            Teacher teacher = FindTeacher(nameTeacher, surnameTeacher);
            if (teacher == null)
            {
                exemption = $"Учителя {surnameTeacher} {nameTeacher} нет в базе данных";
                return;
            }
            foreach (Class schClass in _classes)
            {
                schClass.AddDeleteLesson(lesson, teacher, isAdd, out exemption);
                if(exemption != null) return;
            }
            if (isAdd)
            {
                foreach (Class schClass in _classes)
                    teacher.AddClassOrLesson(lesson, schClass);
            }
            else
            {
                teacher.DeleteLesson(lesson);
            }
        }

        public void AddDeleteTeacher(string name, string surname, int mainClassNum, bool isAdd, out string exemption)
        {
            exemption = null;
            Teacher teacher = FindTeacher(name, surname);
            if (teacher != null)
            {
                if (!isAdd)
                    _teachers.Remove(FindTeacher(name, surname));
                else
                    exemption = "Этот учитель уже записан в базе данных";
                return;
            }

            if (isAdd)
                _teachers.Add(new Teacher(name, surname, mainClassNum));
            else
                exemption = "Этого учителя нет";
        }
        #endregion
        #region GetSetSomeone
        #region SetMark
        public void SetMark(string className, string studentName, string studentSurname, string lessonName, float mark, uint quarter)
        {
            Class schClass = FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine($"Класса с номером {className} не существует");
                return;
            }

            Student student = FindStudent(schClass, studentName, studentSurname);
            if (student == null)
            {
                Console.WriteLine($"Студента {studentSurname} {studentName} не существует");
                return;
            }

            float realMark = mark > 2 ? mark < 5 ? mark : 5 : 2;
            student.LessonsMark.TryGetValue(lessonName, out List<float> lessonRate);
            if (lessonRate == null)
            {
                Console.WriteLine("Такого пердмета не существует в этом классе");
                return;
            }
            if (lessonRate.Count < quarter - 1)
            {
                Console.WriteLine("Сначала заполните предыдущие четверти");
                return;
            }
            if (quarter < 1)
                return;
            lessonRate[(int)quarter - 1] = realMark;
        }
        public void SetMark(string className, string studentName, string studentSurname, string lessonName, float mark)
        {
            Class schClass = FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine($"Класса с номером {className} не существует");
                return;
            }

            Student student = FindStudent(schClass, studentName, studentSurname);
            if (student == null)
            {
                Console.WriteLine($"Студента {studentSurname} {studentName} не существует");
                return;
            }

            float realMark = mark > 2 ? mark < 5 ? mark : 5 : 2;
            student.LessonsMark.TryGetValue(lessonName, out List<float> marks);
            if (marks == null)
            {
                Console.WriteLine($"Предмет {lessonName} в классе {className} не преподаётся");
            }
            else
            {
                marks.Add(realMark);
            }
        }
        #endregion
        #region GetRating
        public void GetClassRating(out List<Class> classes, out List<float> marks)
        {
            classes = new List<Class>();
            marks = new List<float>();

            if (_classes.Count == 0) return;

            foreach (Class schClass in _classes)
            {
                if (schClass.GetLessons().Count == 0) continue;
                if (schClass.Students.Count == 0) continue;

                classes.Add(schClass);
                var averangeMarks = schClass.GetAverageRatingOfClass();
                float sumMarks = 0;
                float lessonsCount = 0;
                foreach (string lesson in averangeMarks.Keys)
                {
                    List<float> lessonsMark = averangeMarks[lesson];
                    if (lessonsMark.Count == 0) continue;

                    lessonsCount++;
                    sumMarks += lessonsMark.Sum() / lessonsMark.Count();
                }
                marks.Add(lessonsCount > 0 ? sumMarks / lessonsCount : 0);
            }

            //Не бейте за пузырька
            Class mejClass;
            float mejMark;
            for (int i = 0; i < marks.Count; i++)
            {
                for (int j = i + 1; j < marks.Count; j++)
                {
                    if (marks[i] < marks[j])
                    {
                        mejClass = classes[i];
                        mejMark = marks[i];

                        marks[i] = marks[j];
                        marks[j] = mejMark;
                        classes[i] = classes[j];
                        classes[j] = mejClass;
                    }
                }
            }
        }
        public void GetTeacherRating(out List<Teacher> teachers, out List<float> marks)
        {
            teachers = new List<Teacher>();
            marks = new List<float>();

            if (_teachers.Count == 0) return;

            foreach (Teacher teacher in _teachers)
            {
                teachers.Add(teacher);

                var teacherMarks = teacher.GetAllMarks();

                if (teacherMarks == null)
                {
                    marks.Add(0);
                    return;
                }
                //Почему я просто не сделал "marks.Add(teacherMarks.Values.Sum() / teacherMarks.Count())" ? Потому что это не учитывает моменты,
                //когда оценки по предметам не расставлдены, и получается, что число в сумму не прибавляется(+0), но делится на большее кол-во уроков
                float sumMarks = 0;
                int sumLessons = 0;
                foreach (string lesson in teacherMarks.Keys)
                {
                    if (teacherMarks[lesson] == 0) continue;

                    sumMarks += teacherMarks[lesson];
                    sumLessons++;
                }

                marks.Add(sumLessons > 0 ? sumMarks / sumLessons : 0);
            }

            //Не бейте за пузырька во второй раз
            Teacher mejClass;
            float mejMark;
            for (int i = 0; i < marks.Count; i++)
            {
                for (int j = i + 1; j < marks.Count; j++)
                {
                    if (marks[i] < marks[j])
                    {
                        mejClass = teachers[i];
                        mejMark = marks[i];

                        marks[i] = marks[j];
                        marks[j] = mejMark;
                        teachers[i] = teachers[j];
                        teachers[j] = mejClass;
                    }
                }
            }

        }
        #endregion

        public List<float> GetMarks(string className, string studentName, string studentSurname, string lessonName, out string exemption)
        {
            exemption = null;
            Class schClass = FindClass(className);
            if (schClass == null)
            {
                exemption = $"Класса с номером {className} не существует";
                return null;
            }
            Student student = FindStudent(schClass, studentName, studentSurname);
            if (student == null)
            {
                exemption = $"Студента {studentSurname} {studentName} не существует";
                return null;
            }
            return student.LessonsMark[lessonName];
        }

        public List<string> GetLessons(string className)
        {
            List<string> lessons = FindClass(className).GetLessons();
            return lessons;
        }
        #endregion
        #region FindInBase
        public Class FindClass(string name)
        {
            foreach (Class schooleClass in _classes)
                if (schooleClass.Name.ToLower() == name.ToLower()) return schooleClass;
            return null;
        }

        public Teacher FindTeacher(string name, string surname)
        {
            if (_teachers.Count > 0)
                foreach (Teacher teacher in _teachers)
                    if (teacher.Name.ToLower() == name.ToLower() && teacher.Surname.ToLower() == surname.ToLower()) return teacher;
            return null;
        }

        public Student FindStudent(string schClass, string name, string surname)
        {
            Class schClassClass = FindClass(schClass);
            if (schClassClass == null)
                return null;

            foreach (Student student in schClassClass.Students)
                if (student.Name.ToLower() == name.ToLower() && student.Surname.ToLower() == surname.ToLower()) return student;

            return null;
        }
        public Student FindStudent(Class schClass, string name, string surname)
        {
            if (schClass == null)
                return null;

            foreach (Student student in schClass.Students)
                if (student.Name.ToLower() == name.ToLower() && student.Surname.ToLower() == surname.ToLower()) return student;

            return null;
        }

        public bool IsReallyLesson(string className, string lesson)
        {
            Class schClass = FindClass(className);
            if (schClass == null) return false;

            foreach(var classLesson in schClass.GetLessons())
                if(classLesson.ToLower() == lesson.ToLower())
                    return true;

            return false;
        }

        public bool IsReallyLesson(string teacherName, string TeacherSurname, string lesson)
        {
            Teacher teacher = FindTeacher(teacherName, TeacherSurname);
            if(teacher == null) return false;

            foreach (var teachLesson in teacher.Lessons.Keys)
                if (lesson.ToLower() == teachLesson.ToLower())
                    return true;
            return false;
        }
        #endregion
    }
}

