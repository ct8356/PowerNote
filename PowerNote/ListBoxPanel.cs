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
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Data;
using CJT;
using AutoCompleteBox = CJT.AutoCompleteBox;
using ListBox = CJT.ListBox;

namespace PowerNote {
    public class ListBoxPanel : StackPanel {
        public ListBox ListBox { get; set; }
        public AutoCompleteBox AutoCompleteBox { get; set; }

        public ListBoxPanel() {
            Orientation = Orientation.Horizontal;
            ListBox = new ListBox();
            Children.Add(ListBox);
            AutoCompleteBox = new AutoCompleteBox();
            Children.Add(AutoCompleteBox);
            DataContextChanged += this_DataContextChanged;
            AutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        public void autoCompleteBox_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
                autoCompleteBox_SelectionChanged(sender, e);
            }
        }

        public void autoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e) {
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            (DataContext as ListBoxPanelVM<Tag>).addSelectedItem(autoCompleteBox.SelectedItem as Tag);
            //IF THIS works HERE, then it will work EVERYWHERE! DO IT! CURRENT!
            autoCompleteBox.Text = null;
            //CURRENT! OF COURSE, problem is, this called twice. TEXT set to null!!
            //THEN, second time, null is put in to SelectedItems!
            //BUT SCREW THIS! lets just use a COMBOBOX!
        }

        public void bindTextBox(Task student) {
            //this.Student = student;
            //Binding binding = new Binding("Contents"); //This is the MODEL property it binds to.
            //binding.Source = Student; // the binding source (which must fire a PROP CHANGED event).
            //textBox.SetBinding(ListBox., binding); //fortunately, textBox already fires an event when changed.
            //YOU created the event for the dataSource. SO HOPEFULLY, we have 2 way binding now... we do :)
        }

        public void this_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            updateControls();
        }

        public virtual void updateControls() {
            ListBox.ItemsSource = (DataContext as ListBoxPanelVM<Entry>).SelectedObjects; //SHOULD WORK!
            //NOTE! trying to make this work, BUT dont think even need it, if bind in XAML!!!!
            AutoCompleteBox.ItemsSource = (DataContext as ListBoxPanelVM<Entry>).Objects;
            //LISTBOX
            //Binding binding = new Binding();
            //binding.Path = new PropertyPath("taskItems");
            //BindingOperations.SetBinding(ListBox, ListView.ItemsSourceProperty, binding);
        }

        //OK! might need to make a COMMON viewModel too, CBTL
        //THAT ALL things can bind too. (I guess that is what makes inheritance easier with XAML???)

    }
}
