using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT;

namespace PowerNote.ViewModels {
    class TagVM : EntryVM {
        public TagVM(Tag task, EntriesTreeVM treeVM) {
            //NOTE: this constructor just WRAPS a student in a VM.
            initialize(task, treeVM);
        }

        public TagVM(String name, EntriesTreeVM treeVM) {
            //NOTE: this one creates the Entry, and THEN wraps it!!!
            Tag newEntry = new Tag(name);
            initialize(newEntry, treeVM);
            DbContext.Tags.Add(newEntry);
        }

        public void initialize(Tag entry, EntriesTreeVM treeVM) {
            base.initialize(entry, treeVM);
        }

        protected override void initializePropertyList() {
            base.initializePropertyList();
            //revisit
        }

        public override void insertEntry(EntryVM selectedVM) {
            TagVM entryVM = new TagVM("blank", TreeVM);
            insertEntry(entryVM, selectedVM);
        }

        public override void insertSubEntry(EntryVM selectedVM) {
            TagVM entryVM = new TagVM((selectedVM.Entry as Tag).Title + " child", TreeVM);
            //this creates an entry too!
            insertSubEntry(entryVM, selectedVM);
        }
    }
}
