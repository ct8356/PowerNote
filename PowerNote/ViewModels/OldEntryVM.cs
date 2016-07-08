using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using PowerNote.Models;
using PowerNote.DAL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows;
using CJT;
using AutoCompleteBox = CJT.AutoCompleteBox;

namespace PowerNote.ViewModels {
    public class OldEntryVM : INotifyPropertyChanged {
        public EntriesTreeVM TreeVM { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public Entry Entry { get; set; }
        public DAL.DbContext Context { get; set; }
        public ObservableCollection<string> AllProperties { get; set; }
        public ObservableCollection<Tag> AllTags { get; set; }
        public EntryVM Parent { get; set; }
        public ObservableCollection<EntryVM> Children { get; set; }
        public ListBoxPanelVM<Tag> Filter { get; set; }
        public bool IsExpanded { get; set; }

        public OldEntryVM() {
            //do nothing.
            //NOTE: is this called? maybe. Even if it is, does not matter.
        }

        public void initialize(Entry entry, EntriesTreeVM treeVM) {
            bindToEntry(entry);
            TreeVM = treeVM;
            Children = new ObservableCollection<EntryVM>();
            Filter = treeVM.Filter;
            Context = treeVM.DbContext;
            AllProperties = new ObservableCollection<string>();
            Context.Tags.Load();
            AllTags = Context.Tags.Local; 
        }

        public void addNewTagToEntry(object sender, string text) {
            Tag newTag = new Tag(); newTag.Title = text;
            Context.Tags.Add(newTag); Context.SaveChanges();
            addTagToEntry(sender, newTag);
        }

        public void addTagToEntry(object sender, Tag selectedCourse) {
            if (Entry.Tags.Contains(selectedCourse)) {
                //do nothing
            }
            else {
                Entry.Tags.Add(selectedCourse);
                Context.SaveChanges();
                //ParentVM.ParentVM.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.  
                //above is bad, because deletes all entryVMs.
                (sender as AutoCompleteBox).Text = null;
            }
        }

        public void adoptChild() {
            Entry.Children.Add(TreeVM.Orphan);
        }

        public void bindToEntry(Entry entry) {
            Entry = entry;
        }

        public void changeParent() {
            Entry parent = Entry.Parent; //just to check if it is anything! if EF works.
            //(EVEN if it does work, its a fluke. MUST be a way of FURTHER specifying/Explicitfying link between parent and child).
            Entry.Parent = null;
            TreeVM.waitForParentSelection(Entry);
        }

        public void deleteEntry() {
            Context.Entrys.Remove(Entry);
            Context.SaveChanges(); //ALSO lazy. CBTL.
            //SO NOTE: OF COURSE, easier you just do these things, IN THE VIEWMODEL!
            TreeVM.ParentVM.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
        }

    }
}
