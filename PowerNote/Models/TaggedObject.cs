using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel; //this allows INotifyPropertyChanged

namespace PowerNote.Models {
    public class TaggedObject : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

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
    }
}
