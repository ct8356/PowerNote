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
    public partial class ToDoPanel : StackPanel {

        public ToDoPanel() {
            InitializeComponent();
            //RIGHT CLICKS
            //Could do this in treeView. Does not really matter.
            //PROBS doing it in treeView means less instances...
            TextBox.ContextMenu = new ContextMenu();
            MenuItem insertNote = new MenuItem();
            insertNote.Header = "Insert entry";
            insertNote.Click += insertNote_Click;
            TextBox.ContextMenu.Items.Add(insertNote);
            MenuItem insertSubNote = new MenuItem();
            insertSubNote.Header = "Insert sub-entry";
            insertSubNote.Click += insertSubNote_Click;
            TextBox.ContextMenu.Items.Add(insertSubNote);
            MenuItem deleteEntry = new MenuItem();
            deleteEntry.Header = "Delete entry";
            deleteEntry.Click += deleteEntry_Click;
            TextBox.ContextMenu.Items.Add(deleteEntry); //this causes invocation error.
            //Easier to do this in XAML???
            //AUTOCOMPLETEBOX
            //autoCompleteBox.SelectionChanged += autoCompleteBox_SelectionChanged;
            //autoCompleteBox.LostFocus += autoCompleteBox_LostFocus;
            MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        public void deleteEntry_Click(object sender, RoutedEventArgs e) {
            (DataContext as StudentVM).Context.ToDos.Remove((DataContext as StudentVM).Student);
            (DataContext as EntryVM).Context.SaveChanges(); //ALSO lazy. CBTL.
            //SO NOTE: OF COURSE, easier you just do these things, IN THE VIEWMODEL!
            (DataContext as EntryVM).MainPanel.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
        }

        public void autoCompleteBox_KeyUp(object sender, KeyEventArgs e) {
            //Oddly, this is called twice when you click in the dropdown box. I guess, lets accept that, and work around it.
            //Put an if statement in there, to stop something being saved twice.
            if (e.Key == Key.Return) {
                AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
                List<String> courseStrings = new List<String>();
                List<Tag> courses = new List<Tag>();
                courses = (DataContext as StudentVM).Context.Tags.Select(c => c).ToList<Tag>();
                foreach (Tag course in courses) {
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
                        Tag newCourse = new Tag();
                        newCourse.Title = autoCompleteBox.Text;
                        (DataContext as StudentVM).Context.Tags.Add(newCourse);
                        (DataContext as StudentVM).Context.SaveChanges();
                        addTagToEntry(newCourse);
                    }
                }
            }
        }

        public void autoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e) {
            //Add new tag to Navigation property
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            Tag selectedCourse = (Tag)autoCompleteBox.SelectedItem;
            addTagToEntry(selectedCourse);
        }

        public void addTagToEntry(Tag selectedCourse) {
            if ((DataContext as EntryVM).Tags.Contains(selectedCourse)) {
                //do nothing
            }
            else {
                (DataContext as EntryVM).Tags.Add(selectedCourse);
                (DataContext as EntryVM).Context.SaveChanges();
                (DataContext as EntryVM).MainPanel.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.  
                MyAutoCompleteBox.Text = null;
            }
        }

        public void filterAndSortTagsShown() {
            IEnumerable<int> filterCourseIDs = (DataContext as EntryVM).MainPanel.DisplayPanel
                .FilterPanel.Filter.Tags.Select(c => c.TagID);
            var alphabeticalCourses = (DataContext as EntryVM)
                .Tags.Where(c => !filterCourseIDs.Contains(c.TagID)).OrderBy(c => c.Title);
        }

        public void courseList_PropertyChanged(Object sender, EventArgs e) {
            updateTagLabels();
        }

        //public void delete_Click(Object sender, EventArgs e) {
        //    MenuItem menuItem = new MenuItem();
        //    menuItem = (MenuItem)sender;
        //    if (menuItem != null) {
        //        ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
        //        Label label = (Label)((Popup)contextMenu.Parent).PlacementTarget;
        //        String selectedCourseName = (String)label.Content;
        //        Tag selectedCourse = null;
        //        foreach (Tag course in (DataContext as StudentVM).Tags) {
        //            if (course.ToString() == selectedCourseName) {
        //                selectedCourse = course;
        //                break;
        //            }
        //        }
        //        if (selectedCourse != null)
        //            (DataContext as StudentVM).Tags.Remove(selectedCourse);
        //        updateTagLabels(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
        //        (DataContext as StudentVM).Context.SaveChanges(); //ALSO lazy. CBTL.
        //    }
        //}

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
                (DataContext as StudentVM).insertChild(DataContext as StudentVM);
            }
        }

        public void updateTagLabels() {
            filterAndSortTagsShown();
        }

    }
}
