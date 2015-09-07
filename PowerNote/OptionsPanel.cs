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
using PowerNote.Models;

namespace PowerNote {
    public class OptionsPanel : StackPanel {
        MyContext context;
        public bool ShowAllEntries { get; set; }
        public CheckBox ShowAllEntriesCBox { get; set; }
        public bool ShowAllChildren { get; set; }
        public CheckBox ShowAllChildrenCBox { get; set; }

        public OptionsPanel(MyContext context) {
            this.context = context;
            //PANEL
            Orientation = Orientation.Horizontal;
            //SHOW ALL ENTRIES
            ShowAllEntriesCBox = new CheckBox();
            ShowAllEntriesCBox.Content = "Show all entries";
            Children.Add(ShowAllEntriesCBox);
            bind("ShowAllEntries", this, ShowAllEntriesCBox);
            //SHOW ALL CHILDREN
            ShowAllChildrenCBox = new CheckBox();
            ShowAllChildrenCBox.Content = "Show all children";
            Children.Add(ShowAllChildrenCBox);
            bind("ShowAllChildren", this, ShowAllChildrenCBox);
        }

        public void bind(String property, object source, CheckBox checkBox) {
            Binding binding = new Binding(property); //This is the MODEL property it binds to.
            binding.Source = source; // the binding source (which must fire a PROP CHANGED event).
            checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
        }

    }
}