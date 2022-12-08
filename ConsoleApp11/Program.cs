using System;
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

        static void Main(string[] args)
        {
            _arrays = new Arrays();
            if (File.Exists(FileName))
            {
                _arrays = ReadFile(out Stream stream);
                stream.Close();
            }


            List<float> list = new List<float> {
                0,
                0,
                0
            };

          
            Console.WriteLine("Вывод всех учеников из всех классов");
            foreach(Class schClass in _arrays._classes)
            {
                Console.WriteLine($"Class {schClass.Name}");
                WriteStudentsOfClass(schClass.Name);
            }

            SaveFile();
        }

        #region AddDeleteAnything
        private static void AddDeleteClass(bool isAdd)
        {
            Console.WriteLine("Введите название класса");
            string className = Console.ReadLine();
            _arrays.AddDeleteClass(className, isAdd);
        }

        private static void AddDeleteStudent(bool isAdd)
        {
            Console.Clear();

            Console.WriteLine("Введите название класса, куда необходимо добавить ученика: ");
            string className = Console.ReadLine();
            if(_arrays.FindClass(className) == null)
            {
                Console.WriteLine("Такого класса не существует");
                return;
            }

            Console.WriteLine("Введите имя и фамилию ученика этого класса");
            string studentName = Console.ReadLine();
            string studentSurname = Console.ReadLine();

           _arrays.AddDeleteStudent(className, studentName, studentSurname, isAdd);
        }

        private static void AddDeleteTeacher(bool isAdd)
        {
            Console.Clear();
            Console.WriteLine("Добавление учителя");

            Console.WriteLine("Напишите имя и фамилию учителя");
            string name = Console.ReadLine();
            string surname = Console.ReadLine();

            Console.WriteLine("Напишите, к какому кабинету привязан учитель");
            uint.TryParse(Console.ReadLine(), out uint cabinetNum);

            _arrays.AddDeleteTeacher(name, surname, cabinetNum, isAdd);
        }
        #endregion

        private static void WriteStudentsOfClass(string className)
        {
            Class schClass = _arrays.FindClass(className);
            if (schClass != null)
                foreach (Student student in schClass.Students)
                    Console.WriteLine($"Ученик {student.Surname} {student.Name}");
        }

        #region Serialization
        private static Arrays ReadFile(out Stream stream)
        {
            Console.WriteLine("Reading saved file");
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
                    Console.WriteLine("This class is here");

                return;
            }

            _classes.Add(new Class(name));
            _classes.Sort();
        }

        public void AddDeleteStudent(string className, string stundentName, string studentSurname, bool isAdd)
        {
            Class schClass = FindClass(className);
            if (schClass == null)
            {
                Console.WriteLine("Класс не найден");
                return;
            }

            if (isAdd)
            {
                schClass.AddStudent(new Student(stundentName, studentSurname));
                schClass.Students.Sort();
            }
            else
            {
                schClass.DeleteStudent(FindStudent(schClass, stundentName, studentSurname));
            }
        }

        public void AddDeleteLesson(string className, string lesson, string nameTeacher, string surnameTeacher, bool isAdd)
        {
            Class schClass = FindClass(className);
            Teacher teacher = FindTeacher(nameTeacher, surnameTeacher);
            if (teacher == null)
            {
                Console.WriteLine($"Учитель {surnameTeacher} {nameTeacher} нет в базе данных");
                return;
            }
            if (schClass == null)
            {
                Console.WriteLine($"Класса под номером {className} не существует. Вы можете создать его");
                return;
            }

            if (isAdd)
                schClass.AddLesson(lesson, teacher);
            else
                schClass.DeleteLession(lesson);

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

            _teachers.Add(new Teacher(name, surname, mainClassNum));
        }
        #endregion

        #region FindInBase
        public Class FindClass(string name)
        {
            foreach (Class schooleClass in _classes)
                if (schooleClass.Name == name) return schooleClass;
            return null;
        }

        public Teacher FindTeacher(string name, string surname)
        {
            foreach (Teacher teacher in _teachers)
                if (teacher.Name == name && teacher.Surname == surname) return teacher;
            return null;
        }

        private Student FindStudent(Class schClass, string name, string surname)
        {
            foreach (Student student in schClass.Students)
                if (student.Name == name && student.Surname == surname) return student;
            return null;
        }
        #endregion
    }
}
