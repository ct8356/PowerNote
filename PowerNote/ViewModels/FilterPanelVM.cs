using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.Models;

namespace PowerNote.ViewModels {
    public class FilterPanelVM : ListBoxPanelVM {
        //public ObservableCollection<Tag> Tags { get; set; }
        //public ObservableCollection<Tag> SelectedTags { get; set; }
             
        public FilterPanelVM() {
            Objects = new ObservableCollection<object>();
            SelectedObjects = new ObservableCollection<object>();;
        }

        public override void addSelectedItem(object selectedItem) {
            SelectedObjects.Add(selectedItem as Tag);
        }

    }
}
