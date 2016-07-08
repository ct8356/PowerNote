using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.DAL;
using System.ComponentModel; //this allows INotifyPropertyChanged
using PowerNote.Models;
using CJT;

namespace PowerNote.ViewModels {
    public class ListBoxPanelVM<T> : BaseClass, INotifyInputConfirmed where T : Entry {
        public delegate void ObjectEventHandler(object sender, ObjectEventArgs<T> e);
        public event ObjectEventHandler InputConfirmed;
        public DbContext DbContext { get; set; }
        public ObservableCollection<T> Objects { get; set; }
        private ObservableCollection<T> selectedObjects;
        public ObservableCollection<T> SelectedObjects {
            get { return selectedObjects; }
            set { selectedObjects = value; NotifyPropertyChanged("SelectedObjects"); }
        }

        public ListBoxPanelVM(DbContext dbContext) {
            Objects = new ObservableCollection<T>();
            SelectedObjects = new ObservableCollection<T>();
            DbContext = dbContext;
        }

        public virtual void addSelectedItem(T selectedItem) {
            SelectedObjects.Add(selectedItem);
            //FIRE an event... or not.... use NotifyPropChanged?
        }

        public void NotifyInputConfirmed(object input) {
            InputConfirmed(this, new ObjectEventArgs<T>(input as T));
        }

    }
}
