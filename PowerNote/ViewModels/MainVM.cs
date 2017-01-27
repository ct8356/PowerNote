    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using PowerNote.Migrations;
    //using PowerNote.Models;
    using PowerNote.DAL;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using CJT.ViewModels;
    using CJT;

namespace PowerNote.ViewModels {
    public class MainVM : IMainVM, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        public CJT.DbContext DbContext { get; set; }
        public EntryClassOptionsVM EntryClassOptionsVM { get; set; }
        public EntriesTreeVM TreeVM { get; set; }
        private EntryVM selectedEntryVM;
        public EntryVM SelectedEntryVM {
            get { return selectedEntryVM; }
            set { selectedEntryVM = value; NotifyPropertyChanged("SelectedEntryVM"); }
        }

        public MainVM() {
            createDbContext();
            //seedDatabase();     
            TreeVM = new EntriesTreeVM(this);
            if (TreeVM.FirstGenEntryVMs.Count != 0)
                SelectedEntryVM = TreeVM.FirstGenEntryVMs.First<EntryVM>();
            //AHAH! CBTL! CURRENT. BEST way to do this, is to instantiate the childVM here. and name it.
            //THEN, WHEN make new view in XAML, set its DataContext to this childVM!!!
            //SUBSCRIBE
        }

        public void createDbContext() {
            DbContext = new DAL.EFContext(); //works fine. I guess table is made now.
            //Ok, so this does indeed, create a database, if not already exist.
            //BUT WHERE is this database created? It is in ProgramFiles, in the SQLServer folder. Let's delete it.
            //ACtually, NO! the database is still not made? WTF? //MAYBE just deleting .mdf files from SQLServer data folder is bad idea.
            //IT PROBS still thinks that they exist. Yes I reckon. BUT then why when I changed Context class name, did it not make NEW one?
        }

        public void UpdateEntries() {
            if (TreeVM != null)
                TreeVM.UpdateEntries();
        }

        protected void NotifyPropertyChanged(String propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void seedDatabase() {
            DbContext.Database.Delete(); DbContext.Database.Create();
            Configuration configuration = new Configuration();
            configuration.callSeed((DAL.EFContext)DbContext);          
            //YES! Now it works. WHy it no work without this? Me no know.  
        }

    }
}
