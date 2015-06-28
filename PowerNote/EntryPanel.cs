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

    class EntryPanel : StackPanel { //Put this in the header.
     
        MyContext context;
        List<Course> courseList;
        List<Label> labelList;
        SuggestionBox autoCompleteBox;
        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.Register("TextBox", typeof(TextBox), typeof(EntryPanel));
        public TextBox TextBox {
            get { return GetValue(TextBoxProperty) as TextBox; }
            set { SetValue(TextBoxProperty, value); }
        }
        public TextBox Priority { get; set; }
        DisplayPanel displayPanel;
        TaggedObject filter;
        MainPanel mainPanel;
        public static readonly DependencyProperty StudentProperty =
            DependencyProperty.Register("Student", typeof(Student), typeof(EntryPanel));
        Student Student {
            get { return GetValue(StudentProperty) as Student; }
            set { SetValue(StudentProperty, value); }
        }

        public EntryPanel() {
            Orientation = Orientation.Horizontal;
            //TEXT BOX
            TextBox = new TextBox();
            TextBox.Width = 100;
            Children.Add(TextBox);
            //PRIORITY TEXT BOX
            Priority = new TextBox();
            Priority.Width = 100;
            Children.Add(Priority);
        }

        public EntryPanel(Student student, MainPanel mainPanel) : this() {
            this.Student = student;
            this.context = mainPanel.Context;
            this.displayPanel = mainPanel.DisplayPanel;
            this.filter = displayPanel.FilterPanel.Filter;
            this.mainPanel = mainPanel;
            //TEXTBOX BINDING
            Binding binding = new Binding(displayPanel.ColumnNames[0]); //This is the MODEL property it binds to.
            binding.Source = student; // the binding source (which must fire a PROP CHANGED event).
            TextBox.SetBinding(TextBox.TextProperty, binding); //fortunately, textBox already fires an event when changed.
            //YOU created the event for the dataSource. SO HOPEFULLY, we have 2 way binding now... we do :)
            //PRIORITY TEXTBOX BINDING
            Binding binding2 = new Binding(displayPanel.ColumnNames[1]);
            binding2.Source = student;
            Priority.SetBinding(TextBox.TextProperty, binding2);
            //RIGHT! SO we need to stop binding individually (although, that is fine actually).
            //NOW! we just need to bind the STRUCTURE. i.e. the TEMPLATE!
            //RIGHT CLICKS
            TextBox.ContextMenu = new ContextMenu();
            MenuItem deleteEntry = new MenuItem();
            deleteEntry.Header = "Delete entry";
            deleteEntry.Click += deleteEntry_Click;
            TextBox.ContextMenu.Items.Add(deleteEntry); //this causes invocation error.
            MenuItem insertSubNote = new MenuItem();
            insertSubNote.Header = "Insert sub-note";
            insertSubNote.Click += insertSubNote_Click;
            TextBox.ContextMenu.Items.Add(insertSubNote);
            //TAG LABELS
            courseList = new List<Course>();
            labelList = new List<Label>();
            addTagLabels();//courseList.Property changed += courseList_PropertyChanged;
            //AUTOCOMPLETEBOX
            autoCompleteBox = new SuggestionBox(context);
            addAutoCompleteBox();
            //autoCompleteBox.SelectionChanged += autoCompleteBox_SelectionChanged;
            //autoCompleteBox.LostFocus += autoCompleteBox_LostFocus;
            autoCompleteBox.KeyUp += autoCompleteBox_KeyUp;        
        }

        public void deleteEntry_Click(object sender, RoutedEventArgs e) {
            context.Students.Remove(Student);
            context.SaveChanges(); //ALSO lazy. CBTL.
            mainPanel.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
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
                        context.SaveChanges();
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
                mainPanel.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.  
                autoCompleteBox.Text = null;
            }
        }

        public void addAutoCompleteBox() {
            Children.Add(autoCompleteBox);
        }

        public void addTagLabels() {
            IEnumerable<int> filterCourseIDs = filter.Courses.Select(c => c.CourseID);
            var alphabeticalCourses = Student.Courses.Where(c => !filterCourseIDs.Contains(c.CourseID)).OrderBy(c => c.Title);
            foreach (Course course in alphabeticalCourses) {
                Label label = new Label();
                labelList.Add(label);
                Children.Add(label);
                //Binding
                Binding binding2 = new Binding("Title"); //This is the MODEL property it binds to.
                binding2.Source = course; // the binding source (which must fire a PROP CHANGED event).
                label.SetBinding(Label.ContentProperty, binding2);       
                //RIGHT CLICKS
                label.ContextMenu = new ContextMenu();
                MenuItem delete = new MenuItem();
                delete.Header = "Remove tag";       
                label.ContextMenu.Items.Add(delete); //this causes invocation error.
                delete.Click += delete_Click;        
            }
        }

        public void courseList_PropertyChanged(Object sender, EventArgs e) {
            updateTagLabels();
        }

        public void delete_Click(Object sender, EventArgs e) {
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

        public void insertSubNote_Click(Object sender, EventArgs e) {
            MenuItem menuItem = new MenuItem();
            menuItem = (MenuItem)sender;
            if (menuItem != null) {
                Student newStudent = new Student(Student.Contents + " child");
                Student.Children.Add(newStudent);
                context.Students.Add(newStudent);
                context.SaveChanges();
                mainPanel.updateEntries();
                context.SaveChanges();
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
