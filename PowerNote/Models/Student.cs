using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged

namespace PowerNote.Models {

    public class Student : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        //SOME people say, models should know nothing about your DALayer.
        //WHICH might be true. Just make DAL respond to events started here...
        public int StudentID { get; set; }
        public string LastName { get; set; }
        string firstMidName;
        public string FirstMidName {
            get { return firstMidName; }
            set {
                firstMidName = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("FirstMidName"));
                    //This basically means, when set, FIRE THE EVENT, "propertyChanged".
                } //IF clause is needed, coz set is often called, when PropChanged is not true.
            }
        }
        public DateTime EnrollmentDate { get; set; }
        ICollection<Course> courses;
        public virtual ICollection<Course> Courses {
            get { return courses; }
            set {
                courses = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Courses"));
                    //This basically means, when set, FIRE THE EVENT, "propertyChanged".
                }
            }
        }


        public Student() {
            Courses = new List<Course>();
        }

        public Student(string firstMidName) : this() {
            this.firstMidName = firstMidName;
            LastName = "Alexander";
            EnrollmentDate = DateTime.Parse("2010-09-01");
        }
    }
}
