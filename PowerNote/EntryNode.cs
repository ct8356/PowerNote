using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.Migrations;
using System.Windows.Data;
using System.Windows.Input;

namespace PowerNote {
    class EntryNode : TreeViewItem {
        public Student Student {get; set;}
        MainPanel mainPanel;
        MyContext context;

        public EntryNode(Student student, MainPanel mainPanel) {
            this.Student = student;
            this.mainPanel = mainPanel;
            student.PropertyChanged += student_PropertyChanged;
            context = mainPanel.Context;
            EntryPanel entry = new EntryPanel(student, mainPanel);
            Header = entry;
        }
        //MAYBE I don't need this class anymore?

        public void student_PropertyChanged(Object sender, PropertyChangedEventArgs e) {
            context.SaveChanges();
            //RIGHT, so this IS being called, straight after the property is set.
            //BUT it is not persisting it! Why? ahh! Because it is seeding it every time!
            if (e.PropertyName == "Priority") {
                mainPanel.updateEntries();
            }
        }

    }
}
