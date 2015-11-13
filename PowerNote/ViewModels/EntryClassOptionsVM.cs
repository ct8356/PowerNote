using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using PowerNote.DAL;
using System.Collections.ObjectModel;

namespace PowerNote.ViewModels {
    public class EntryClassOptionsVM {
        MainVM ParentVM { get; set; }
        MyContext context;
        Type Type { get; set; }
        ObservableCollection<object> Properties { get; set; } //Prop Names better for now, since you use
        //strings to specify your columnNames!
        IEnumerable<string> PropertyNames { get; set; }

        public EntryClassOptionsVM(MainVM parentVM) {
            ParentVM = parentVM;
            this.context = parentVM.Context;
            Type = parentVM.TypePanelVM.SelectedObjects.First() as Type;
            PropertyNames = Type.GetProperties().Select(x => x.Name);
        }

    }
}