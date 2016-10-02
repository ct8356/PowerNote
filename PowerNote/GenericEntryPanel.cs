using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using CJT.Models;
using PowerNote.ViewModels;
using PowerNote.DAL;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CJT;
using AutoCompleteBox = CJT.AutoCompleteBox;
using ListBox = CJT.ListBox;

namespace PowerNote {
    public class GenericEntryPanel : StackPanel {

        public GenericEntryPanel() {
            Orientation = Orientation.Horizontal;
            //RIGHT CLICKS
            //DependencyProps is the real proper way anyway.
            //AUTOCOMPLETEBOX
            //autoCompleteBox.SelectionChanged += autoCompleteBox_SelectionChanged;
            //autoCompleteBox.LostFocus += autoCompleteBox_LostFocus;
            //TRY KEEPING below method here, BUT, call it from sub class...
            //MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
            //SUBSCRIBE TO EVENTS
            DataContextChanged += this_DataContextChanged; //Is Loaded better?
            MouseUp += this_MouseUp;
        }

        public void conditionalSubscribe() {
            if ((DataContext as EntryVM).TreeVM.WaitingForParentSelection)
                MouseUp += sendToFosterParent;
        } //call this onLoad?

        public void sendToFosterParent(object sender, RoutedEventArgs e) {
            (DataContext as EntryVM).TreeVM.WaitingForParentSelection = false;
            MouseUp -= sendToFosterParent; //unsubscribe
            EntryPanel selectedParentPanel = sender as EntryPanel;
            (selectedParentPanel.DataContext as EntryVM).adoptChildFromTreeVM();
        }

        public void changeParent_Click(object sender, RoutedEventArgs e) {
            //Entry entry = sender as Entry;
            (DataContext as EntryVM).changeParent();
        }

        public void deleteEntry_Click(object sender, RoutedEventArgs e) {
            (DataContext as EntryVM).deleteEntry();
        }

        public void this_MouseUp(object sender, EventArgs e) {
            (DataContext as EntryVM).updateSelectedEntry(DataContext as EntryVM);
        } //AHAH! perhaps you just need to call the DATACONTEXT of this,
        //THEN CALL the datacontext of the treeview,
        //THEN to the datacontext of the propertiesPanel,
        //THEN properties panel SEES it has changed, and so updates itself...
        //AND THAT is the case when THEN need a listener!!! listening to Datacontext, property changed!

        public void filterAndSortTagsShown() {
            IEnumerable<int> filterCourseIDs = 
                (DataContext as EntryVM).TagsVM.SelectableItems.Select(c => (c as Tag).EntryID);
            var alphabeticalCourses = (DataContext as EntryVM)
                .Entry.Tags.Where(c => !filterCourseIDs.Contains(c.EntryID)).OrderBy(c => c.Name);
        }

        public void courseList_PropertyChanged(Object sender, EventArgs e) {
            updateTagLabels();
        }

        public void this_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            updateControls();
        }

        public void updateControls() {
            Children.Clear();
            EntryVM selectedEntryVM = (DataContext as EntryVM);
            foreach (Property property in selectedEntryVM.ImportantProperties) {
                if (property.IsVisibleInEntryPanel && property.Value != null) {
                    switch (property.Type) {
                        case InfoType.TextBox:
                            TextBox textBox = new TextBox();
                            Binding binding = new Binding("Value"); //This is the MODEL property it binds to.
                            binding.Source = property; // the binding source (which must fire a PROP CHANGED event).
                            textBox.SetBinding(TextBox.TextProperty, binding); //fortunately, textBox already fires an event when changed.
                            Children.Add(textBox);
                            break;
                        case InfoType.ComboBox:
                            break;
                        case InfoType.ListBox:
                            Children.Add(new ListBox() { ItemsSource = property.Value as ObservableCollection<Entry> });
                            break;
                        case InfoType.CheckBox:
                            break;
                    }
                }
            }
        }

        public void updateTagLabels() {
            filterAndSortTagsShown();
        }

  
    }
}
