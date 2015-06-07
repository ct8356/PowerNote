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
    class OptionsPanel : StackPanel {
        Label title;
        MyContext context;
        public bool ShowAllEntriesBool { get; set; }
        public CheckBox ShowAllEntries { get; set; }
        public bool ShowAllChildrenBool { get; set; }
        public CheckBox ShowAllChildren { get; set; }

        public OptionsPanel(MyContext context) {
            this.context = context;
            //PANEL
            Orientation = Orientation.Horizontal;
            //TITLE
            title = new Label();
            title.Content = "Options panel:";
            Children.Add(title);
            //SHOW ALL ENTRIES
            ShowAllEntries = new CheckBox();
            ShowAllEntries.Content = "Show all entries";
            Children.Add(ShowAllEntries);
            bind("ShowAllEntriesBool", this, ShowAllEntries);
            //SHOW ALL CHILDREN
            ShowAllChildren = new CheckBox();
            ShowAllChildren.Content = "Show all children";
            Children.Add(ShowAllChildren);
            bind("ShowAllChildrenBool", this, ShowAllChildren);
        }

        public void bind(String property, object source, CheckBox checkBox) {
            Binding binding = new Binding(property); //This is the MODEL property it binds to.
            binding.Source = source; // the binding source (which must fire a PROP CHANGED event).
            checkBox.SetBinding(CheckBox.IsCheckedProperty, binding);
        }

    }
}