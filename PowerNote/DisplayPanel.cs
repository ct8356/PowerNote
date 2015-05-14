using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Migrations;
using System.Windows.Data;

namespace PowerNote {

    class DisplayPanel : DockPanel {
            Label label1;
            Label label2;
            TextBox textBox1;
            TextBox textBox2;
            SchoolContext schoolContext;

            public DisplayPanel(SchoolContext schoolContext) {
                this.schoolContext = schoolContext;
                //PANEL
                label1 = new Label();
                label1.Content = "Display panel";
                label1.FontWeight = FontWeights.Bold;
                textBox1 = new TextBox();
                textBox1.Text = schoolContext.Database.Connection.ConnectionString;
                textBox2 = new TextBox();
                Children.Add(label1);
                //Children.Add(textBox1);
                SetDock(label1, Dock.Top);
                //ADD ENTRIES
                for (int i = 0; i < schoolContext.Students.ToList().Count; i++) {
                    Entry entry = new Entry(schoolContext.Students.ToList()[i], schoolContext);
                    Children.Add(entry);
                    SetDock(entry, Dock.Top);
                }
                label2 = new Label();
                //label2.Content = schoolContext.Students.ToList()[0].FirstMidName;
                Binding binding = new Binding("FirstMidName"); //This is the MODEL property it binds to.
                binding.Source = schoolContext.Students.ToList()[0]; // the binding source (which must fire a PROP CHANGED event).
                label2.SetBinding(Label.ContentProperty, binding);
                Children.Add(label2);
                LastChildFill = false;
            }

    }
}
