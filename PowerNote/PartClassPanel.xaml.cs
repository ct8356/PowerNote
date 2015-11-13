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
using PowerNote.Models;
using PowerNote.ViewModels;

namespace PowerNote {
    public partial class PartClassPanel : EntryPanel {
        public PartClassPanel() {
            InitializeComponent();
            MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        public void insertPart_Click(object sender, RoutedEventArgs e) {
            (DataContext as PartClassVM).insertPart(DataContext as PartClassVM);
        }

        public void insertSubPart_Click(Object sender, EventArgs e) {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null) {
                (DataContext as PartClassVM).insertSubPart(DataContext as PartClassVM);
            }
        }
    }
}
