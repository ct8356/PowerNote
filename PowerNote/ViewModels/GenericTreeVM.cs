using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.ViewModels;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq.Expressions;
using CJT;
using AutoCompleteBox = CJT.AutoCompleteBox;

namespace PowerNote.ViewModels {
    public class GenericTreeVM<T> : EntriesTreeVM where T : Entry {
        public new ObservableCollection<EntryVM<T>> FirstGenEntryVMs { get; set; } 
        //needed for treeview to bind to
        public new ObservableCollection<EntryVM<T>> AllEntryVMs { get; set; }

        public GenericTreeVM(MainVM parentVM) : base(parentVM) {
        }

    }
}
