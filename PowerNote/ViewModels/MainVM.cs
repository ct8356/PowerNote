    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Data.Entity;
    using PowerNote.DAL;
    using PowerNote.Migrations;
    using PowerNote.Models;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using CJT;

namespace PowerNote.ViewModels {
    public class MainVM : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public DAL.DbContext DbContext { get; set; }
        public ComboBoxVM TypePanelVM { get; set; }
        public ComboBoxVM StructurePanelVM { get; set; }
        public ObservableCollection<Tag> AllTags { get; set; }
        public ListBoxPanelVM<Tag> FilterPanelVM { get; set; }
        public OptionsPanelVM OptionsPanelVM { get; set; }
        public EntryClassOptionsVM EntryClassOptionsVM { get; set; }
        public EntriesTreeVM EntriesTreeVM { get; set; }
        private EntryVM selectedEntryVM;
        public EntryVM SelectedEntryVM {
            get { return selectedEntryVM; }
            set { selectedEntryVM = value; NotifyPropertyChanged("SelectedEntryVM"); }
        }

        public MainVM() {
            createDbContext();
            seedDatabase();
            TypePanelVM = new ComboBoxVM(this);
            TypePanelVM.Objects = new ObservableCollection<object> { typeof(Entry), typeof(PartClass), typeof(PartInstance), typeof(Task) };
            TypePanelVM.SelectedObject = TypePanelVM.Objects.First();
            StructurePanelVM = new ComboBoxVM(this);
            StructurePanelVM.Objects = new ObservableCollection<object>() { "Parent", "Sensor" };
            StructurePanelVM.SelectedObject = StructurePanelVM.Objects.First();
            AllTags = new ObservableCollection<Tag>();
            DbContext.Tags.Load();
            foreach (Tag tag in DbContext.Tags.Local) {
                AllTags.Add(tag);
            }
            FilterPanelVM = new ListBoxPanelVM<Tag>(this); //NOTE does not seem to have anything in its lists REVISIT CURRENT
            FilterPanelVM.Objects = new ObservableCollection<Tag>(AllTags);
            OptionsPanelVM = new OptionsPanelVM(this);
            EntriesTreeVM = new EntriesTreeVM(this);
            SelectedEntryVM = EntriesTreeVM.FirstGenEntryVMs.First<EntryVM>();
            //AHAH! CBTL! CURRENT. BEST way to do this, is to instantiate the childVM here. and name it.
            //THEN, WHEN make new view in XAML, set its DataContext to this childVM!!!
            //SUBSCRIBE
        }

        public void createDbContext() {
            DbContext = new DAL.DbContext(); //works fine. I guess table is made now.
            //Ok, so this does indeed, create a database, if not already exist.
            //BUT WHERE is this database created? It is in ProgramFiles, in the SQLServer folder. Let's delete it.
            //ACtually, NO! the database is still not made? WTF? //MAYBE just deleting .mdf files from SQLServer data folder is bad idea.
            //IT PROBS still thinks that they exist. Yes I reckon. BUT then why when I changed Context class name, did it not make NEW one?
        }

        public void UpdateEntries() {
            EntriesTreeVM.UpdateEntries();
        }

        public void deleteDatabase() {
            if (DbContext.Database.Exists()) {
                DbContext.Database.Delete();
            }
        }

        protected void NotifyPropertyChanged(String propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void seedDatabase() {
            if (DbContext.Database.Exists()) {
                DbContext.Database.Delete();
            }
            DbContext.Database.Create();
            Configuration configuration = new Configuration();
            configuration.callSeed((DAL.DbContext)DbContext);          
            //YES! Now it works. WHy it no work without this? Me no know.  
        }

    }
}
