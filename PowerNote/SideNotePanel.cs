using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using PowerNote.DAL;
using CJT.Models;
using PowerNote.Migrations;
using System.Windows.Data;
using System.Windows.Input;

namespace PowerNote {
    class SideNotePanel : DockPanel {
        Label title;
        DAL.DbContext context;
        SideEntry sideEntry;
        Button newEntry_button;
        MainPanel mainPanel;

        public SideNotePanel(DAL.DbContext context, MainPanel mainPanel) {
            this.context = context;
            this.mainPanel = mainPanel;
            //PANEL
            title = new Label();
            title.Content = "Side-note panel";
            title.FontWeight = FontWeights.Bold;
            Children.Add(title);
            SetDock(title, Dock.Top);
            //SIDE ENTRY
            sideEntry = new SideEntry(context, this);
            Children.Add(sideEntry);
            SetDock(sideEntry, Dock.Top);
            sideEntry.LostFocus += new RoutedEventHandler(sideEntry_LostFocus);
            sideEntry.KeyUp += new KeyEventHandler(sideEntry_KeyUp);
            //POST BUTTON
            newEntry_button = new Button();
            newEntry_button.Content = "New side-note";
            Children.Add(newEntry_button);
            SetDock(newEntry_button, Dock.Top);
            //OTHER
            LastChildFill = false;
        }

        public void sideEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                sideEntry_LostFocus(sender, e);
            }
        }

        public void sideEntry_LostFocus(object sender, RoutedEventArgs e) {
            if (sideEntry.textBox.Text != null && sideEntry.textBox.Text != "") {
                Task newStudent = new Task(sideEntry.textBox.Text);
                context.Tasks.Add(newStudent);
                context.SaveChanges();
                //mainPanel.DisplayPanel.updateEntries();
                //CBTL
                //TODO
                sideEntry.bindTextBox(newStudent);
            }
        }
    }
}
