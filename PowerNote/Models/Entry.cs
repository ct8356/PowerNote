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
        public DateTime CreationDate { get; set; }
        ObservableCollection<Tag> tags;
        public virtual ObservableCollection<Tag> Tags {
            get { return tags; }
            set { tags = value; NotifyPropertyChanged("Tags"); }
        }
        public virtual Entry Parent { get; set; }
        public virtual ObservableCollection<Entry> Children { get; set; }

        protected void NotifyPropertyChanged(String propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


      
    }
}
