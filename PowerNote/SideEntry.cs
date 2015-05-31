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
using PowerNote.DAL;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PowerNote {
    class SideEntry : StackPanel {
        MyContext context;
        Student Student { get; set; }
        List<Course> courseList;
        List<Label> labelList;
        SuggestionBox autoCompleteBox;
        CheckBox checkBox { get; set; }
        public TextBox textBox { get; set; }
        DisplayPanel displayPanel;
        SideNotePanel sideNotePanel;

        public SideEntry() {
            Orientation = Orientation.Horizontal;
            //CHECK BOX
            checkBox = new CheckBox();
            Children.Add(checkBox);
            //TEXT BOX
            textBox = new TextBox();
            textBox.Width = 100;
            Children.Add(textBox);
        }

        public SideEntry(MyContext context, SideNotePanel sideNotePanel) : this() {
            this.context = context;
            this.sideNotePanel = sideNotePanel;
            //BASICALLY, got to make it CREATE it, 
            //THEN Bind it... ONCE IT HAS BEEN FILLED.
            //THEN CLEAR when button pressed.
            //TAG LABELS
            courseList = new List<Course>();
            labelList = new List<Label>();
            //addTagLabels();//courseList.Property changed += courseList_PropertyChanged;
            //AUTOCOMPLETEBOX
            autoCompleteBox = new SuggestionBox(context);
            addAutoCompleteBox();
            //SUBSCRIBE TO STUFF
            autoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        public void bindTextBox(Student student) {
            this.Student = student;
            Binding binding = new Binding("Contents"); //This is the MODEL property it binds to.
            binding.Source = Student; // the binding source (which must fire a PROP CHANGED event).
            textBox.SetBinding(TextBox.TextProperty, binding); //fortunately, textBox already fires an event when changed.
            //YOU created the event for the dataSource. SO HOPEFULLY, we have 2 way binding now... we do :)
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
                    //IF no, then create new entry.
                    if (autoCompleteBox.Text != null && autoCompleteBox.Text != "") {
                        Course newCourse = new Course();
                        newCourse.Title = autoCompleteBox.Text;
                        context.Courses.Add(newCourse);
                        addCourseToStudent(newCourse);
                    }
                }
            }
        }

        public void autoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e) {
            //Add new tag to Navigation property
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            Course selectedCourse = (Course) autoCompleteBox.SelectedItem;
            addCourseToStudent(selectedCourse);          
        }

        public void addCourseToStudent(Course selectedCourse) {
            if (Student.Courses.Contains(selectedCourse)) {
                //do nothing
            }
            else {
                Student.Courses.Add(selectedCourse);
                context.SaveChanges();
                updateTagLabels(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.  
                autoCompleteBox.Text = null;
            }
        }

        public void addAutoCompleteBox() {
            Children.Add(autoCompleteBox);
        }

        public void addTagLabels() {
            foreach (Course course in Student.Courses) {
                Label label = new Label();
                labelList.Add(label);
                Binding binding2 = new Binding("Title"); //This is the MODEL property it binds to.
                binding2.Source = course; // the binding source (which must fire a PROP CHANGED event).
                label.SetBinding(Label.ContentProperty, binding2);
                //RIGHT CLICKS
                MenuItem delete_menuItem = new MenuItem();
                delete_menuItem.Click += delete_menuItem_Click;
                label.ContextMenu = new ContextMenu();
                label.ContextMenu.Items.Add(delete_menuItem); //this causes invocation error.
                delete_menuItem.Header = "Remove tag";       
                Children.Add(label);
            }
        }

        public void courseList_PropertyChanged(Object sender, EventArgs e) {
            updateTagLabels();
        }

        public void delete_menuItem_Click(Object sender, EventArgs e) {
            MenuItem menuItem = new MenuItem();
            menuItem = (MenuItem)sender;
            if (menuItem != null) {
                ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
                Label label = (Label)((Popup)contextMenu.Parent).PlacementTarget;
                String selectedCourseName = (String) label.Content;
                Course selectedCourse = null;
                //Course selectedCourse = student.Courses.Single(c => c.Title == selectedCourseName);
                foreach (Course course in Student.Courses) {
                    if (course.ToString() == selectedCourseName) {
                        selectedCourse = course;
                        break;
                    }
                }
                if (selectedCourse != null)
                    Student.Courses.Remove(selectedCourse);
                updateTagLabels(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
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
