using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel; //this allows INotifyPropertyChanged

namespace PowerNote.Models {
    public class TaggedObject : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        ICollection<Tag> tags;
        public virtual ICollection<Tag> Tags {
            get { return tags; }
            set {
                tags = value;
                if (PropertyChanged != null) {
                    PropertyChanged(this, new PropertyChangedEventArgs("Tags"));
                    //This basically means, when set, FIRE THE EVENT, "propertyChanged".
                }
            }
        }
    }
}
