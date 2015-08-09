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

    public class StudentVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public MainPanel MainPanel { get; set; }
        public MyContext Context { get; set; }
        public ObservableCollection<Course> AllCourses { get; set; }
        public Student Student { get; set; }

        public string Contents { get; set; }
        public int Priority { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public ObservableCollection<Course> Courses { get; set; }
        public StudentVM Parent { get; set; }
        public ObservableCollection<StudentVM> Children { get; set; }

        public DisplayPanel DisplayPanel {get; set;}

        public StudentVM(MainPanel mainPanel) {
            Children = new ObservableCollection<StudentVM>();
            MainPanel = mainPanel;
            Context = MainPanel.Context;
            Context.Courses.Load();
            AllCourses = Context.Courses.Local;
        }

        public StudentVM(Student student, MainPanel mainPanel) : this(mainPanel) {
            //NOTE: this constructor just WRAPS a student in a VM.
            bindToStudent(student);
            //Children = Student.Children; Children should be filled elsewhere.
        }

        public StudentVM(String name, MainPanel mainPanel) : this(mainPanel) {
            //NOTE: this one creates the Student, and THEN wraps it!!!
            Student newStudent = new Student(name);
            Context.Students.Add(newStudent);
            bindToStudent(newStudent);
        }

        public void bindToStudent(Student student) {
            Student = student;
            Contents = Student.Contents;
            Priority = Student.Priority;
            Courses = Student.Courses;
        }

        public void insertNote() {
            MainPanel.DisplayPanel.EntriesView.insertNote();
        }

        public void insertSubNote(StudentVM studentVM) {
            MainPanel.DisplayPanel.EntriesView.insertSubNote(studentVM);
        }
    }
}
