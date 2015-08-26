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
        IQueryable<ToDo> filteredStudents;
        public ObservableCollection<EntryVM> EntryVMs { get; set; }
        public ObservableCollection<StudentVM> StudentVMs { get; set; }
        MyContext context;
        ToDoPanel newEntry; //keep for now
        int childLevel;

        public EntriesTreeView() {
            //DO nought.
        }

        public EntriesTreeView(MyContext context, DisplayPanel displayPanel) {
            this.context = context;
            this.displayPanel = displayPanel;
            EntryVMs = new ObservableCollection<EntryVM>();
            filterEntries();
            //filterToDos();
            //SUBSCRIBE
            displayPanel.OptionsPanel.ShowAllEntries.Click += ShowAllEntries_Click; //subscribe
            displayPanel.OptionsPanel.ShowAllChildren.Click += ShowAllChildren_Click; //subscribe
            //INITIALIZE
            InitializeComponent();
            int count = EntryVMs.Count();
        }

        public void filterEntries() {
            filter = displayPanel.FilterPanel.Filter;
            IEnumerable<int> filterCourseIDs = filter.Tags.Select(c => c.TagID);
            filteredEntries = context.Entrys;
            filteredEntries = filteredEntries
                .Where(s => !filterCourseIDs.Except(s.Tags.Select(c => c.TagID)).Any());
            //ORDERBY
            switch (displayPanel.SortPanel.ComboBox.SelectedItem.ToString()) {
                case "ID": break;
                case "Priority": break;
            }
            IQueryable<Entry> filteredParents = showFirstLevel();
            //IQueryable<ToDo> filteredChildren = showMoreLevels(filteredParents);
        }

        public void filterToDos() {
            filter = displayPanel.FilterPanel.Filter;
            IEnumerable<int> filterCourseIDs = filter.Tags.Select(c => c.TagID);
            filteredStudents = context.ToDos;
            filteredStudents = filteredStudents
                .Where(s => !filterCourseIDs.Except(s.Tags.Select(c => c.TagID)).Any());
            int mycount = filteredStudents.Count();
            //ORDERBY
            switch (displayPanel.SortPanel.ComboBox.SelectedItem.ToString()) {
                case "ID": break;
                case "Priority": filteredStudents = filteredStudents.OrderBy(s => s.Priority); break;
            }
            if (displayPanel.OptionsPanel.ShowAllEntriesBool) {
                showAllEntries();
            }
            else {
                IQueryable<ToDo> filteredParents = showFirstLevel1();
                //ItemsSource = StudentVMs; //Now works! So, making it new, BREAKS the bind, it seems.
                IQueryable<ToDo> filteredChildren = showMoreLevels(filteredParents);
                //filteredChildren = showDeeperLevels(filteredChildren);
            }
        }

        public void insertPart() {
            EntryVMs.Add(new PartVM("blank", displayPanel.MainPanel));
        }

        public void insertNote() {
            //PROBLEM IS, the view is only bound to STUDENTVMs.
            //SO, need to create new STudentVM, add it to the list,
            //THEN, StudentVM itself, should be incharge of making a student,
            //and adding it to the list.
            EntryVMs.Add(new StudentVM("blank", displayPanel.MainPanel));
            //CBTL! Will have to make the below, part of the template???
            //Well yes. BUT technically, not needed now.
            //MIGHT be useful for the RANDOMNOTE window... (if even do that,
            //rather than just MULTIPLE windows). in fairness, it might have its uses...
            //newEntry.LostFocus += new RoutedEventHandler(newEntry_LostFocus);
            //newEntry.KeyUp += new KeyEventHandler(newEntry_KeyUp);
        }

        public void insertSubNote(StudentVM studentVM) {
            //NOT SAFE TO USE YET!
            StudentVM newStudent = new StudentVM(studentVM.Contents + " child", displayPanel.MainPanel);
            EntryVMs.Add(newStudent);
            studentVM.Children.Add(newStudent); //CBTL GONNA CAUSE ISSUES!
            //coz when does it get added to the STUDENTS children???
            context.ToDos.Add(newStudent.Student);
            context.SaveChanges();
            displayPanel.MainPanel.updateEntries();
            context.SaveChanges();
        }

        public void newEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                newEntry_LostFocus(sender, e);
            }
        }

        public void newEntry_LostFocus(object sender, RoutedEventArgs e) {
            if (newEntry.TextBox.Text != null && newEntry.TextBox.Text != "") {
                ToDo newStudent = new ToDo(newEntry.TextBox.Text);
                context.ToDos.Add(newStudent);
                context.SaveChanges();
                updateEntries(); //CBTL. Lazy way. (rather than using events). But ok for now.   
            }
        }

        public void ShowAllChildren_Click(object sender, EventArgs e) {
            updateEntries();
        }

        public void ShowAllEntries_Click(object sender, EventArgs e) {
            EntryVMs.Clear();
            showAllEntries();
        }

        public void showAllEntries() {
            filteredEntries = context.Entrys;
            IQueryable<Entry> filteredParents = showFirstLevelAll();
        }

        public IQueryable<Entry> showFirstLevel() {
            IEnumerable<int> filterTagIDs = filter.Tags.Select(c => c.TagID);
            IQueryable<Entry> filteredParents = filteredEntries
                .Where(s => filterTagIDs.Except(s.Parent.Tags.Select(c => c.TagID)).Any());
            //WHERE it has NO parent that matches the filter.
            foreach (Entry entry in filteredParents) {
                //CBTL: NEED to do a TYPE check here!!!
                if (entry is Part)
                EntryVMs.Add(new PartVM(entry as Part, displayPanel.MainPanel)); //CBTL. problem?
            } //seems to work FINE up to here...
            return filteredParents;
        }

        public IQueryable<Entry> showFirstLevelAll() {
            IQueryable<Entry> filteredParents = filteredEntries.Where(s => s.Parent == null);
            //WHERE it has NO parent.
            foreach (Entry entry in filteredParents) {
                //CBTL: NEED to do a TYPE check here!!!
                if (entry is Part)
                    EntryVMs.Add(new PartVM(entry as Part, displayPanel.MainPanel)); //CBTL. problem?
            } //seems to work FINE up to here...
            return filteredParents;
        }

        public IQueryable<ToDo> showFirstLevel1() {
            IEnumerable<int> filterTagIDs = filter.Tags.Select(c => c.TagID);
            IQueryable<ToDo> filteredParents = filteredStudents
                .Where(s => filterTagIDs.Except(s.Parent.Tags.Select(c => c.TagID)).Any());
            //WHERE it has NO parent.
            foreach (ToDo toDo in filteredParents) {
                EntryVMs.Add(new StudentVM(toDo, displayPanel.MainPanel));
            }
            return filteredParents;
        }

        public IQueryable<ToDo> showMoreLevels(IQueryable<ToDo> filteredParents) {
            childLevel++;
            IQueryable<ToDo> filteredChildren = null;
            IEnumerable<int> parentStudentIDs = filteredParents.Select(s => s.ToDoID);
            if (displayPanel.OptionsPanel.ShowAllChildrenBool) {
                filteredChildren =
                    context.ToDos.Where(s => parentStudentIDs.Contains(s.Parent.ToDoID));
            }
            else {
                filteredChildren =
                    filteredStudents.Where(s => parentStudentIDs.Contains(s.Parent.ToDoID));
            } //NOTE! innefficiency above, in that it does this, for parents too...
            foreach (ToDo child in filteredChildren) {
                if (child.Parent != null) { //if kid has a parent
                    try {
                        foreach (EntryVM eVM in EntryVMs) {
                            StudentVMs.Add(eVM as StudentVM);
                        }
                        StudentVM viewParentVM = StudentVMs.Where(sVM => sVM.Student == child.Parent).Single();
                        viewParentVM.Children.Add(new StudentVM(child, displayPanel.MainPanel));
                    }
                    catch (Exception e) {
                        //Do nothing
                    }
                }
            }
            return filteredChildren;
        }

        public void showUntaggedEntries() {
            IQueryable<ToDo> untaggedStudents =
                   filteredStudents.Where(s => !s.Tags.Any()); //Will this work?
            foreach (ToDo student in untaggedStudents) {
                //EntryNode entryNode = new EntryNode(student, displayPanel.MainPanel);
            }
        }

        public void updateEntries() {
            EntryVMs.Clear();
            //filterToDos;
            filterEntries();
        }
    }
}
