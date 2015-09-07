using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;

namespace PowerNote.Models {

    public class Task : Entry {
        //SOME people say, models should know nothing about your DALayer.
        //WHICH might be true. Just make DAL respond to events started here...
        string content;
        public string Contents {
            get { return content; }
            set { content = value; NotifyPropertyChanged("Contents"); } 
            //IF clause is needed, coz set is often called, when PropChanged is not true.
        }
        int priority;
        public int Priority {
            get { return priority; }
            set { priority = value; NotifyPropertyChanged("Priority"); }
        }

        public Task(): base() {
            Priority = 10;
        }

        public Task(string contents) : this() {
            this.content = contents;
        }
    }
}
