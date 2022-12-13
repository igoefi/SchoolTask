﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using School.Classes;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

namespace School
{
    [Serializable()]
    internal class Program
    {
        const string FileName = @"../../../SavedClass.json";
        private static Arrays _arrays;

        static void Main()
        {
            _arrays = new Arrays();
            if (File.Exists(FileName))
            {
                _arrays = ReadFile(out Stream stream);
                stream.Close();
            }

            while (true)
            {
                Console.Clear();

                Console.WriteLine("Работа с базой данных учеников:");
                Console.WriteLine("1 - учителя");
                Console.WriteLine("2 - классы");
                Console.WriteLine("3 - ученики");

                ConsoleKeyInfo key = Console.ReadKey();
                Console.Clear();
                switch (key.KeyChar)
                {
                    case '1':
                        TeacherManipulation();
                        break;
                    case '2':
                        ClassManipulation();
                        break;
                    case '3':
                        StudentManipulation();
                        break;
                    default:
                        return;
                }
                Console.WriteLine("Нажмите на любую клавишу для продолжения");
                Console.ReadKey();
            }
            SaveFile();
        }

        #region Manipulation
        private static void TeacherManipulation()
        {
            Console.Clear();
            Console.WriteLine("1 - Добавить учителя");
            Console.WriteLine("2 - Удалить учителя");
            Console.WriteLine("3 - Добавить предмет, который ведёт учитель");
            Console.WriteLine("4 - Удалить предмет, который ведёт учитель");
            Console.WriteLine("5 - Вывеси список всех учителей");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.Clear();
            switch (key.KeyChar)
            {
                case '1':
                    AddDeleteTeacher(true);
                    break;
                case '2':
                    AddDeleteTeacher(false);
                    break;
                case '3':
                    AddDeleteLesson(true);
                    break;
                case '4':
                    AddDeleteLesson(false);
                    break;
                case '5':
                    WriteAllTeachers();
                    break;
                default:
                    Console.Clear();
                    return;
            }
        }

        private static void ClassManipulation()
        {
            Console.Clear();
            Console.WriteLine("1 - Добавить класс");
            Console.WriteLine("2 - Удалить класс");
            Console.WriteLine("3 - Добавить предмет у класса(ов)");
            Console.WriteLine("4 - Удалить предмет у класса(ов)");
            Console.WriteLine("5 - Вывести список всех классов");
            Console.WriteLine("6 - Вывести список всех учеников класса");
            Console.WriteLine("7 - Вывести список всех уроков класса");
            Console.WriteLine("8 - Вывести информацию о классе");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.Clear();
            switch (key.KeyChar)
            {
                case '1':
                    AddDeleteClass(true);
                    break;
                case '2':
                    AddDeleteClass(false);
                    break;
                case '3':
                    AddDeleteLesson(true);
                    break;
                case '4':
                    AddDeleteLesson(false);
                    break;
                case '5':
                    WriteAllClasses();
                    break;
                case '6':
                    WriteStudentsOfClass();
                    break;
                case '7':
                    WriteLessons();
                    break;          
                case '8':
                    FindAndWriteInfoAboutClass();
                    break;
                default:
                    Console.Clear();
                    return;
            }
        }

        private static void StudentManipulation()
        {
            Console.WriteLine("1 - Добавить ученика");
            Console.WriteLine("2 - Удалить ученика");
            Console.WriteLine("3 - Добавить оценки за четверть");
            Console.WriteLine("4 - Изменить оценки за четверть");
            Console.WriteLine("5 - Узнать оценки по предмету");
            Console.WriteLine("6 - Вывести информацию о ученике");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.Clear();
            switch (key.KeyChar)
            {
                case '1':
                    AddDeleteStudent(true);
                    break;
                case '2':
                    AddDeleteStudent(false);
                    break;
                case '3':
                    SetStudentMark(true);
                    break;
                case '4':
                    SetStudentMark(false);
                    break;
                case '5':
                    FindAndWriteInfoAboutStudentMarks();
                    break;         
                case '6':
                    FindAndWriteInfoAboutStudent();
                    break;
                default:
                    Console.Clear();
                    return;
            }
        }
        #endregion
        #region AddDeleteAnything
        private static void AddDeleteClass(bool isAdd)
        {
            string className = ClassInput();
            _arrays.AddDeleteClass(className, isAdd);
            SaveFile();
        }

        private static void AddDeleteLesson(bool isAdd)
        {
            string needWord = isAdd ? "добавить" : "удалить";
            Console.WriteLine($"Введите название класса, в котором необходимо {needWord} предмет. Если хотите {needWord} во всех, то напишите 'Все'");
            string className = ClassInput();

            if (_arrays.FindClass(className) == null && className.ToLower() != "все")
            {
                Console.WriteLine("Такого класса не существует");
                return;
            }

            Console.Write("Название урока - ");
            string lesson = Console.ReadLine();

            NameInput("учителя", out string teacherName, out string teacherSurname);
            if (className.ToLower() != "все")
                _arrays.AddDeleteLesson(className, lesson, teacherName, teacherSurname, isAdd);
            else
                _arrays.AddDeleteLesson(lesson, teacherName, teacherSurname, isAdd);
            SaveFile();
        }

