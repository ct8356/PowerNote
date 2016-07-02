using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.Models;
using PowerNote.DAL;

namespace PowerNote.ViewModels {
    public class ComboBoxVM {
        public ObservableCollection<object> Objects { get; set; }
        public object SelectedObject { get; set; }

        public ComboBoxVM(MainVM parentVM) {  
            //SO YOU CAN make object = something. BUT with lists, for some reason,it gets complicated. 
            //I SAY, just gonna have to iterate, probs...
            Objects = new ObservableCollection<object> { typeof(Entry), typeof(PartClass), typeof(PartInstance), typeof(Task) };
            SelectedObject = Objects.First();
        }

        public void updateSelectedObject(object selectedObject) {
            SelectedObject = selectedObject;
        }


    }
}
