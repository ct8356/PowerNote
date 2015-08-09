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
    partial class MyEntriesView : TreeView {
        DisplayPanel displayPanel;
        TaggedObject filter;
        IQueryable<Student> filteredStudents;
        public ObservableCollection<StudentVM> StudentVMs { get; set; }
        MyContext context;
        EntryPanel newEntry;
        int childLevel;

        public MyEntriesView() {
            //DO nought.
        }

        public MyEntriesView(MyContext context, DisplayPanel displayPanel) {
            this.context = context;
            this.displayPanel = displayPanel;
            StudentVMs = new ObservableCollection<StudentVM>();
            filterEntries();
            //SUBSCRIBE
            displayPanel.OptionsPanel.ShowAllEntries.Click += ShowAllEntries_Click; //subscribe
            displayPanel.OptionsPanel.ShowAllChildren.Click += ShowAllChildren_Click; //subscribe
            //INITIALIZE
            InitializeComponent();
            int count = StudentVMs.Count();
        }

        public void filterEntries() {
            filter = displayPanel.FilterPanel.Filter;
            IEnumerable<int> filterCourseIDs = filter.Courses.Select(c => c.CourseID);
            filteredStudents = context.Students;
            filteredStudents = filteredStudents
                .Where(s => !filterCourseIDs.Except(s.Courses.Select(c => c.CourseID)).Any());
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
                IQueryable<Student> filteredParents = showFirstLevel();
                //ItemsSource = StudentVMs; //Now works! So, making it new, BREAKS the bind, it seems.
                IQueryable<Student> filteredChildren = showDeeperLevels(filteredParents);
                //filteredChildren = showDeeperLevels(filteredChildren);
            }
        }

        public void insertNote() {
            //PROBLEM IS, the view is only bound to STUDENTVMs.
            //SO, need to create new STudentVM, add it to the list,
            //THEN, StudentVM itself, should be incharge of making a student,
            //and adding it to the list.
            StudentVMs.Add(new StudentVM("blank", displayPanel.MainPanel));
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
            StudentVMs.Add(newStudent);
            studentVM.Children.Add(newStudent); //CBTL GONNA CAUSE ISSUES!
            //coz when does it get added to the STUDENTS children???
            context.Students.Add(newStudent.Student);
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
                Student newStudent = new Student(newEntry.TextBox.Text);
                context.Students.Add(newStudent);
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

        public void showAllEntries() {
            context.Students.Load();
            //ItemsSource = context.Students.Local;//because binding direct to query is NOT supported.
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
                    try {
                        StudentVM viewParent = StudentVMs.Where(sVM => sVM.Student == kid.Parent).Single();
                        viewParent.Children.Add(new StudentVM(kid, displayPanel.MainPanel));
                    } catch (Exception e) {
                        //Do nothing
                    }
                }
            }
            return filteredChildren;
        }

        public IQueryable<Student> showFirstLevel() {
            IEnumerable<int> filterCourseIDs = filter.Courses.Select(c => c.CourseID);
            IQueryable<Student> filteredParents = filteredStudents
                .Where(s => filterCourseIDs.Except(s.Parent.Courses.Select(c => c.CourseID)).Any());
            foreach (Student student in filteredParents) {
                StudentVMs.Add(new StudentVM(student, displayPanel.MainPanel));
            }
            return filteredParents;
        }

        public void showUntaggedEntries() {
            IQueryable<Student> untaggedStudents =
                   filteredStudents.Where(s => !s.Courses.Any()); //Will this work?
            foreach (Student student in untaggedStudents) {
                EntryNode entryNode = new EntryNode(student, displayPanel.MainPanel);
            }
        }

        public void updateEntries() {
            StudentVMs.Clear();
            filterEntries();
        }
    }
}
