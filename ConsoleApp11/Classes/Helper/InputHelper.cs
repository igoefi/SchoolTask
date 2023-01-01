using School;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11.Classes.Helper
{
    public class InputHelper
    {
        protected static Arrays _arrays;

        internal InputHelper() => _arrays = Interface.Arrays;

        public static bool NameInput(out string teacherName, out string teacherSurname)
        {
            Console.WriteLine($"Напишите имя и фамилию учителя");

            Console.Write("Имя - ");
            teacherName = Console.ReadLine();

            Console.Write("Фамилия - ");
            teacherSurname = Console.ReadLine();

            if (_arrays.FindTeacher(teacherName, teacherSurname) == null)
            {
                return false;
            }

            return true;
        }

        public static bool NameInput(string className, out string name, out string surname)
        {
            Console.WriteLine($"Напишите имя и фамилию ученика");

            Console.Write("Имя - ");
            name = Console.ReadLine();

            Console.Write("Фамилия - ");
            surname = Console.ReadLine();

            if (_arrays.FindStudent(className, name, surname) == null)
            {
                Console.WriteLine("Этого ученика нет в данном классе");
                return false;
            }

            return true;
        }

        public static string ClassInput()
        {
            Console.Write("Название класса - ");
            string className = Console.ReadLine();

            if (className.ToLower() != "все" && _arrays.FindClass(className) == null)
            {
                Console.WriteLine("Такого класса не существует");
                return null;
            }

            return className;
        }

        public static string LessonInput()
        {
            Console.Write("Название урока - ");
            return Console.ReadLine();
        }
    }
}
