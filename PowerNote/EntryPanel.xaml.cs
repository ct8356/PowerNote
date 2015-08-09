using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PowerNote.Models;
using PowerNote.ViewModels;
using PowerNote.DAL;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PowerNote {
    public partial class EntryPanel : StackPanel {

        public EntryPanel() {
            InitializeComponent();
            //RIGHT CLICKS
            //TBH CHRIS! I BET THE TREEVIEW DEALS WITH THIS STUFF!!! LIKE LISTBOX DOES!!!
            //WELL ACTUALLY! APPARENTLY, in MVVM, it is the VIEWMODEL that deals with this stuff.
            //If gonna use XAML, then fair play, do it their way.
            TextBox.ContextMenu = new ContextMenu();
            MenuItem insertNote = new MenuItem();
            insertNote.Header = "Insert note";
            insertNote.Click += insertNote_Click;
            TextBox.ContextMenu.Items.Add(insertNote);
            MenuItem insertSubNote = new MenuItem();
            insertSubNote.Header = "Insert sub-note";
            insertSubNote.Click += insertSubNote_Click;
            TextBox.ContextMenu.Items.Add(insertSubNote);
            MenuItem deleteEntry = new MenuItem();
            deleteEntry.Header = "Delete note";
            deleteEntry.Click += deleteEntry_Click;
            TextBox.ContextMenu.Items.Add(deleteEntry); //this causes invocation error.
            //Easier to do this in XAML???
            //AUTOCOMPLETEBOX
            //autoCompleteBox.SelectionChanged += autoCompleteBox_SelectionChanged;
            //autoCompleteBox.LostFocus += autoCompleteBox_LostFocus;
            MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        public void deleteEntry_Click(object sender, RoutedEventArgs e) {
            (DataContext as StudentVM).Context.Students.Remove((DataContext as StudentVM).Student);
            (DataContext as StudentVM).Context.SaveChanges(); //ALSO lazy. CBTL.
            //SO NOTE: OF COURSE, easier you just do these things, IN THE VIEWMODEL!
            (DataContext as StudentVM).MainPanel.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
        }

        public void autoCompleteBox_KeyUp(object sender, KeyEventArgs e) {
            //Oddly, this is called twice when you click in the dropdown box. I guess, lets accept that, and work around it.
            //Put an if statement in there, to stop something being saved twice.
            if (e.Key == Key.Return) {
                AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
                List<String> courseStrings = new List<String>();
                List<Course> courses = new List<Course>();
                courses = (DataContext as StudentVM).Context.Courses.Select(c => c).ToList<Course>();
                foreach (Course course in courses) {
                    courseStrings.Add(course.ToString());
                }
                if (courseStrings.Contains(autoCompleteBox.Text)) {
                    //i.e. IF course exists already, then say "selection Changed!".
                    //This method no longer needed, but keep it, just in case.
                    autoCompleteBox_SelectionChanged(sender, e);
                    //OR, just do nothing, since called anyway, when selection changed.
                    //WHICH is called first by the way? LostFocus, or SelectionCHanged? lostFocus first. SC, SC. LF.
                }
                else {
                    //IF no, then create new entry.
                    if (autoCompleteBox.Text != null && autoCompleteBox.Text != "") {
                        Course newCourse = new Course();
                        newCourse.Title = autoCompleteBox.Text;
                        (DataContext as StudentVM).Context.Courses.Add(newCourse);
                        (DataContext as StudentVM).Context.SaveChanges();
                        addCourseToStudent(newCourse);
                    }
                }
            }
        }

        public void autoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e) {
            //Add new tag to Navigation property
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            Course selectedCourse = (Course)autoCompleteBox.SelectedItem;
            addCourseToStudent(selectedCourse);
        }

        public void addCourseToStudent(Course selectedCourse) {
            if ((DataContext as StudentVM).Courses.Contains(selectedCourse)) {
                //do nothing
            }
            else {
                (DataContext as StudentVM).Courses.Add(selectedCourse);
                (DataContext as StudentVM).Context.SaveChanges();
                (DataContext as StudentVM).MainPanel.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.  
                MyAutoCompleteBox.Text = null;
            }
        }

        public void addTagLabels() {
            IEnumerable<int> filterCourseIDs = (DataContext as StudentVM).MainPanel.DisplayPanel
                .FilterPanel.Filter.Courses.Select(c => c.CourseID);
            var alphabeticalCourses = (DataContext as StudentVM)
                .Courses.Where(c => !filterCourseIDs.Contains(c.CourseID)).OrderBy(c => c.Title);
        } //WHOLE THING needs to be rewritten, to suit LISTBOX.

        public void courseList_PropertyChanged(Object sender, EventArgs e) {
            updateTagLabels();
        }

        public void delete_Click(Object sender, EventArgs e) {
            MenuItem menuItem = new MenuItem();
            menuItem = (MenuItem)sender;
            if (menuItem != null) {
                ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
                Label label = (Label)((Popup)contextMenu.Parent).PlacementTarget;
                String selectedCourseName = (String)label.Content;
                Course selectedCourse = null;
                foreach (Course course in (DataContext as StudentVM).Courses) {
                    if (course.ToString() == selectedCourseName) {
                        selectedCourse = course;
                        break;
                    }
                }
                if (selectedCourse != null)
                    (DataContext as StudentVM).Courses.Remove(selectedCourse);
                updateTagLabels(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
                (DataContext as StudentVM).Context.SaveChanges(); //ALSO lazy. CBTL.
            }
        }

        public void insertNote_Click(Object sender, EventArgs e) {
            MenuItem menuItem = new MenuItem();
            menuItem = (MenuItem)sender;
            if (menuItem != null) {
                (DataContext as StudentVM).insertNote();
                //NOTE: not ok to just create student, HAVE TO ADD IT to CONTEXT!
                //NOTE: at best, STUDENTVM can know about the CONTEXT.
                //NOT this entryPanel.
                //BUT TO BE HONEST: it makes more sense to get TREEVIEWMODEL to do it.
                //NOTE THOUGH; want it to be aware of the filter...
                //I'LL BE HONEST: I think either would work.
                //BUT I THINK it makes more sense to do in TREEVIEWMODEL! 
                //since MODIFYING the structure of the TREE!
                //BUT SINCE already started here, I will continue here.
            }
        }

        public void insertSubNote_Click(Object sender, EventArgs e) {
            MenuItem menuItem = new MenuItem();
            menuItem = (MenuItem)sender;
            if (menuItem != null) {
                (DataContext as StudentVM).insertSubNote(DataContext as StudentVM);
            }
        }

        public void updateTagLabels() {
            addTagLabels();
        }

    }
}
