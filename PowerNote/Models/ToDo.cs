using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;

namespace PowerNote.Models {

    public class ToDo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        //SOME people say, models should know nothing about your DALayer.
        //WHICH might be true. Just make DAL respond to events started here...
        public int ToDoID { get; set; }
        string content;
        public string Contents {
            get { return content; }
            set {
                content = value;
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
        ObservableCollection<Tag> tags;
        public virtual ObservableCollection<Tag> Tags {
            get { return tags; }
            set {
                tags = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Tags"));
                }
            }
        }
        public virtual ToDo Parent { get; set; }
        public virtual ObservableCollection<ToDo> Children { get; set; }

        public ToDo() {
            Tags = new ObservableCollection<Tag>();
            Priority = 10;
        }

        public ToDo(string contents) : this() {
            this.content = contents;
            EnrollmentDate = DateTime.Parse("2010-09-01");
        }
    }
}
