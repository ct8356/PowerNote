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
using ListBox = CJT.ListBox;
using CJT;

namespace PowerNote {
    public class ControlPanel : DockPanel {
        DAL.DbContext context;
        MainPanel mainPanel;
        ListBox ListBox { get; set; }

        public ControlPanel(DAL.DbContext context, MainPanel mainPanel) {
            //LETS leave this out for a bit.
            this.context = context;
            this.mainPanel = mainPanel;
            //TITLE
            Label title = new Label();
            title.Content = "Control panel";
            Children.Add(title);
            SetDock(title, Dock.Top);
            title.FontWeight = FontWeights.Bold;
            //COLLECTION OF TAG LABELS
            ListBox = new System.Windows.Controls.ListBox() as ListBox;
            context.Tags.OrderBy(c => c.Title).Load(); //explicit load. I.e. submit query now.
            ListBox.ItemsSource = context.Tags.Local;
            ListBox.Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending)); //"" is for property name.
            ListBox.ContextMenu = new ContextMenu();
            MenuItem delete_menuItem = new MenuItem();
            ListBox.ContextMenu.Items.Add(delete_menuItem); //PROBLEM! menu belongs to listBox, not ITEM!
            delete_menuItem.Click += delete_menuItem_Click;
            delete_menuItem.Header = "Delete tag";
            Children.Add(ListBox);
            SetDock(ListBox, Dock.Top);
        }

        public void delete_menuItem_Click(Object sender, EventArgs e) {   
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem != null) {
                ContextMenu contextMenu = (ContextMenu)menuItem.Parent;
                System.Windows.Controls.ListBox listBox = (System.Windows.Controls.ListBox)contextMenu.PlacementTarget;
                //ListBox listBox = (ListBox)menuItem.DataContext;
                if (listBox.SelectedIndex == -1) return;
                Tag selectedCourse = (Tag)listBox.Items[listBox.SelectedIndex]; //the line i was missing.
                //Course selectedCourse = student.Courses.Single(c => c.Title == selectedCourseName);   
                if (selectedCourse != null)
                    context.Tags.Remove(selectedCourse);
                context.SaveChanges(); //ALSO lazy. CBTL.
                mainPanel.updateEntries();
            }
        }

        //public void reorder() {
        //    context.Courses.OrderBy(c => c.Title).Load(); //explicit load. I.e. submit query now.
        //    ListBox.ItemsSource = context.Courses.Local;
        //    Children.Remove(ListBox);
        //    Children.Add(ListBox);
        //}

    }
}
