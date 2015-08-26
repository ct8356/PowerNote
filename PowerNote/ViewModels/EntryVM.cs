using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using PowerNote.Models;
using PowerNote.DAL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows;

namespace PowerNote.ViewModels {
    public class EntryVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public Entry Entry { get; set; }
        public MainPanel MainPanel { get; set; }
        public MyContext Context { get; set; }
        public ObservableCollection<Tag> AllTags { get; set; }
        public DateTime CreationDate { get; set; }
        public ObservableCollection<Tag> Tags { get; set; }
        public PartVM Parent { get; set; }
        public ObservableCollection<EntryVM> Children { get; set; }
        public DisplayPanel DisplayPanel { get; set; }

        public EntryVM(MainPanel mainPanel) {
            Children = new ObservableCollection<EntryVM>();
            MainPanel = mainPanel;
            Context = MainPanel.Context;
            Context.Tags.Load();
            AllTags = Context.Tags.Local;
        }

        public void deleteEntry() {
            Context.Entrys.Remove(Entry);
            Context.SaveChanges(); //ALSO lazy. CBTL.
            //SO NOTE: OF COURSE, easier you just do these things, IN THE VIEWMODEL!
            MainPanel.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
        }
    }
}
