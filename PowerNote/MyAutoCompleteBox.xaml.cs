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
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PowerNote.Models;
using PowerNote.DAL;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using PowerNote.ViewModels;

namespace PowerNote {
    public partial class MyAutoCompleteBox : AutoCompleteBox {
        
        public MyAutoCompleteBox() {
            InitializeComponent();
            IsTextCompletionEnabled = true; //YES! Works a treat. Just backspace if dont want text completion.
            //BUT it would be good if it would highlight the box below too. for easy up and down arrowing. //Not important though.
        }

        public void autoCompleteBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
                autoCompleteBox_SelectionChanged(sender, e);
            }
        }

        public void autoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e) {
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            (DataContext as ListBoxVM<object>).addSelectedItem(autoCompleteBox.SelectedItem);
            //IF THIS works HERE, then it will work EVERYWHERE! DO IT! CURRENT!
            autoCompleteBox.Text = null;
        }

        private void UpdateSelection() { //NOT USED
            // get the source of the ListBox control inside the template
            var enumerator = ((Selector)GetTemplateChild("Selector")).ItemsSource.GetEnumerator();
            // update Selecteditem with the first item in the list
            enumerator.Reset();
            if (enumerator.MoveNext()) {
                var item = enumerator.Current;
                SelectedItem = item;
                // close the popup, highlight the text
                IsDropDownOpen = false;
                //(TextBox)GetTemplateChild("Text").SelectAll();
            }
        }
    }
}
