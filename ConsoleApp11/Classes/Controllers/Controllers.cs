using School.Classes;
using School;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp11.Classes.Helper;

namespace ConsoleApp11.Classes.Controllers
{
    public class SerializationController
    {
        public Arrays ReadFile(out Stream stream, string fileName)
        {
            stream = File.OpenRead(fileName);
            BinaryFormatter deserializer = new BinaryFormatter();
            return (Arrays)deserializer.Deserialize(stream);
        }

        public void SaveFile(Arrays arrays, string fileName)
        {
            Stream SaveFileStream = File.Create(fileName);
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(SaveFileStream, arrays);
            SaveFileStream.Close();
        }
    }

    public class SetController : InputHelper
    {
        public void SetStudentMark(bool isNewQuarter)
        {
            Console.Clear();
            string className = ClassInput();
            if (className == null) return;

            NameInput(className, out string studentName, out string studentSurname);

            string lessonName = LessonInput();

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
        }

        public void SetClassMarks()
        {
            Console.Clear();
            string className = ClassInput();
            if (className == null)
                return;

            var schClass = _arrays.FindClass(className);

            Console.WriteLine("Введите название предмета, по которому ставится оценка");
            string lessonName = Console.ReadLine();

            if (!_arrays.IsReallyLesson(className, lessonName))
            {
                Console.WriteLine($"Урока {lessonName} в классе {className} нет");
                return;
            }

            Console.WriteLine("Какую четверть необходимо изменить? Если добавить новую, то напишите 0");
            Console.Write("Четверть - ");
            int.TryParse(Console.ReadLine(), out int quarter);

            foreach (Student student in schClass.Students)
            {
                Console.Clear();

                Console.WriteLine($"Класс {className}, урок {lessonName}" +
                    (quarter == 0 ? "." : $", {quarter} четверть"));

                Console.WriteLine($"Ученик {student.Surname} {student.Name}");
                Console.Write("Оценка - ");
                float.TryParse(Console.ReadLine(), out float mark);

                if (quarter == 0)
                    _arrays.SetMark(className, student.Name, student.Surname, lessonName, mark);
                else
                    _arrays.SetMark(className, student.Name, student.Surname, lessonName, mark, (uint)quarter);
            }
        }
    }

    public class AddDeleteController : InputHelper
    {
        public void AddDeleteClass(bool isAdd)
        {
            string className = ClassInput();
            if (className == null) return;

            string exemption = null;
            if (className != null)
                _arrays.AddDeleteClass(className, isAdd, out exemption);

            if (exemption != null)
                Console.WriteLine(exemption);
        }

        public void AddDeleteLesson(bool isAdd)
        {
            string needWord = isAdd ? "добавить" : "удалить";
            Console.WriteLine($"Введите название класса, в котором необходимо {needWord} предмет. Если хотите {needWord} во всех, то напишите 'Все'");

            string className = ClassInput();
            if (className == null)
                return;

            if (!NameInput(out string teacherName, out string teacherSurname))
                return;

            string lesson = LessonInput();
            if (!_arrays.IsReallyLesson(teacherName, teacherSurname, lesson)) return;


            string exemption;
            if (className.ToLower() != "все")
                _arrays.AddDeleteLesson(className, lesson, teacherName, teacherSurname, isAdd, out exemption);
            else
                _arrays.AddDeleteLesson(lesson, teacherName, teacherSurname, isAdd, out exemption);

            if (exemption != null)
                Console.WriteLine(exemption);
        }

        public void AddDeleteStudent(bool isAdd)
        {
            string className = ClassInput();
            if (className == null) return;

            NameInput(className, out string studentName, out string studentSurname);
            if (studentName == null) return;

            _arrays.AddDeleteStudent(className, studentName, studentSurname, isAdd, out string exemprion);

            if (exemprion != null) Console.WriteLine(exemprion);
        }

        public void AddDeleteTeacher(bool isAdd)
        {
            if (NameInput(out string name, out string surname))
            {
                Console.WriteLine("Этот учитель уже существует");
                return;
            }


            Console.WriteLine("Напишите, к какому кабинету привязан учитель");
            int.TryParse(Console.ReadLine(), out int cabinetNum);

            _arrays.AddDeleteTeacher(name, surname, cabinetNum, isAdd, out string exemption);

            if (exemption != null) Console.WriteLine(exemption);
        }
    }

    public class WriteInformationController : InputHelper
    {

        #region WriteRating
        public void WriteClassRating()
        {
            _arrays.GetClassRating(out List<Class> classes, out List<float> marks);
            for (int classNum = 0; classNum < classes.Count; classNum++)
            {
                Console.WriteLine($"{classNum + 1} класс {classes[classNum].Name}." + (marks[classNum] > 0 ?
                    $" Средняя оценка - {marks[classNum]}" : " Оценка не выставлена"));
            }
        }

        public void WriteTeacherRating()
        {
            _arrays.GetTeacherRating(out List<Teacher> teachers, out List<float> marks);
            for (int teacherNum = 0; teacherNum < teachers.Count; teacherNum++)
            {
                Console.WriteLine($"{teacherNum + 1} учитель, {teachers[teacherNum].Surname} {teachers[teacherNum].Name}." + (marks[teacherNum] > 0 ?
                    $" Средняя оценка - {marks[teacherNum]}" : " Оценки не выставлены"));
            }
        }
        #endregion

        #region WriteAboutPerson
        public void FindAndWriteInfoAboutTeacher()
        {
            NameInput(out string name, out string surname);

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

        public void FindAndWriteInfoAboutClass()
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

        public void FindAndWriteInfoAboutStudent()
        {
            string className = ClassInput();
            Class schClass = _arrays.FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine("Этого класса не существует");
                return;
            }
            NameInput(className, out string name, out string surname);


            if (name == null)
                return;

            Student student = _arrays.FindStudent(schClass, name, surname);

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

        public void FindAndWriteInfoAboutStudentMarks()
        {
            string className = ClassInput();
            if (className == null) return;


            NameInput(className, out string studentName, out string studentSurname);
            if (studentName == null) return;

            string lessonName = LessonInput();

            List<float> marks = _arrays.GetMarks(className, studentName, studentSurname, lessonName, out string exemption);

            if (exemption != null)
            {
                Console.WriteLine(exemption);
                return;
            }

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

        public void WriteAllClasses()
        {
            foreach (Class schClass in _arrays._classes)
            {
                Console.WriteLine(schClass.Name);
            }
        }

        public void WriteStudentsOfClass()
        {
            string className = ClassInput();
            if (className == null)
                return;
            var schClass = _arrays.FindClass(className);

            foreach (Student student in schClass.Students)
                Console.WriteLine($"Ученик {student.Surname} {student.Name}");
        }

        public void WriteAllTeachers()
        {
            foreach (Teacher teacher in _arrays._teachers)
                Console.WriteLine($"Учитель {teacher.Surname} {teacher.Name}");
        }

        public void WriteLessons()
        {
            string className = ClassInput();
            List<string> lessons = _arrays.GetLessons(className);

            foreach (string lesson in lessons)
            {
                Console.WriteLine($"Урок {lesson}");
            }
        }
    }
}
