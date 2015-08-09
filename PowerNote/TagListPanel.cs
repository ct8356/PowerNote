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
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PowerNote {
    class TagListPanel : StackPanel {
        MyContext context;
        AutoCompleteBox autoCompleteBox;
        TaggedObject taggedObject;
        List<Label> labelList;
        DisplayPanel displayPanel; 

        public TagListPanel(TaggedObject taggedObject, MyContext context) {
            this.taggedObject = taggedObject;
            this.context = context;
            //PANEL
            Orientation = Orientation.Horizontal;
            //LABELS
            labelList = new List<Label>();
            addTagLabels();//courseList.Property changed += courseList_PropertyChanged;
            //AUTOCOMPLETEBOX
            autoCompleteBox = new SuggestionBox(context);
            addAutoCompleteBox();
            //SUBSCRIBE TO STUFF
            //autoCompleteBox.SelectionChanged += autoCompleteBox_SelectionChanged;
            //autoCompleteBox.LostFocus += autoCompleteBox_LostFocus;
            autoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        public void autoCompleteBox_KeyUp(object sender, KeyEventArgs e) {
            //Oddly, this is called twice when you click in the dropdown box. I guess, lets accept that, and work around it.
            //Put and if statement in there, to stop something being saved twice.
            if (e.Key == Key.Return) {
                AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
                List<String> courseStrings = new List<String>();
                List<Course> courses = new List<Course>();
                courses = context.Courses.Select(c => c).ToList<Course>();
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
                    //IF course does not exist, then do nothing
                }
            }
        }

        public void autoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e) {
            //Add new tag to Navigation property
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            Course selectedCourse = (Course)autoCompleteBox.SelectedItem;
            addCourseToStudent(selectedCourse);
            displayPanel = (DisplayPanel)((FilterPanel)Parent).Parent;
            MainPanel mainPanel = (MainPanel)displayPanel.Parent;
            mainPanel.updateEntries(); //CBTL lazy but I don't care.
        }

        public void addAutoCompleteBox() {
            Children.Add(autoCompleteBox);
        }

        public void addCourseToStudent(Course selectedCourse) {
            if (taggedObject.Courses.Contains(selectedCourse)) {
                //do nothing
            }
            else {
                taggedObject.Courses.Add(selectedCourse);
                context.SaveChanges();
                updateTagLabels(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.  
                autoCompleteBox.Text = null;
            }
        }

        public void addTagLabels() {
            foreach (Course course in taggedObject.Courses) {
                Label label = new Label();
                labelList.Add(label);
                Binding binding2 = new Binding("Title"); //This is the MODEL property it binds to.
                binding2.Source = course; // the binding source (which must fire a PROP CHANGED event).
                label.SetBinding(Label.ContentProperty, binding2);
                //RIGHT CLICKS
                MenuItem delete_menuItem = new MenuItem();
                delete_menuItem.Click += remove_menuItem_Click;
                label.ContextMenu = new ContextMenu();
                label.ContextMenu.Items.Add(delete_menuItem); //this causes invocation error.
                delete_menuItem.Header = "Remove tag";
                Children.Add(label);
            }
        }

        public void remove_menuItem_Click(Object sender, EventArgs e) {
            MenuItem menuItem = new MenuItem();
            menuItem = (MenuItem)sender;
            if (menuItem != null) {
                ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
                Label label = (Label)((Popup)contextMenu.Parent).PlacementTarget;
                String selectedCourseName = (String)label.Content;
                Course selectedCourse = null;
                //Course selectedCourse = student.Courses.Single(c => c.Title == selectedCourseName);
                foreach (Course course in taggedObject.Courses) {
                    if (course.ToString() == selectedCourseName) {
                        selectedCourse = course;
                        break;
                    }
                }
                if (selectedCourse != null)
                    taggedObject.Courses.Remove(selectedCourse);
                updateTagLabels(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
                displayPanel.updateEntries(); //CBTL lazy but I don't care.
                context.SaveChanges(); //ALSO lazy. CBTL.
            }
        }

        public void updateTagLabels() {
            foreach (Label label in labelList) {
                Children.Remove(label);
            }
            Children.Remove(autoCompleteBox);
            addTagLabels();
            addAutoCompleteBox();
        }
    }
}
