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
using PowerNote.Migrations;
using System.Windows.Data;
using System.Windows.Input;

namespace PowerNote {
    /// <summary>
    /// Interaction logic for MyEntriesView.xaml
    /// </summary>
    partial class MyEntriesView : UserControl {

        DisplayPanel displayPanel;
        TaggedObject filter;
        IQueryable<Student> filteredStudents;
        List<Student> viewStudents;
        MyContext context;
        EntryPanel newEntry;
        int childLevel;

        public MyEntriesView() {
            InitializeComponent();
        }

        MyEntriesView(MyContext context, DisplayPanel displayPanel) {
            this.context = context;
            this.displayPanel = displayPanel;
            viewStudents = new List<Student>();
            //ADD ENTRIES
            //NOTE: if properly bound, should not even need to SHOW entries!
            //MAYBE: should try binding with a list first? Might shine some light...
            addEntries();
            //BINDING
            //LABEL FACTORY
            FrameworkElementFactory elementFactory = new FrameworkElementFactory(typeof(EntryPanel));
            //NOTE: this is deprecated!
            //elementFactory.SetBinding(EntryPanel.StudentProperty, new Binding("this"));
            elementFactory.SetBinding(TextBox.TextProperty, new Binding("Contents"));
            //shit, maybe just have to set a load of bindings. MAYBE. would need load of depProperties too.
            //OK OK, maybe, the itemSource for TREEVIEW, should NOT be, the model...
            //TEMPLATE
            HierarchicalDataTemplate template = new HierarchicalDataTemplate(typeof(Student));
            template.ItemsSource = new Binding("Children");
            //template.VisualTree = SURE I have to update this one.
            //ItemTemplate = template;
            //SUBSCRIBE
            displayPanel.OptionsPanel.ShowAllEntries.Click += ShowAllEntries_Click; //subscribe
            displayPanel.OptionsPanel.ShowAllChildren.Click += ShowAllChildren_Click; //subscribe
        }

        public void addEntries() {
            filter = displayPanel.FilterPanel.Filter;
            IEnumerable<int> filterCourseIDs = filter.Courses.Select(c => c.CourseID);
            filteredStudents = context.Students;
            filteredStudents = filteredStudents
                .Where(s => !filterCourseIDs.Except(s.Courses.Select(c => c.CourseID)).Any());
            //filter.Courses.SelectMany(c => c.Students); //Does not work yet, but look into.
            //ORDERBY
            switch (displayPanel.SortPanel.ComboBox.SelectedItem.ToString()) {
                case "ID": break;
                case "Priority": filteredStudents = filteredStudents.OrderBy(s => s.Priority); break;
            }
            if (displayPanel.OptionsPanel.ShowAllEntriesBool) {
                showAllEntries();
            }
            else {
                 //AH YES, because binding direct to query is NOT supported.
                //OK! THIS will always show ALL children.
                //if only want to show filtered children, I imagine need to do 3 forloops thing again?
                //OR not even that... just a line, for each LEVEL, setting the ITEMSSOURCE to right thing?
                //well, no. not the itemSource.
                IQueryable<Student> filteredParents = showFirstLevel();
                IQueryable<Student> filteredChildren = showDeeperLevels(filteredParents);
                filteredChildren = showDeeperLevels(filteredChildren);
                //ItemsSource = viewStudents;
            }
            //AHAH! So either way, will have a foreach loop for each child layer.
            //BUT if do a query for each layer, 
            //(rather than just one, and then placing them all in right place,
            //BUT having to check first, whether was a parent, or whatever) 
            //(NOTE above would have worked fine, probs, but now, I think what I got is bit quicker)
            //THEN can just have 3 foreach loops,
            //one after the other,
            //rather than nested. WHICH I THINK will be quicker, right???
            //addNewEntry();
        }

        public void addNewEntry() {
            newEntry = new EntryPanel();
            //SetDock(newEntry, Dock.Top);
            //Items.Add(newEntry);
            newEntry.LostFocus += new RoutedEventHandler(newEntry_LostFocus);
            newEntry.KeyUp += new KeyEventHandler(newEntry_KeyUp);
        }

        public void newEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                newEntry_LostFocus(sender, e);
            }
        }

        public void newEntry_LostFocus(object sender, RoutedEventArgs e) {
            if (newEntry.TextBox.Text != null && newEntry.TextBox.Text != "") {
                Student newStudent = new Student(newEntry.TextBox.Text);
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
            context.Students.Load();
            //ItemsSource = context.Students.Local;//AH YES, because binding direct to querty is NOT supported.
        }

        public IQueryable<Student> showDeeperLevels(IQueryable<Student> filteredParents) {
            childLevel++;
            IQueryable<Student> filteredChildren = null;
            IEnumerable<int> parentStudentIDs = filteredParents.Select(s => s.StudentID);
            if (displayPanel.OptionsPanel.ShowAllChildrenBool) {
                filteredChildren =
                    context.Students.Where(s => parentStudentIDs.Contains(s.Parent.StudentID));
            }
            else {
                filteredChildren =
                    filteredStudents.Where(s => parentStudentIDs.Contains(s.Parent.StudentID));
            } //NOTE! innefficiency here, in that it does this, for parents too...
            List<Student> filteredKids = filteredChildren.ToList();
            //for (int countDown = filteredKids.Count()-1; countDown >= 0; countDown--) {
            foreach (Student kid in filteredKids) {
                if (kid.Parent != null) { //if kid has a parent
                    Student viewParent = viewStudents.Find(s => s == kid.Parent);
                    viewParent.Children.Add(kid);
                    viewStudents.Add(kid);
                    //List<Student> parentNodeIndex = Items as List<Student>; //first level items.
                    //EntryNode parentNode
                    //EntryNode entryNode = new EntryNode(kid, displayPanel.MainPanel);
                    //parentNode.Items.Add(entryNode);
                }
            }
            return filteredChildren;
        }

        public IQueryable<Student> showFirstLevel() {
            childLevel = 0;
            IEnumerable<int> filterCourseIDs = filter.Courses.Select(c => c.CourseID);
            IQueryable<Student> filteredParents = filteredStudents
                .Where(s => filterCourseIDs.Except(s.Parent.Courses.Select(c => c.CourseID)).Any());
            foreach (Student student in filteredParents) {
                viewStudents.Add(student);
                EntryNode entryNode = new EntryNode(student, displayPanel.MainPanel);
                //Items.Add(entryNode);
            }
            return filteredParents;
        }

        public void showUntaggedEntries() {
            IQueryable<Student> untaggedStudents =
                   filteredStudents.Where(s => !s.Courses.Any()); //Will this work?
            foreach (Student student in untaggedStudents) {
                EntryNode entryNode = new EntryNode(student, displayPanel.MainPanel);
                //Items.Add(entryNode);
            }
        }

        public void updateEntries() {
            //viewStudents.Clear();
            addEntries();
        }
    }
}
