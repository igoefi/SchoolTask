using ConsoleApp11.Classes.Controllers;
using School.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp11.Classes.Abstraction
{

    public class ControllerAbstraction
    {
        protected AddDeleteController addDelete;
        protected SetController set;
        protected WriteInformationController write;
        protected SerializationController serialization;

        public ControllerAbstraction(SerializationController serialController, SetController setController,
            AddDeleteController addDeleteController, WriteInformationController writeInformationController)
        {
            serialization = serialController;
            set = setController;
            addDelete = addDeleteController;
            write = writeInformationController;
        }

        #region Serialization
        public Arrays ReadFile(out Stream stream, string fileName)
        {
            return serialization.ReadFile(out stream, fileName);
        }

        public void SaveFile(Arrays arrays, string fileName) =>
            serialization.SaveFile(arrays, fileName);
        #endregion
        #region Set
        public void SetStudentMark(bool isNewQuarter) =>
            set.SetStudentMark(isNewQuarter);

        public void SetClassMarks() =>
            set.SetClassMarks();
        #endregion
        #region AddDelete
        public void AddDeleteClass(bool isAdd) =>
            addDelete.AddDeleteClass(isAdd);


        public void AddDeleteLesson(bool isAdd) =>
            addDelete.AddDeleteLesson(isAdd);
        

        public void AddDeleteStudent(bool isAdd) =>
            addDelete.AddDeleteStudent(isAdd);
     

        public void AddDeleteTeacher(bool isAdd) =>
            addDelete.AddDeleteTeacher(isAdd);
        #endregion
        #region Write
        #region WriteRating
        public void WriteClassRating() =>
            write.WriteClassRating();

        public void WriteTeacherRating() =>
            write.WriteTeacherRating();
        #endregion
        #region WriteAboutPerson
        public void FindAndWriteInfoAboutTeacher() =>
            write.FindAndWriteInfoAboutTeacher();

        public void FindAndWriteInfoAboutClass() =>
            write.FindAndWriteInfoAboutClass();

        public void FindAndWriteInfoAboutStudent() =>
            write.FindAndWriteInfoAboutStudent();
        #endregion

        public void FindAndWriteInfoAboutStudentMarks() =>
            write.FindAndWriteInfoAboutStudentMarks();

        public void WriteAllClasses() =>
            write.WriteAllClasses();

        public void WriteStudentsOfClass() =>
            write.WriteStudentsOfClass();

        public void WriteAllTeachers() =>
            write.WriteAllTeachers();

        public void WriteLessons() =>
            write.WriteLessons();
        #endregion
    }
}