        private static void AddDeleteStudent(bool isAdd)
        {
            string className = ClassInput();
            if (_arrays.FindClass(className) == null)
            {
                Console.WriteLine("Такого класса не существует");
                return;
            }

            NameInput("ученика", out string studentName, out string studentSurname);

            _arrays.AddDeleteStudent(className, studentName, studentSurname, isAdd);
            SaveFile();
        }

        private static void AddDeleteTeacher(bool isAdd)
        {
            Console.WriteLine("Добавление учителя");

            NameInput("учителя", out string name, out string surname);

            Console.WriteLine("Напишите, к какому кабинету привязан учитель");
            uint.TryParse(Console.ReadLine(), out uint cabinetNum);

            _arrays.AddDeleteTeacher(name, surname, cabinetNum, isAdd);
            SaveFile();
        }
        #endregion
        #region SetSomeone
        public static void SetStudentMark(bool isNewQuarter)
        {
            Console.Clear();
            string className = ClassInput();

            NameInput("ученика", out string studentName, out string studentSurname);

            Console.WriteLine("Введите название урока, по которому ставится оценка");
            string lessonName = Console.ReadLine();

            Console.WriteLine("Введите оценку");
            float.TryParse(Console.ReadLine(), out float mark);

            if (!isNewQuarter)
            {
                Console.WriteLine("Введите номер четверти, за которую ставить оценку");
                uint.TryParse(Console.ReadLine(), out uint quarter);
                _arrays.SetMark(className, studentName, studentSurname, lessonName, mark, quarter);
            }
            else
            {
                _arrays.SetMark(className, studentName, studentSurname, lessonName, mark);
            }
            SaveFile();
        }
        #endregion
        #region WriteInformation
        private static void FindAndWriteInfoAboutClass()
        {
            string className = ClassInput();
            Class schClass = _arrays.FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine("Данного класса не существует");
                 return;
            }

            Console.Clear();
            Console.WriteLine($"Класс {className}.");
            Console.WriteLine("Студенты::");
            for (int studentNum = 0; studentNum < schClass.Students.Count; studentNum++)
            {
                Console.WriteLine($"");
            }
        }

        private static void FindAndWriteInfoAboutStudent()
        {
            string className = ClassInput();
            Class schClass = _arrays.FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine("Этого класса не существует");
                return;
            }
            NameInput("ученика", out string name, out string surname);
            Student student = _arrays.FindStudent(schClass, name, surname);
            if (student == null)
            {
                Console.WriteLine("Указанного ученика нет в этом классе");
                return;
            }

