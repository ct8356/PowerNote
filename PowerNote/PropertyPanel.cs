using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerNote.ViewModels;
using System.Windows.Controls;
using System.Windows;

namespace PowerNote {
    class PropertyPanel : StackPanel {

        public Label Label { get; set; }
        
        public PropertyPanel() {
            Orientation = Orientation.Horizontal;
        }

        public PropertyPanel(string name) : this() {
            Label = new Label();
            Label.Content = name + ":";
            Label.Width = 100;
            Children.Add(Label);
        }
    }
}
