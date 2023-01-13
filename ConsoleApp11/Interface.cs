using ConsoleApp11.Classes;
using ConsoleApp11.Classes.Abstraction;
using ConsoleApp11.Classes.Controllers;
using ConsoleApp11.Classes.Helper;
using School.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace School
{
    [Serializable()]
    internal class Interface
    {
        const string FileName = @"../../../SavedClass.json";
        public static Arrays Arrays { get; private set; }

        static ControllerAbstraction controllers;

        static void Main()
        {
            Arrays = new Arrays();
            if (File.Exists(FileName))
                Arrays = controllers.ReadFile(FileName);

            controllers = new ControllerAbstraction(new SerializationController(), new SetController(),
                new AddDeleteController(), new WriteInformationController());

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
            controllers.SaveFile(Arrays, FileName);
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
                    controllers.AddDeleteTeacher(true);
                    break;
                case '2':
                    controllers.AddDeleteTeacher(false);
                    break;
                case '3':
                    controllers.AddDeleteLesson(true);
                    break;
                case '4':
                    controllers.AddDeleteLesson(false);
                    break;
                case '5':
                    controllers.WriteAllTeachers();
                    break;
                case '6':
                    controllers.FindAndWriteInfoAboutTeacher();
                    break;
                case '7':
                    controllers.WriteTeacherRating();
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
            Console.WriteLine("3 - Выставить оценки по предмету в классе");
            Console.WriteLine("4 - Вывести список всех классов");
            Console.WriteLine("5 - Вывести список всех учеников класса");
            Console.WriteLine("6 - Вывести список всех уроков класса");
            Console.WriteLine("7 - Вывести информацию о классе");
            Console.WriteLine("8 - Вывести рейтинг классов по средним оценкам");
            ConsoleKeyInfo key = Console.ReadKey();
            Console.Clear();
            switch (key.KeyChar)
            {
                case '1':
                    controllers.AddDeleteClass(true);
                    break;
                case '2':
                    controllers.AddDeleteClass(false);
                    break;
                case '3':
                    controllers.SetClassMarks();
                    break;
                case '4':
                    controllers.WriteAllClasses();
                    break;
                case '5':
                    controllers.WriteStudentsOfClass();
                    break;
                case '6':
                    controllers.WriteLessons();
                    break;
                case '7':
                    controllers.FindAndWriteInfoAboutClass();
                    break;
                case '8':
                    controllers.WriteClassRating();
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
                    controllers.AddDeleteStudent(true);
                    break;
                case '2':
                    controllers.AddDeleteStudent(false);
                    break;
                case '3':
                    controllers.SetStudentMark(true);
                    break;
                case '4':
                    controllers.SetStudentMark(false);
                    break;
                case '5':
                    controllers.FindAndWriteInfoAboutStudentMarks();
                    break;
                case '6':
                    controllers.FindAndWriteInfoAboutStudent();
                    break;
                default:
                    Console.Clear();
                    return;
            }
        }
        #endregion
    }
}