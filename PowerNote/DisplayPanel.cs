using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.Migrations;
using System.Windows.Data;
using System.Windows.Input;

namespace PowerNote {

    class DisplayPanel : DockPanel {
            Label title;
            MyContext context;
            public FilterPanel FilterPanel { get; set; }
            TaggedObject filter;
            IQueryable<Student> filteredStudents;
            public SortPanel SortPanel { get; set; }
            public OptionsPanel OptionsPanel { get; set; }
            public List<String> ColumnNames { get; set; }
            List<Entry> entryList;
            Entry newEntry;
            MainPanel mainPanel;
            int childLevel;

            public DisplayPanel(MyContext context, MainPanel mainPanel) {
                this.context = context;
                this.mainPanel = mainPanel;
                //PANEL
                title = new Label();
                title.Content = "Display panel";
                title.FontWeight = FontWeights.Bold;
                Children.Add(title);
                SetDock(title, Dock.Top);
                //FILTER PANEL
                FilterPanel = new FilterPanel(context);
                Children.Add(FilterPanel);
                SetDock(FilterPanel, Dock.Top);
                //SORT PANEL
                SortPanel = new SortPanel(context);
                Children.Add(SortPanel);
                SetDock(SortPanel, Dock.Top);
                SortPanel.ComboBox.SelectionChanged += ComboBox_SelectionChanged; //subscribe
                //OPTIONS PANEL
                OptionsPanel = new OptionsPanel(context);
                Children.Add(OptionsPanel);
                SetDock(OptionsPanel, Dock.Top);
                OptionsPanel.ShowAllEntries.Click += ShowAllEntries_Click; //subscribe
                OptionsPanel.ShowAllChildren.Click += ShowAllChildren_Click; //subscribe
                //COLUMN NAME PANEL
                //var colNames = typeof(Student).GetProperties().Select(a => a.Name).ToList();
                ColumnNames = new List<String>() { "Contents", "Priority" };
                ColumnNamePanel columnNamePanel = new ColumnNamePanel(this);
                Children.Add(columnNamePanel);
                SetDock(columnNamePanel, Dock.Top);
                //ADD ENTRIES
                entryList = new List<Entry>();
                addEntries();
                //OTHER
                //label2 = new Label();
                //Binding binding = new Binding("FirstMidName"); //This is the MODEL property it binds to.
                //binding.Source = context.Students.ToList()[0]; // the binding source (which must fire a PROP CHANGED event).
                //label2.SetBinding(Label.ContentProperty, binding);
                //Children.Add(label2);
                LastChildFill = false;
            }

            public void addEntries() {
                filter = FilterPanel.Filter;
                filteredStudents = context.Students;
                IEnumerable<int> filterCourseIDs = filter.Courses.Select(c => c.CourseID);
                filteredStudents = filteredStudents
                    .Where(s => !filterCourseIDs.Except(s.Courses.Select(c => c.CourseID)).Any());
                //filter.Courses.SelectMany(c => c.Students); //Does not work yet, but look into.
                switch (SortPanel.ComboBox.SelectedItem.ToString()) {
                    case "ID": break;
                    case "Priority": filteredStudents = filteredStudents.OrderBy(s => s.Priority); break;
                }
                if (OptionsPanel.ShowAllEntriesBool) {
                    showAllEntries();
                }
                else {
                    IQueryable<Student> filteredParents = showFirstLevel();
                    IQueryable<Student> filteredChildren = showDeeperLevels(filteredParents);
                    filteredChildren = showDeeperLevels(filteredChildren);
                }
                    //AHAH! So either way, will have a foreach loop for each child layer.
                    //BUT if do a query for each layer, 
                    //(rather than just one, and then placing them all in right place,
                    //BUT having to check first, whether was a parent, or whatever) 
                    //(NOTE above would have worked fine, probs, but now, I think what I got is bit quicker)
                    //THEN can just have 3 foreach loops,
                    //one after the other,
                    //rather than nested. WHICH I THINK will be quicker, right???
                addNewEntry();           
            }

