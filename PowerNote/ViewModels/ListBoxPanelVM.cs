using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PowerNote.ViewModels {
    public abstract class ListBoxPanelVM {
        public ObservableCollection<object> Objects { get; set; }
        public ObservableCollection<object> SelectedObjects { get; set; }

        public ListBoxPanelVM() {
            Objects = new ObservableCollection<object>();
            SelectedObjects = new ObservableCollection<object>();
        }

        public abstract void addSelectedItem(object selectedItem);

    }
}
