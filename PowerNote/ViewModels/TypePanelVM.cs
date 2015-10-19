using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using PowerNote.Models;

namespace PowerNote.ViewModels {
    class TypePanelVM : ListBoxPanelVM {
        //public ObservableCollection<Type> Types { get; set; }
        //public ObservableCollection<Type> SelectedTypes { get; set; }

        public TypePanelVM() {
            //SO YOU CAN make object = something. BUT with lists, for some reason,it gets complicated. 
            //I SAY, just gonna have to iterate, probs...
            //Types = new ObservableCollection<Type>() { typeof(Entry), typeof(PartClass), typeof(PartInstance) };
            Objects = new ObservableCollection<object> { typeof(Entry), typeof(PartClass), typeof(PartInstance) };
            //SelectedTypes = new ObservableCollection<Type>();
            //SelectedTypes.Add(Types.Where(t => t == typeof(PartInstance)).First());
            SelectedObjects = new ObservableCollection<object>();
            SelectedObjects.Add(Objects.Where(o => (o as Type) == typeof(PartInstance)).First());
        }

        public override void addSelectedItem(object selectedItem) {
            //SelectedTypes.Clear();
            //SelectedTypes.Add(selectedItem as Type);
            SelectedObjects.Clear();
            SelectedObjects.Add(selectedItem as Type);
        }

    }
}
