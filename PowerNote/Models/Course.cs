using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PowerNote.Models {
    public class Course : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //?? CBTL? important?
        public int CourseID { get; set; }
        string title;
        public string Title {
            get { return title; }
            set {
                title = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                }
            }
        }
        public int Credits { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }

        public override string ToString() {
            return Title;
        }
    }
}
