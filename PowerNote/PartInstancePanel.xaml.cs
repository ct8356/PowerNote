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
    public partial class PartInstancePanel : EntryPanel {
        public PartInstancePanel() {
            InitializeComponent();
            MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
            PropertyAutoCompleteBox.KeyUp += partClassACBox_KeyUp;
        }

        public void insertPart_Click(object sender, RoutedEventArgs e) {
            (DataContext as PartInstanceVM).insertPart();
        }

        //public void insertSubPart_Click(Object sender, EventArgs e) {
        //    MenuItem menuItem = sender as MenuItem;
        //    if (menuItem != null) {
        //        (DataContext as PartInstanceVM).insertSubPart(DataContext as PartInstanceVM);
        //    }
        //}

        public void partClassACBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
                //IF tag does NOT exist, then create new tag.
                if (autoCompleteBox.Text != null && autoCompleteBox.Text != "") {
                    (DataContext as PartInstanceVM).addPartClassToEntry(autoCompleteBox.Text);
                }
                autoCompleteBox.Text = null;
            }
        }

    }
}
