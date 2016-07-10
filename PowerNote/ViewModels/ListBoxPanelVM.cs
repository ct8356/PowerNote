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
    public class ListBoxPanelVM<T> : BaseClass, INotifyInputConfirmed, IRemovable where T : Entry {
        public delegate void ObjectEventHandler(object sender, ObjectEventArgs<T> e);
        public event ObjectEventHandler InputConfirmed;
        public MainVM MainVM { get; set; }
        public DbContext DbContext { get; set; }
        public ObservableCollection<T> Objects { get; set; }
        private ObservableCollection<T> selectedObjects;
        public ObservableCollection<T> SelectedObjects {
            get { return selectedObjects; }
            set { selectedObjects = value; NotifyPropertyChanged("SelectedObjects"); }
        }

        public ListBoxPanelVM(MainVM mainVM) {
            Objects = new ObservableCollection<T>();
            SelectedObjects = new ObservableCollection<T>();
            MainVM = mainVM;
            DbContext = mainVM.DbContext;
            //SUBSCRIBE
            InputConfirmed += This_InputConfirmed;
        }

        public void NotifyInputConfirmed(object input) {
            if (InputConfirmed != null)
                InputConfirmed(this, new ObjectEventArgs<T>(input as T));
        }

        public void Remove(object item) {
            SelectedObjects.Remove(item as T);
            MainVM.UpdateEntries();
        }

        public void This_InputConfirmed(object sender, ObjectEventArgs<T> e) {
            T item = e.Object as T;
            if (!Objects.Contains(item)) Objects.Add(item);
            if (!SelectedObjects.Contains(item)) {
                SelectedObjects.Add(item);
                //FIRE an event... or not.... use NotifyPropChanged?
                //ACTUALLY, FOR better DISTRIBUTION,
                //BETTER if this class just calls TreeVM to update itself!
                MainVM.UpdateEntries();
            }
        }

    }
}
