using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PowerNote.Models;
using PowerNote.ViewModels;
using System.Collections.ObjectModel;

namespace PowerNote {
    public class ListBoxPanel : StackPanel {

        public ListBoxPanel() {
        }

        public void autoCompleteBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
                autoCompleteBox_SelectionChanged(sender, e);
            }
        }

        public void autoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e) {
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            (DataContext as ListBoxPanelVM).addSelectedItem(autoCompleteBox.SelectedItem);
            autoCompleteBox.Text = null;
        }

        //OK! might need to make a COMMON viewModel too, CBTL
        //THAT ALL things can bind too. (I guess that is what makes inheritance easier with XAML???)

    }
}
