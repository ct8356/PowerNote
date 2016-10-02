using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace PowerNote.Models {
    public class Tag : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //?? CBTL? important?
        public int TagID { get; set; }
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
        public virtual ICollection<Entry> Entries { get; set; }

        public Tag() {
            //do nothing
        }

        public Tag(string title) {
            this.title = title;
        }

        public override string ToString() {
            return Title;
        }
    }
}
