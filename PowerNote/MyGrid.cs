using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace PowerNote {
    class MyGrid : Grid{
        Label label;
        Label label2;

        public MyGrid() {
            ShowGridLines = true;
            //Width = 250;
            //Height = 100;
            // Define the Columns
            for (int col = 0; col < 2; col++) {
                ColumnDefinitions.Add(new ColumnDefinition());
            }
            // Define the Rows
            for (int row = 0; row < 2; row++) {
                RowDefinitions.Add(new RowDefinition());
            }
            //YES! This is important... (note, grid is not quite like tableLayout though...).Or is it?
            label = new Label();
            label.Content = "LABEL 1";
            label2 = new Label();
            label2.Content = "label 2";
            Children.Add(label);
            Children.Add(label2);
            Grid.SetColumn(label, 0);
            Grid.SetColumn(label2, 1);
        }

    }
}
