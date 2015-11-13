﻿    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Data.Entity;
    using PowerNote.DAL;
    using PowerNote.Migrations;

namespace PowerNote.ViewModels {
    public class MainVM {
        public MyContext Context { get; set; }
        public TypePanelVM TypePanelVM { get; set; }
        public StructurePanelVM StructurePanelVM { get; set; }
        public FilterPanelVM FilterPanelVM { get; set; }
        public OptionsPanelVM OptionsPanelVM { get; set; }
        public EntryClassOptionsVM EntryClassOptionsVM { get; set; }
        public EntriesTreeVM EntriesTreeVM { get; set; }

        public MainVM() {
            makeDatabase();
            TypePanelVM = new TypePanelVM(this);
            StructurePanelVM = new StructurePanelVM(this);
            FilterPanelVM = new FilterPanelVM(this);
            OptionsPanelVM = new OptionsPanelVM(this);
            EntriesTreeVM = new EntriesTreeVM(this);
            //AHAH! CBTL! CURRENT. BEST way to do this, is to instantiate the childVM here. and name it.
            //THEN, WHEN make new view in XAML, set its DataContext to this childVM!!!
        }

        public void updateEntries() {
            EntriesTreeVM.updateEntries();
        }

        public void makeDatabase() {
            //DATA
            Context = new MyContext(); //works fine. I guess table is made now.
            //Context.Database.Delete();
            //Context.Database.Create(); Configuration configuration = new Configuration(); configuration.callSeed(Context);
            //Ok, so this does indeed, create a database, if not already exist.
            //BUT WHERE is this database created? It is in ProgramFiles, in the SQLServer folder. Let's delete it.
            //ACtually, NO! the database is still not made? WTF? //MAYBE just deleting .mdf files from SQLServer data folder is bad idea.
            //IT PROBS still thinks that they exist. Yes I reckon. BUT then why when I changed Context class name, did it not make NEW one?
            //YES! Now it works. WHy it no work without this? Me no know.  
        }
    }
}
