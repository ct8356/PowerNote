using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.ViewModels;
using PowerNote.Migrations;
using System.Collections.ObjectModel;
using CJT;

namespace PowerNote.ViewModels {
    public class PartInstanceVM : EntryVM {
        public static IEnumerable<string> PropertyNames { get; set; }
        public static ObservableCollection<string> Structures { get; set; }
        public static string SelectedStructure { get; set; }
        //NOTE: might be a good idea to make own DbSet class,
        //AND then put these statics, in there.
        public IEnumerable<PartClass> NickNames { get; set; }

        public PartInstanceVM(Entry entry, EntriesTreeVM parentVM) {
            //NOTE: this constructor just WRAPS an entry in a VM.
            initialize(entry, parentVM);
            IQueryable<PartClass> partClasses = DbContext.Parts;
            PartClass partClass = partClasses.First();
            //(entry as PartInstance).PartClass = partClass;
            //WHAT THE!!??? NOW IT WORKS???
            //Weird.
        }

        public PartInstanceVM(String name, EntriesTreeVM parentVM) {
            //NOTE: this one creates the Entry, and THEN wraps it!!!
            PartInstance entry = new PartInstance(name);
            initialize(entry, parentVM);
            DbContext.PartInstances.Add(entry);
            DbContext.SaveChanges(); //remember this.
        }

        public new void initialize(Entry entry, EntriesTreeVM parentVM) {
            base.initialize(entry, parentVM);
            IQueryable<PartInstance> partInstances = DbContext.PartInstances;
            PropertyNames = typeof(PartInstance).GetProperties().Select(x => x.Name);
            NickNames = DbContext.Parts;
        }//NOTE: new, means, this is NOT called, if call EntryVM.initialize, and happens to be a partInstance.

        protected override void initializePropertyList() {
            base.initializePropertyList();
            AllProperties.Add(new Property("Function text", (Entry as PartInstance).FunctionText, InfoType.TextBox, true, DbContext));
            AllProperties.Add(new Property("Part class", (Entry as PartInstance).PartClass, InfoType.TextBox, false, DbContext));
            AllProperties.Add(new Property("Parent part", (Entry as PartInstance).ParentPartInstance, InfoType.TextBox, false, DbContext));
            AllProperties.Add(new Property("Children parts", (Entry as PartInstance).ChildPartInstances, InfoType.ListBox, false, DbContext));
            AllProperties.Add(new Property("Tasks", (Entry as PartInstance).ChildTasks, InfoType.ListBox, false, DbContext));
            AllProperties.Add(new Property("Sensor", (Entry as PartInstance).Sensor, InfoType.TextBox, false, DbContext));
        }

        public void addPartClassToEntry(string text) {
            IQueryable<PartClass> partClasses = DbContext.Parts;
            PartClass partClass = partClasses.Where(pc => pc.NickName == text).First();
            (Entry as PartInstance).PartClass = partClass;
            DbContext.SaveChanges();
            TreeVM.updateEntries();
            //CBTL, why was this not working before?
            //nav property? lack of update entries?
        }

        public override void insertEntry(EntryVM selectedVM) {
            PartInstanceVM entryVM = new PartInstanceVM("blank", TreeVM);
            insertEntry(entryVM, selectedVM);
        }

        public override void insertSubEntry(EntryVM parentVM) {
            PartInstanceVM entryVM = new PartInstanceVM((parentVM.Entry as PartInstance).FunctionText + " child", 
                TreeVM); //create part.
            insertSubEntry(entryVM, parentVM);
        }

    }
}
