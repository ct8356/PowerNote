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

namespace PowerNote {
    partial class EntriesTreeView : TreeView {
        DisplayPanel displayPanel;
        TaggedObject filter;
        IQueryable<Entry> filteredEntries;
        IQueryable<Task> filteredStudents;
        public ObservableCollection<EntryVM> FirstGenEntryVMs { get; set; } 
        //needed for treeview to bind to
        public ObservableCollection<EntryVM> AllEntryVMs { get; set; }
        //needed to keep track of EntryVMs shown in the treeView (without having to scan tree).
        MyContext context;
        ToDoPanel newEntry; //keep for now
        int childLevel;
        public Entry Orphan {get; set;}
        public bool WaitingForParentSelection { get; set; }

        public EntriesTreeView() {
            //DO nought.
        }

        public EntriesTreeView(MyContext context, DisplayPanel displayPanel) {
            this.context = context;
            this.displayPanel = displayPanel;
            FirstGenEntryVMs = new ObservableCollection<EntryVM>();
            AllEntryVMs = new ObservableCollection<EntryVM>();
            filterEntries();
            //filterToDos();
            //SUBSCRIBE
            displayPanel.OptionsPanel.ShowAllEntriesCBox.Click += ShowAllEntries_Click; //subscribe
            displayPanel.OptionsPanel.ShowAllChildrenCBox.Click += ShowAllChildren_Click; //subscribe
            //INITIALIZE
            InitializeComponent();
            int count = FirstGenEntryVMs.Count();
            //newEntry.LostFocus += new RoutedEventHandler(newEntry_LostFocus);
            //newEntry.KeyUp += new KeyEventHandler(newEntry_KeyUp);
        }

        public void filterEntries() {
            filter = displayPanel.FilterPanel.Filter;
            IEnumerable<int> filterCourseIDs = filter.Tags.Select(c => c.TagID);
            filteredEntries = context.Entrys;
            if (!displayPanel.OptionsPanel.ShowAllEntries)
                filteredEntries = filteredEntries
                .Where(s => !filterCourseIDs.Except(s.Tags.Select(c => c.TagID)).Any());
            //ORDERBY
            switch (displayPanel.SortPanel.ComboBox.SelectedItem.ToString()) {
                case "ID": break;
                case "Priority": break;
            }
            IQueryable<Entry> filteredParents = showFirstLevel();
            for (int gen = 2; gen <= 5; gen++) {
                filteredParents = showMoreLevels(filteredParents);
            }
        }

        public void sortTasks() {
            //ORDERBY
            switch (displayPanel.SortPanel.ComboBox.SelectedItem.ToString()) {
                case "ID": break;
                case "Priority": filteredStudents = filteredStudents.OrderBy(s => s.Priority); break;
            }
        }

