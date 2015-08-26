using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using PowerNote.Models;
using PowerNote.DAL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PowerNote.ViewModels {

    public class StudentVM : EntryVM {
        public ToDo Student { get; set; }
        public string Contents { get; set; }
        public int Priority { get; set; }

        public StudentVM(ToDo student, MainPanel mainPanel) : base(mainPanel) {
            //NOTE: this constructor just WRAPS a student in a VM.
            bindToStudent(student);
            //Children = Student.Children; Children should be filled elsewhere.
        }

        public StudentVM(String name, MainPanel mainPanel) : base(mainPanel) {
            //NOTE: this one creates the Student, and THEN wraps it!!!
            ToDo newStudent = new ToDo(name);
            Context.ToDos.Add(newStudent);
            bindToStudent(newStudent);
        }

        public void bindToStudent(ToDo student) {
            Student = student;
            Contents = Student.Contents;
            Priority = Student.Priority;
            Tags = Student.Tags;
        }

        public void insertNote() {
            MainPanel.DisplayPanel.EntriesView.insertNote();
        }

        public void insertChild(StudentVM studentVM) {
            MainPanel.DisplayPanel.EntriesView.insertSubNote(studentVM);
        }
    }
}
