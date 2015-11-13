using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.DAL;

namespace PowerNote.ViewModels {
    public abstract class ListBoxPanelVM {
        public ObservableCollection<object> Objects { get; set; }
        public ObservableCollection<object> SelectedObjects { get; set; }
        public MyContext DbContext { get; set; }
        public MainVM ParentVM { get; set; }

        public ListBoxPanelVM(MainVM parentVM) {
            Objects = new ObservableCollection<object>();
            SelectedObjects = new ObservableCollection<object>();
            DbContext = parentVM.Context;
            ParentVM = parentVM;
        }

        public virtual void addSelectedItem(object selectedItem) {
            ParentVM.updateEntries();
        }

    }
}
