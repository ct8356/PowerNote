using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PowerNote.Models;
using PowerNote.ViewModels;
using PowerNote.DAL;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CJT;
using AutoCompleteBox = CJT.AutoCompleteBox;

namespace PowerNote {
    public class EntryPanel : StackPanel {
        public TextBlock TextBlock { get; set; }

        public EntryPanel() {
            TextBlock = new TextBlock();
            Children.Add(TextBlock);
            Orientation = Orientation.Horizontal;
            //conditionalSubscribe(); //OF COURSE! WON'T WORK, coz datacontext don't exist yet.
            //REALLY, need to do conditional subscribe in XAML? But, complex, CBTL.
            //AUTOCOMPLETEBOX
            //autoCompleteBox.SelectionChanged += autoCompleteBox_SelectionChanged;
            //autoCompleteBox.LostFocus += autoCompleteBox_LostFocus;
            //TRY KEEPING below method here, BUT, call it from sub class...
            //MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
            //SUBSCRIBE TO EVENTS
            this.MouseUp += this_MouseUp;
            DataContextChanged += This_DataContextChanged;
        }

        public void conditionalSubscribe() {
            if ((DataContext as EntryVM).TreeVM.WaitingForParentSelection)
                this.MouseUp += sendToFosterParent;
        }

        public void changeParent_Click(object sender, RoutedEventArgs e) {
            //Entry entry = sender as Entry;
            (DataContext as EntryVM).changeParent();
        }

        public void deleteEntry_Click(object sender, RoutedEventArgs e) {
            (DataContext as EntryVM).deleteEntry();
        }

        public void autoCompleteBox_KeyUp(object sender, KeyEventArgs e) {
            //Oddly, this is called twice when you click in the dropdown box. I guess, lets accept that, and work around it.
            //Put an if statement in there, to stop something being saved twice.
            if (e.Key == Key.Return) {
                AutoCompleteBox autoCompleteBox = sender as AutoCompleteBox;
                List<Tag> tags = (DataContext as EntryVM).DbContext.Tags.ToList<Tag>();
                if (tags.Select(t => t.Title).Contains(autoCompleteBox.Text)) {
                    //i.e. IF course exists already, then say "selection Changed!".
                    //This method no longer needed, but keep it, just in case.
                    autoCompleteBox_SelectionChanged(sender, e);
                    //OR, just do nothing, since called anyway, when selection changed.
                    //WHICH is called first by the way? LostFocus, or SelectionCHanged? lostFocus first. SC, SC. LF.
                }
                else {
                    //IF no, then create new entry.
                    if (autoCompleteBox.Text != null && autoCompleteBox.Text != "") {
                        (DataContext as EntryVM).addNewTagToEntry(sender, autoCompleteBox.Text);
                    }
                }
            }
        }

        public void autoCompleteBox_SelectionChanged(object sender, RoutedEventArgs e) {
            //Add new tag to Navigation property
            AutoCompleteBox autoCompleteBox = (AutoCompleteBox)sender;
            Tag selectedCourse = (Tag)autoCompleteBox.SelectedItem;
            (DataContext as EntryVM).addTagToEntry(sender, selectedCourse);
        }

        public void filterAndSortTagsShown() {
            IEnumerable<int> filterCourseIDs = 
                (DataContext as EntryVM).TagsInputVM.Objects.Select(c => (c as Tag).TagID);
            var alphabeticalCourses = (DataContext as EntryVM)
                .Entry.Tags.Where(c => !filterCourseIDs.Contains(c.TagID)).OrderBy(c => c.Title);
        }

        public void courseList_PropertyChanged(Object sender, EventArgs e) {
            updateTagLabels();
        }

        public void sendToFosterParent(object sender, RoutedEventArgs e) {
            (DataContext as EntryVM).TreeVM.WaitingForParentSelection = false;
            this.MouseUp -= sendToFosterParent; //unsubscribe
            EntryPanel selectedParentPanel = sender as EntryPanel;
            (selectedParentPanel.DataContext as EntryVM).adoptChildFromTreeVM();
        }

        public void This_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            //RIGHT CLICKS
            //DependencyProps is the real proper way anyway.
            EntryVM dataContext = DataContext as EntryVM;
            MenuItem insertEntry = new MenuItem() { Header = "Insert entry" };
            insertEntry.Click += delegate { dataContext.insertEntry(dataContext); };//USE polymorphism!
            MenuItem insertSubEntry = new MenuItem() { Header = "Insert sub-entry" };
            insertSubEntry.Click += delegate { dataContext.insertSubEntry(dataContext); };
            MenuItem deleteEntry = new MenuItem() { Header = "Delete entry" };
            deleteEntry.Click += delegate { dataContext.deleteEntry(); };
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(insertEntry);
            contextMenu.Items.Add(insertSubEntry);
            contextMenu.Items.Add(deleteEntry);
            ContextMenu = contextMenu;
            //TITLE
            TextBlock.Text = (DataContext as EntryVM).ToString();
        }

        public void this_MouseUp(object sender, EventArgs e) {
            (DataContext as EntryVM).updateSelectedEntry(DataContext as EntryVM);
        } //AHAH! perhaps you just need to call the DATACONTEXT of this,
        //THEN CALL the datacontext of the treeview,
        //THEN to the datacontext of the propertiesPanel,
        //THEN properties panel SEES it has changed, and so updates itself...
        //AND THAT is the case when THEN need a listener!!! listening to Datacontext, property changed!

        public void updateTagLabels() {
            filterAndSortTagsShown();
        }

    }
}
