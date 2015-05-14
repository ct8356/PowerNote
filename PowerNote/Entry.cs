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

namespace PowerNote {

    class Entry : StackPanel {
        DAL.SchoolContext schoolContext;
        CheckBox checkBox { get; set; }
        public TextBox textBox {get; set;}
        Student student;

        public Entry(Student student, DAL.SchoolContext schoolContext) {
            this.student = student;
            this.schoolContext = schoolContext;
            Orientation = Orientation.Horizontal;
            checkBox = new CheckBox();
            //TEXT BOX
            textBox = new TextBox();
            //textBox.Text = "do some shopping";
            Binding binding = new Binding("FirstMidName"); //This is the MODEL property it binds to.
            binding.Source = student; // the binding source (which must fire a PROP CHANGED event).
            textBox.SetBinding(TextBox.TextProperty, binding);
            //SO, now the binding is in place. fortunately, textBox already fires an event when changed.
            //YOU created the event for the dataSource. SO HOPEFULLY, we have 2 way binding now...
            Children.Add(checkBox);
            Children.Add(textBox);
            //TAG LABELS
            //IQueryable<ICollection<Enrollment>> enrollments = schoolContext.Students.Select(s => s.Enrollments);
            foreach (Enrollment enrollment in student.Enrollments) {
                Label label = new Label();
                Binding binding2 = new Binding("Title"); //This is the MODEL property it binds to.
                binding2.Source = enrollment.Course; // the binding source (which must fire a PROP CHANGED event).
                label.SetBinding(Label.ContentProperty, binding2);
                Children.Add(label);
            }
            //AUTOCOMPLETE BOX
            AutoCompleteBox autoCompleteBox = new AutoCompleteBox();
            //autoCompleteBox.ItemsSource = new List<String> {"qwer","asdf", "zxcv"};//this works...
            autoCompleteBox.ItemsSource = schoolContext.Courses.Select(c => c);
            //WOULD be good, to set this to Courses, rather than just strings. CBTL.
            //Now, I THINK itemsSource, does a binding, as long as you fire a PropChanged event from Courses class. Done.
            //NOW I need to research, how to NICELY add something to the ENROLLMENT TABLE. See Contoso tutorial.
            //autoCompleteBox.SelectedItem = schoolContext.Courses.ToList()[0].Title; //This does not matter really.
            //Binding binding2 = new Binding("Title"); //This is the MODEL property it binds to.
            //binding2.Source = course; // the binding source (which must fire a PROP CHANGED event).
            //autoCompleteBox.SetBinding(AutoCompleteBox., binding2);
            Children.Add(autoCompleteBox);
            //Now, subscribe to the Autoboxes "enterPressed" event.
            //and call a method here called: "add to navigation property".
            autoCompleteBox.SelectionChanged += addToNavigationProperty;
        }

        public void addToNavigationProperty(object sender, RoutedEventArgs e) {
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            Course selectedCourse = (Course) autoCompleteBox.SelectedItem;
            Enrollment enrollment = new Enrollment();
            enrollment.Course = selectedCourse;
            student.Enrollments.Add(enrollment);
        }
    }
}
