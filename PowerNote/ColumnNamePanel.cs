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
    class ColumnNamePanel : StackPanel {

        public ColumnNamePanel(DisplayPanel displayPanel) {
            //PANEL 
            Orientation = Orientation.Horizontal;
            foreach (String colName in displayPanel.ColumnNames) {
                Label label = new Label();
                label.Content = colName;
                Children.Add(label);
                label.FontWeight = FontWeights.Bold;
                label.Width = 100;
            }
        }

    }
}