        public void newEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                newEntry_LostFocus(sender, e);
            }
        }

        public void newEntry_LostFocus(object sender, RoutedEventArgs e) {
            if (newEntry.TextBox.Text != null && newEntry.TextBox.Text != "") {
                Task newStudent = new Task(newEntry.TextBox.Text);
                context.ToDos.Add(newStudent);
                context.SaveChanges();
                updateEntries(); //CBTL. Lazy way. (rather than using events). But ok for now.   
            }
        }

        public void ShowAllChildren_Click(object sender, EventArgs e) {
            updateEntries();
        }

        public void ShowAllEntries_Click(object sender, EventArgs e) {
            updateEntries();
        }

        public IQueryable<Entry> showFirstLevel() {
            IEnumerable<int> filterTagIDs = filter.Tags.Select(c => c.TagID);
            IQueryable<Entry> filteredParents = null;
            if (displayPanel.OptionsPanel.ShowAllEntries) {
                filteredParents = filteredEntries.Where(s => s.Parent == null);
                //WHERE it has NO parent.
            }
            else {
                filteredParents = filteredEntries
                .Where(s => filterTagIDs.Except(s.Parent.Tags.Select(c => c.TagID)).Any());
                //WHERE it has NO parent that matches the filter.
            }
            foreach (Entry entry in filteredParents) {
                //CBTL: NEED to do a TYPE check here!!!
                EntryVM entryVM = null;
                if (entry is PartClass)
                    entryVM = new PartClassVM(entry as PartClass, displayPanel.MainPanel);
                if (entry is PartInstance)
                    entryVM = new PartInstanceVM(entry as PartInstance, displayPanel.MainPanel);
                if (entry is Task)
                    entryVM = new TaskVM(entry as Task, displayPanel.MainPanel);
                //FINALLY
                FirstGenEntryVMs.Add(entryVM);
                AllEntryVMs.Add(entryVM);
            } //seems to work FINE up to here...
            return filteredParents;
        }

        public IQueryable<Entry> showMoreLevels(IQueryable<Entry> filteredParents) {
            childLevel++;
            IQueryable<Entry> filteredChildren = null;
            IEnumerable<int> parentEntryIDs = filteredParents.Select(s => s.EntryID);
            if (displayPanel.OptionsPanel.ShowAllChildren) {
                filteredChildren =
                    context.Entrys.Where(s => parentEntryIDs.Contains(s.Parent.EntryID));
            }
            else {
                filteredChildren =
                    filteredEntries.Where(s => parentEntryIDs.Contains(s.Parent.EntryID));
            } //NOTE! innefficiency above, in that it does this for parents too...
            foreach (Entry child in filteredChildren) {
                if (child.Parent != null) { //if kid has a parent
                    try {     
                        EntryVM parentVM = AllEntryVMs.Where(eVM => eVM.Entry == child.Parent).Single();
                        //FIND the parentVM.
                        //AHAH! WERE searching in the wrong list...
                        EntryVM entryVM = null;
                        if (child is PartClass)
                            entryVM = new PartClassVM(child as PartClass, displayPanel.MainPanel);
                        //AHAH! CBTL! Has to be passed a PARTVM, not ENTRYVM, or cannot tell what it is!
                        //BUT DUH! IF the VM is CREATED as an ENTRYVM, then it is NOT a PARTVM!!!
                        //SO FAIR PLAY! CBTL. I SAY, just create proper one here for now. Fine.
                        //Too much dawdling already.
                        if (child is PartInstance)
                            entryVM = new PartInstanceVM(child as PartInstance, displayPanel.MainPanel);
                        if (child is Task)
                            entryVM = new TaskVM(child as Task, displayPanel.MainPanel);
                        //FINALLY
                        parentVM.Children.Add(entryVM);
                        AllEntryVMs.Add(entryVM);
                    }
                    catch (Exception e) {
                        //Do nothing
                    }
                }
            }
            return filteredChildren;
        }

        public void showUntaggedEntries() {
            IQueryable<Task> untaggedStudents =
                   filteredStudents.Where(s => !s.Tags.Any()); //Will this work?
            foreach (Task student in untaggedStudents) {
                //EntryNode entryNode = new EntryNode(student, displayPanel.MainPanel);
            }
        }

        public void updateEntries() {
            FirstGenEntryVMs.Clear();
            AllEntryVMs.Clear();
            filterEntries();
        }

        public void waitForParentSelection(Entry entry) {
            WaitingForParentSelection = true;
            Orphan = entry;
            updateEntries();
            //this will update the treeView, AND if this is true,
            //it will be redrawn with certain colour scheme, to make the waiting, obvious.
            //drag and drop won't be too hard, BUT start here. easier.
            //ALSO, WHEN TRUE, need to create a LISTENER,
            //THAT subscribes now to the itemClick event.
            //and WHEN fires,
            //it will call that PANELs dataContext, and its method adoptChild(orphan);
            //WILL JUST HAVE TO UNSUBSCRIBE! It is poss. -=...
        }

    }
}
