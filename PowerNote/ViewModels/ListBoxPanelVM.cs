using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.DAL;
using System.ComponentModel; //this allows INotifyPropertyChanged
using PowerNote.Models;

namespace PowerNote.ViewModels {
    public class ListBoxVM<T> : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<T> Objects { get; set; }

        private ObservableCollection<T> selectedObjects;
        public ObservableCollection<T> SelectedObjects {
            get { return selectedObjects; }
            set { selectedObjects = value; NotifyPropertyChanged("SelectedObjects"); }
        }
        public List<string> TestString { get; set; } = new List<string>() { "fah", "lah" };

        public DbContext DbContext { get; set; }
        public MainVM ParentVM { get; set; }

        public ListBoxVM(MainVM parentVM) {
            Objects = new ObservableCollection<T>();
            SelectedObjects = new ObservableCollection<T>();
            DbContext = parentVM.DbContext;
            ParentVM = parentVM;
        }

        public virtual void addSelectedItem(object selectedItem) {
            ParentVM.updateEntries();
            //FIRE an event... or not....
        }

        protected void NotifyPropertyChanged(String propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
