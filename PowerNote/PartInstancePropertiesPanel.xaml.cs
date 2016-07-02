using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PowerNote.ViewModels;

namespace PowerNote {
    public partial class PartInstancePropertiesPanel : StackPanel {
        public PartInstancePropertiesPanel() {
            InitializeComponent();
            //SUBSCRIBE
            //(DataContext as EntryVM).PropertyChanged += DataContext_PropertyChanged;
            //can't subscribe to this property, if "this" NOT constructed yet!
        }

        public void DataContext_PropertyChanged(object sender, EventArgs e) {
            
        }

    }
}
