using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using PowerNote.DAL;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PowerNote.Models;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PowerNote {
    class ControlPanel : DockPanel {
        MyContext context;
        List<Label> labelList;

        public ControlPanel(MyContext context) {
            this.context = context;
            //TITLE
            Label title = new Label();
            title.Content = "Control panel";
            Children.Add(title);
            SetDock(title, Dock.Top);
            title.FontWeight = FontWeights.Bold;
            //COLLECTION OF TAG LABELS
            labelList = new List<Label>();
            //COULD do it like this, BUT then cannot bind it. BETTER, to use listBox/listView?for now,yes
            ListBox listBox = new ListBox();
            context.Courses.Load();
            listBox.ItemsSource = context.Courses.Local;
            listBox.ContextMenu = new ContextMenu();
            MenuItem delete_menuItem = new MenuItem();
            listBox.ContextMenu.Items.Add(delete_menuItem); //PROBLEM! menu belongs to listBox, not ITEM!
            delete_menuItem.Click += delete_menuItem_Click;
            delete_menuItem.Header = "Delete tag";
            Children.Add(listBox);
            SetDock(listBox, Dock.Top);
            //NEW ENTRY BUTTON
            Button newEntryButton = new Button();
            newEntryButton.Content = "New entry";
            Children.Add(newEntryButton);
            SetDock(newEntryButton, Dock.Top);
            //SAVE CHANGES BUTTON
            Button saveChangesButton = new Button();
            saveChangesButton.Content = "Save changes";
            //Children.Add(saveChangesButton);
            //SUBSCRIBE TO EVENT
            //necessary?
            saveChangesButton.Click += saveChangesButton_Click;
            //YES! IT WORKS! perhaps, the delegate, already exists? so don't need the new keyword?
        }

        private void saveChangesButton_Click(object sender, EventArgs e) {
            //Ah, note, a delegate, calling this method,
            //is automatically subscribed to the button event!
            //BUT guess what, it is not calling this...
            context.SaveChanges();
        }

        public void delete_menuItem_Click(Object sender, EventArgs e) {   
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem != null) {
                ContextMenu contextMenu = (ContextMenu)menuItem.Parent;      
                ListBox listBox = (ListBox)contextMenu.PlacementTarget;
                //ListBox listBox = (ListBox)menuItem.DataContext;
                if (listBox.SelectedIndex == -1) return;
                Course selectedCourse = (Course)listBox.Items[listBox.SelectedIndex]; //the line i was missing.
                //Course selectedCourse = student.Courses.Single(c => c.Title == selectedCourseName);   
                if (selectedCourse != null)
                    context.Courses.Remove(selectedCourse);
                //updateTagLabels(); Should not have to, since I will only delete ones with NO attachments. For now.
                context.SaveChanges(); //ALSO lazy. CBTL.
            }
        }
    }
}
