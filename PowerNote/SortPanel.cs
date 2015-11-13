﻿using System;
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
    public class SortPanel : StackPanel {
        Label title;
        public ComboBox ComboBox { get; set; }

        public SortPanel() {
            //PANEL
            Orientation = Orientation.Horizontal;
            title = new Label();
            title.Content = "Sort by:";
            Children.Add(title);
            //DROP DOWN MENU
            ComboBox = new ComboBox();
            ComboBox.ItemsSource = new List<String>() { "ID", "Priority" };
            ComboBox.SelectedItem = "Priority";
            Children.Add(ComboBox);
        }

    }
}
