using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.DAL;

namespace PowerNote.ViewModels {
    public class StructurePanelVM : ListBoxPanelVM<object> {

        public StructurePanelVM(MainVM parentVM) : base(parentVM) {
            //context.ToDos.GetColumnNames();
            Objects = new ObservableCollection<object>() { "Parent", "Sensor" };
            SelectedObjects = new ObservableCollection<object>();
            SelectedObjects.Add(Objects.Where(o => (o as string) == "Parent").First());
        }

        public override void addSelectedItem(object selectedItem) {           
            SelectedObjects.Clear();
            SelectedObjects.Add(selectedItem as object);
            base.addSelectedItem(selectedItem); //maybe lazy. (should use listeners?)
        }

    }
}
