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
            Label label2;
            MyContext context;
            public FilterPanel FilterPanel {get; set;}
            ICollection<Student> filteredStudents;
            List<Entry> entryList;
            Entry newEntryEntry;

            public DisplayPanel(MyContext context) {
                this.context = context;
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
                TaggedObject filter = FilterPanel.Filter;
                IQueryable<Student> filteredStudents = context.Students;
                foreach (Course course in filter.Courses) {
                    if (course != null) {
                        Course tempCourse = course;
                        filteredStudents = filteredStudents.Where(s => s.Courses.Select(c => c.CourseID).Contains(tempCourse.CourseID)); 
                        //Can only use LINQ Contains(), with PRIMITIVE values! (i.e. strings and ints etc!)
                    }
                }
                List<Student> studentList = filteredStudents.ToList<Student>();
                foreach (Student student in studentList) { //AHAH, so query not fired until THIS BIT! i.e. we actually ACCESS the nav property, filteredStudents!
                    Entry entry = new Entry(student, context, this);
                    //Entry entry = new Entry(context.Students.ToList()[i], context, FilterPanel.Filter);
                    SetDock(entry, Dock.Top);
                    entryList.Add(entry);
                    Children.Add(entry);
                }
                //NEW ENTRY ENTRY
                newEntryEntry = new Entry();  
                SetDock(newEntryEntry, Dock.Top);
                entryList.Add(newEntryEntry);
                Children.Add(newEntryEntry);
                newEntryEntry.LostFocus += new RoutedEventHandler(newEntryEntry_LostFocus);
                newEntryEntry.KeyUp += new KeyEventHandler(newEntryEntry_KeyUp);
            }

            public void newEntryEntry_KeyUp(object sender, KeyEventArgs e) {
                if (e.Key == Key.Return) {
                    newEntryEntry_LostFocus(sender, e);
                }
            }

            public void newEntryEntry_LostFocus(object sender, RoutedEventArgs e) {
                if (newEntryEntry.textBox.Text != null && newEntryEntry.textBox.Text != "") {
                    Student newStudent = new Student(newEntryEntry.textBox.Text);
                    context.Students.Add(newStudent);
                    context.SaveChanges();
                    updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.   
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
