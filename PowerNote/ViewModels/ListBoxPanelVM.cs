using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.DAL;
using System.ComponentModel; //this allows INotifyPropertyChanged

namespace PowerNote.ViewModels {
    public abstract class ListBoxPanelVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<object> Objects { get; set; }

        private ObservableCollection<object> selectedObjects;
        public ObservableCollection<object> SelectedObjects {
            get { return selectedObjects; }
            set { selectedObjects = value; NotifyPropertyChanged("SelectedObjects"); }
        }

        public MyContext DbContext { get; set; }
        public MainVM ParentVM { get; set; }

        public ListBoxPanelVM(MainVM parentVM) {
            Objects = new ObservableCollection<object>();
            SelectedObjects = new ObservableCollection<object>();
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
