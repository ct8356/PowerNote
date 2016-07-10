using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PowerNote.ViewModels;
using System.Windows.Controls;
using System.Windows;
using TextBlock = CJT.TextBlock;

namespace PowerNote {
    class PropertyPanel : StackPanel {

        public TextBlock TextBlock { get; set; }
        
        public PropertyPanel() {
            Orientation = Orientation.Horizontal;
        }

        public PropertyPanel(string name) : this() {
            TextBlock = new TextBlock();
            TextBlock.Text = name + ":";
            TextBlock.Width = 100;
            Children.Add(TextBlock);
        }
    }
}
