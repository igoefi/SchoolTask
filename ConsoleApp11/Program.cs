using ConsoleApp11.Classes;
using School.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

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
            Console.WriteLine("6 - Вывести информацию о конкретном учителе");
            Console.WriteLine("7 - Вывести рейтинг учителей");
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
                case '6':
                    FindAndWriteInfoAboutTeacher();
                    break;
                case '7':
                    WriteTeacherRating();
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
            Console.WriteLine("9 - Вывести рейтинг классов по средним оценкам");
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
                case '9':
                    WriteClassRating();
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
        #region WriteRating
        private static void WriteClassRating()
        {
            _arrays.GetClassRating(out List<Class> classes, out List<float> marks);
            for (int classNum = 0; classNum < classes.Count; classNum++)
            {
                Console.WriteLine($"{classNum + 1} класс {classes[classNum].Name}." + (marks[classNum] > 0 ?
                    $" Средняя оценка - {marks[classNum]}" : " Оценка не выставлена"));
            }
        }

        private static void WriteTeacherRating()
        {
            _arrays.GetTeacherRating(out List<Teacher> teachers, out List<float> marks);
            for (int teacherNum = 0; teacherNum < teachers.Count; teacherNum++)
            {
                Console.WriteLine($"{teacherNum + 1} класс {teachers[teacherNum].Name}." + (marks[teacherNum] > 0 ?
                    $" Средняя оценка - {marks[teacherNum]}" : " Оценки не выставлены"));
            }
        }
        #endregion

        #region WriteAboutPerson
        private static void FindAndWriteInfoAboutTeacher()
        {
            NameInput("учителя", out string name, out string surname);

            Teacher teacher = _arrays.FindTeacher(name, surname);
            if (teacher == null)
            {
                Console.WriteLine("Такого учителя не существует");
                return;
            }

            Console.Clear();
            Console.WriteLine($"Учитель {teacher.Surname} {teacher.Name}, " +
                $"привязан(а) к кабинету {teacher.MainClassNum}");

            var marks = teacher.GetAllMarks();
            if (marks == null)
            {
                Console.WriteLine("Этот учитель не ведёт уроки");
                return;
            }

            Console.WriteLine("Оценки по урокам:");
            foreach (string lesson in marks.Keys)
            {
                Console.WriteLine();
                marks.TryGetValue(lesson, out float mark);
                if (mark == 0)
                    Console.WriteLine($"Урок {lesson}. Оценка не выставлена");
                else
                    Console.WriteLine($"Урок {lesson}. Средняя оценка - {mark}");
            }
        }

        private static void FindAndWriteInfoAboutClass()
        {
            string className = ClassInput();
            Class schClass = _arrays.FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine("Данного класса не существует");
                return;
            }

            List<Student> students = schClass.Students;
            if (students.Count == 0)
            {
                Console.WriteLine("В этом классе нет учеников");
                return;
            }

            Console.Clear();

            Console.WriteLine($"Класс {className}.");
            Console.WriteLine();

            Console.WriteLine("Студенты:");
            for (int studentNum = 0; studentNum < schClass.Students.Count; studentNum++)
            {
                Console.WriteLine($"{studentNum + 1} студент: {students[studentNum].Surname}" +
                    $" {students[studentNum].Name}");
            }

            Console.WriteLine();
            Console.WriteLine("Уроки:");
            var marks = schClass.GetAverageRatingOfClass();

            foreach (var lessons in marks.Keys)
            {
                Console.WriteLine();
                Console.WriteLine($"Урок - {lessons}");
                var mark = marks[lessons];
                if (mark.Count == 0)
                {
                    Console.WriteLine($"Оценки не проставлены");
                    return;
                }
                for (int quarter = 0; quarter < mark.Count; quarter++)
                    Console.WriteLine($"{quarter + 1}-я четверть, средняя оценка: {mark[quarter]}");
                Console.WriteLine($"Средняя оценка - {mark.Sum() / mark.Count()}");
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
            foreach (string lesson in student.LessonsMark.Keys)
            {
                Console.WriteLine($"Урок {lesson}:");
                List<float> marks = student.LessonsMark[lesson];
                if (marks.Count == 0)
                {
                    Console.WriteLine("По этому предмету не выставлены оценки");
                    continue;
                }

                for (int i = 0; i < marks.Count; i++)
                {
                    Console.WriteLine($"{i + 1} четверть - {marks[i]}");
                }
                Console.WriteLine($"Средняя оценка : {Math.Round(marks.Sum() / marks.Count, 2)}");
            }
        }
        #endregion
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

}