            public void addNewEntry() {
                newEntry = new Entry();
                SetDock(newEntry, Dock.Top);
                entryList.Add(newEntry);
                Children.Add(newEntry);
                newEntry.LostFocus += new RoutedEventHandler(newEntry_LostFocus);
                newEntry.KeyUp += new KeyEventHandler(newEntry_KeyUp);
            }

            public void ComboBox_SelectionChanged(object sender, RoutedEventArgs e) {
                //START a ORDER BY QUERY. I say, just call to UPDATE methods, BUT with NEW INPUT.
                updateEntries();
            }

            public void newEntry_KeyUp(object sender, KeyEventArgs e) {
                if (e.Key == Key.Return) {
                    newEntry_LostFocus(sender, e);
                }
            }

            public void newEntry_LostFocus(object sender, RoutedEventArgs e) {
                if (newEntry.textBox.Text != null && newEntry.textBox.Text != "") {
                    Student newStudent = new Student(newEntry.textBox.Text);
                    context.Students.Add(newStudent);
                    context.SaveChanges();
                    updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.   
                }
            }

            public void ShowAllChildren_Click(object sender, EventArgs e) {
                updateEntries();
            }

            public void ShowAllEntries_Click(object sender, EventArgs e) {
                updateEntries();
            }

            public void showAllEntries() {
                foreach (Student student in filteredStudents) {
                    Entry entry = new Entry(student, context, this, mainPanel);
                    SetDock(entry, Dock.Top);
                    entryList.Add(entry);
                    Children.Add(entry);
                }
            }

            public IQueryable<Student> showDeeperLevels(IQueryable<Student> filteredParents) {
                childLevel++;
                IQueryable<Student> filteredChildren = null;
                IEnumerable<int> parentStudentIDs = filteredParents.Select(s => s.StudentID);
                if (OptionsPanel.ShowAllChildrenBool) {
                    filteredChildren = 
                        context.Students.Where(s => parentStudentIDs.Contains(s.Parent.StudentID));
                } else {
                    filteredChildren =
                        filteredStudents.Where(s => parentStudentIDs.Contains(s.Parent.StudentID));
                } //NOTE! innefficiency here, in that it does this, for parents too...
                foreach (Student student in filteredChildren) {
                    if (student.Parent != null) {
                        List<Student> parentList = filteredParents.ToList<Student>();
                        int parentIndex = parentList.FindIndex(p => p == student.Parent);
                        Entry entry = new Entry(student, context, this, mainPanel);
                        SetDock(entry, Dock.Top);
                        entryList.Add(entry);
                        for (int i = 0; i < childLevel; i++) {
                            entry.Children.Insert(0, new Label() { Content = "    " });
                        }
                        Children.Insert(parentIndex, entry);
                    }
                } //OK! but will only put all children at end... try it.
                return filteredChildren;
            }

            public IQueryable<Student> showFirstLevel() {
                childLevel = 0;
                IEnumerable<int> filterCourseIDs = filter.Courses.Select(c => c.CourseID);
                IQueryable<Student> filteredParents = filteredStudents
                    .Where(s => filterCourseIDs.Except(s.Parent.Courses.Select(c => c.CourseID)).Any());
                foreach (Student student in filteredParents) {
                    Entry entry = new Entry(student, context, this, mainPanel);
                    SetDock(entry, Dock.Top);
                    entryList.Add(entry);
                    Children.Add(entry);
                }
                return filteredParents;
            }

            public void showUntaggedEntries() {
                IQueryable<Student> untaggedStudents =
                       filteredStudents.Where(s => !s.Courses.Any()); //Will this work?
                foreach (Student student in untaggedStudents) {
                    Entry entry = new Entry(student, context, this, mainPanel);
                    SetDock(entry, Dock.Top);
                    entryList.Add(entry);
                    Children.Add(entry);
                }
            }

            public void updateEntries() {
                foreach (Entry entry in entryList) {
                    Children.Remove(entry);
                }
                addEntries();
            }

    }
}
