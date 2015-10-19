using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;

namespace PowerNote.Models {
    public class Entry: INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public int EntryID { get; set; }
        public string Type { get; set; }
        public DateTime CreationDate { get; set; }
        ObservableCollection<Tag> tags;
        public virtual ObservableCollection<Tag> Tags {
            get { return tags; }
            set { tags = value; NotifyPropertyChanged("Tags"); }
        }
        public virtual Entry Parent { get; set; }
        public virtual ObservableCollection<Entry> Children { get; set; }
        //NOTE: somehow, EntFramework inherently knows that Parent and Children are related.
        //BUT actually, it CAN'T here! how can it distinguish, since Parent and Children are BOTH of type Entry?
        //IT COULD well interpret it as Children means Siblings, and think its a manyToMany relation!

        protected void NotifyPropertyChanged(String propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public Entry() {
            CreationDate = DateTime.Parse("2010-09-01");
            Tags = new ObservableCollection<Tag>();
            Children = new ObservableCollection<Entry>();
        }

    }
}
