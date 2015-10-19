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

namespace PowerNote {
    public partial class EntryPanel : StackPanel {
        public TextBox TextBox { get; set; }

        public EntryPanel() {
            //conditionalSubscribe(); //OF COURSE! WON'T WORK, coz datacontext don't exist yet.
            //REALLY, need to do conditional subscribe in XAML? But, complex, CBTL.
            //RIGHT CLICKS
            //DependencyProps is the real proper way anyway.
            //AUTOCOMPLETEBOX
            //autoCompleteBox.SelectionChanged += autoCompleteBox_SelectionChanged;
            //autoCompleteBox.LostFocus += autoCompleteBox_LostFocus;
            //TRY KEEPING below method here, BUT, call it from sub class...
            //MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        public void conditionalSubscribe() {
            if ((DataContext as EntryVM).MainPanel.DisplayPanel
                .EntriesView.WaitingForParentSelection)
                this.MouseUp += sendToFosterParent;
        }

        public void sendToFosterParent(object sender, RoutedEventArgs e) {
            (DataContext as EntryVM).MainPanel.DisplayPanel
                .EntriesView.WaitingForParentSelection = false;
            this.MouseUp -= sendToFosterParent; //unsubscribe
            EntryPanel selectedParentPanel = sender as EntryPanel;
            (selectedParentPanel.DataContext as EntryVM).adoptChild();
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
                List<Tag> tags = (DataContext as EntryVM).Context.Tags.ToList<Tag>();
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
            IEnumerable<int> filterCourseIDs = (DataContext as EntryVM).MainPanel.DisplayPanel
                .FilterPanel.Filter.Objects.Select(c => (c as Tag).TagID);
            var alphabeticalCourses = (DataContext as EntryVM)
                .Entry.Tags.Where(c => !filterCourseIDs.Contains(c.TagID)).OrderBy(c => c.Title);
        }

        public void courseList_PropertyChanged(Object sender, EventArgs e) {
            updateTagLabels();
        }

        public void updateTagLabels() {
            filterAndSortTagsShown();
        }

    }
}