            Console.Clear();
            Console.WriteLine($"{surname} {name}, ученик {className}");
            Console.WriteLine("Оценки: ");
            foreach(string lesson in student.LessonsMark.Keys)
            {
                Console.WriteLine($"Урок {lesson}:");
                List<float> marks = student.LessonsMark[lesson];
                if(marks.Count == 0)
                {
                    Console.WriteLine("По этому предмету не выставлены оценки");
                    continue;
                }

                for(int i = 0; i < marks.Count; i++)
                {
                    Console.WriteLine($"{i+1} четверть - {marks[i]}");
                }
                Console.WriteLine($"Средняя оценка : {Math.Round(marks.Sum() / marks.Count, 2)}");
            } 
        }

        private static void FindAndWriteInfoAboutStudentMarks()
        {
            string className = ClassInput();

            NameInput("ученика", out string studentName, out string studentSurname);

            Console.WriteLine("Введите название урока, по которому необходимо узнать оценки");
            string lessonName = Console.ReadLine();


            List<float> marks = _arrays.GetMarks(className, studentName, studentSurname, lessonName);

            if (marks == null)
                return;

            Console.Clear();
            Console.WriteLine($"{studentSurname} {studentName}, ученик класса {className}");
            Console.WriteLine($"Предмет - {lessonName}");

            if (marks.Count > 0)
            {
                for (int i = 0; i < marks.Count; i++)
                    Console.WriteLine($"{i + 1} четверть: {marks[i]}");

                Console.WriteLine($"Средняя оценка : {Math.Round(marks.Sum() / marks.Count, 2)}");
            }
            else
            {
                Console.WriteLine("За этот предмет ещё не выставлены оценки");
            }
        }

        private static void WriteAllClasses()
        {
            foreach (Class schClass in _arrays._classes)
            {
                Console.WriteLine(schClass.Name);
            }
        }
        private static void WriteStudentsOfClass()
        {
            string className = ClassInput();
            Class schClass = _arrays.FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine("Такого класса не существует");
                return;
            }
            foreach (Student student in schClass.Students)
                Console.WriteLine($"Ученик {student.Surname} {student.Name}");
        }

        private static void WriteAllTeachers()
        {
            foreach (Teacher teacher in _arrays._teachers)
                Console.WriteLine($"Учитель {teacher.Surname} {teacher.Name}");
        }

        private static void WriteLessons()
        {
            string className = ClassInput();
            List<string> lessons = _arrays.GetLessons(className);
            foreach (string lesson in lessons)
            {
                Console.WriteLine($"Урок {lesson}");
            }
        }
        #endregion
        #region Serialization
        private static Arrays ReadFile(out Stream stream)
        {
            stream = File.OpenRead(FileName);
            BinaryFormatter deserializer = new BinaryFormatter();
            return (Arrays)deserializer.Deserialize(stream);
        }

        private static void SaveFile()
        {
            Stream SaveFileStream = File.Create(FileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, _arrays);
            SaveFileStream.Close();
        }
        #endregion
        #region Help
        private static void NameInput(string nameProfession, out string name, out string surname)
        {
            Console.WriteLine($"Напишите имя и фамилию {nameProfession}");
            Console.Write("Имя - ");
            name = Console.ReadLine();
            Console.Write("Фамилия - ");
            surname = Console.ReadLine();
        }
        private static string ClassInput()
        {
            Console.Write("Название класса - ");
            return Console.ReadLine();
        }
        #endregion
    }

    [Serializable()]
    internal class Arrays
    {
        public List<Class> _classes = new List<Class>();
        public List<Teacher> _teachers = new List<Teacher>();

        #region Add&DeleteSomeone
        public void AddDeleteClass(string name, bool isAdd)
        {
            if (FindClass(name) != null)
            {
                if (!isAdd)
                    _classes.Remove(FindClass(name));
                else
                    Console.WriteLine("Этот класс уже существует");

                return;
            }

            if (isAdd)
                _classes.Add(new Class(name));
            else
                Console.WriteLine("Этого класса не существует");
            _classes.Sort();
        }

        public void AddDeleteStudent(string className, string studentName, string studentSurname, bool isAdd)
        {
            Class schClass = FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine("Класс не найден");
                return;
            }
            if (FindStudent(schClass, studentName, studentSurname) != null && isAdd)
            {
                Console.WriteLine($"Студент {studentSurname} {studentName} уже находится в этом классе");
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

        public void AddDeleteLesson(string className, string lesson, string nameTeacher, string surnameTeacher, bool isAdd)
        {
            Class schClass = FindClass(className);
            Teacher teacher = FindTeacher(nameTeacher, surnameTeacher);
            if (teacher == null)
            {
                Console.WriteLine($"Учителя {surnameTeacher} {nameTeacher} нет в базе данных");
                return;
            }
            if (schClass == null)
            {
                Console.WriteLine($"Класса под номером {className} не существует. Вы можете создать его");
                return;
            }

            schClass.AddDeleteLesson(lesson, teacher, isAdd);
            if (isAdd)
                teacher.AddClassOrLesson(lesson, schClass);
            else
                teacher.DeleteLesson(lesson, schClass);
        }
        public void AddDeleteLesson(string lesson, string nameTeacher, string surnameTeacher, bool isAdd)
        {
            Teacher teacher = FindTeacher(nameTeacher, surnameTeacher);
            if (teacher == null)
            {
                Console.WriteLine($"Учителя {surnameTeacher} {nameTeacher} нет в базе данных");
                return;
            }
            foreach (Class schClass in _classes)
                schClass.AddDeleteLesson(lesson, teacher, isAdd);

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

        public void AddDeleteTeacher(string name, string surname, uint mainClassNum, bool isAdd)
        {
            Teacher teacher = FindTeacher(name, surname);
            if (teacher != null)
            {
                if (!isAdd)
                    _teachers.Remove(FindTeacher(name, surname));
                else
                    Console.WriteLine("Этот учитель уже записан в базе данных");

                return;
            }

            if (isAdd)
                _teachers.Add(new Teacher(name, surname, mainClassNum));
            else
                Console.WriteLine("Этого учителя нет");
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
            if(lessonRate == null)
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
        public List<float> GetMarks(string className, string studentName, string studentSurname, string lessonName)
        {
            Class schClass = FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine($"Класса с номером {className} не существует");
                return null;
            }
            Student student = FindStudent(schClass, studentName, studentSurname);
            if (student == null)
            {
                Console.WriteLine($"Студента {studentSurname} {studentName} не существует");
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

        public Student FindStudent(Class schClass, string name, string surname)
        {
            foreach (Student student in schClass.Students)
                if (student.Name.ToLower() == name.ToLower() && student.Surname.ToLower() == surname.ToLower()) return student;
            return null;
        }
        #endregion
    }
}
