using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;

namespace PowerNote.Models {

    public class Student : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        //SOME people say, models should know nothing about your DALayer.
        //WHICH might be true. Just make DAL respond to events started here...
        public int StudentID { get; set; }
        public string LastName { get; set; }
        string contents;
        public string Contents {
            get { return contents; }
            set {
                contents = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Contents"));
                    //This basically means, when set, FIRE THE EVENT, "propertyChanged".
                } //IF clause is needed, coz set is often called, when PropChanged is not true.
            }
        }
        int priority;
        public int Priority {
            get { return priority; }
            set {
                priority = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Priority"));
                }
            }
        }
        public DateTime EnrollmentDate { get; set; }
        ObservableCollection<Course> courses;
        public virtual ObservableCollection<Course> Courses {
            get { return courses; }
            set {
                courses = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Courses"));
                }
            }
        }
        public virtual Student Parent { get; set; }
        public virtual ObservableCollection<Student> Children { get; set; }

        public Student() {
            Courses = new ObservableCollection<Course>();
            Priority = 10;
        }

        public Student(string contents) : this() {
            this.contents = contents;
            LastName = "Alexander";
            EnrollmentDate = DateTime.Parse("2010-09-01");
        }
    }
}
