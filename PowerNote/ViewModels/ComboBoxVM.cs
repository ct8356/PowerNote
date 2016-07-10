using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.Models;
using PowerNote.DAL;
using CJT;

namespace PowerNote.ViewModels {
    public class ComboBoxVM {
        public ObservableCollection<object> Objects { get; set; }
        public object SelectedObject { get; set; }

        public ComboBoxVM(MainVM parentVM) {  
           
        }

        public void updateSelectedObject(object selectedObject) {
            SelectedObject = selectedObject;
        }


    }
}
