using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.DAL;

namespace PowerNote.ViewModels {
    public class StructurePanelVM : ListBoxPanelVM {
        //public ObservableCollection<String> Structures { get; set; }
        //public ObservableCollection<String> SelectedStructures { get; set; }
             
        public StructurePanelVM(MainVM parentVM) : base(parentVM) {
            //context.ToDos.GetColumnNames();
            //Structures = new ObservableCollection<String>() { "Parent", "Sensor" };
            //SelectedStructures = new ObservableCollection<String>();
            //SelectedStructures.Add(Structures.Where(s => s == "Parent").First());
            Objects = new ObservableCollection<object>() { "Parent", "Sensor" };
            SelectedObjects = new ObservableCollection<object>();
            SelectedObjects.Add(Objects.Where(o => (o as string) == "Parent").First());
        }

        public override void addSelectedItem(object selectedItem) {           
            SelectedObjects.Clear();
            SelectedObjects.Add(selectedItem as String);
            base.addSelectedItem(selectedItem); //maybe lazy. (should use listeners?)
        }

    }
}
