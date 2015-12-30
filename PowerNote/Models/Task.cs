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

        int duration;
        public int Duration {
            get {
                if (Children.Count == 0) { return duration; }
                else { return sumOfChildrensDurations(); }
            }
            set { duration = value; NotifyPropertyChanged("Duration"); }
        }

        bool completed;
        public bool Completed {
            get { return completed; }
            set { completed = value; NotifyPropertyChanged("Completed"); }
        }

        public Task(): base() {
            Type = "PowerNote.Models.Task";
            Priority = 10;
            Duration = 10;
        }

        public Task(string contents) : this() {
            this.content = contents;
        }

        public int sumOfChildrensDurations() {
            int sum = 0;
            foreach (Task child in Children) {
                sum = sum + child.Duration;
            }
            return sum;
        }
    }
}
