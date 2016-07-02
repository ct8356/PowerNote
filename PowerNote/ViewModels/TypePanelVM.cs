using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.Models;
using PowerNote.DAL;

namespace PowerNote.ViewModels {
    public class TypePanelVM : ListBoxVM<object> {

        public TypePanelVM(MainVM parentVM) : base(parentVM) {
            //SO YOU CAN make object = something. BUT with lists, for some reason,it gets complicated. 
            //I SAY, just gonna have to iterate, probs...
            Objects = new ObservableCollection<object> { typeof(Entry), typeof(PartClass), typeof(PartInstance), typeof(Task) };
            SelectedObjects = new ObservableCollection<object>();
            SelectedObjects.Add(Objects.Where(o => (o as Type) == typeof(PartInstance)).First());
        }

        public void updateSelectedObject(object selectedObject) {
            SelectedObjects.Add(selectedObject);
        }


    }
}
