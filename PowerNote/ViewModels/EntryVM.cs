using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using CJT.Models;
using PowerNote.DAL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows;
using CJT;
using AutoCompleteBox = CJT.AutoCompleteBox;

namespace PowerNote.ViewModels {
    public class EntryVM : BaseClass {
        //NOTE: MIGHT be able to make a generic version of this class,
        //BUT it is hard! lots of changing necessary. 
        //TOO MUCH CHANGING considering how short on time I am!
        public EntriesTreeVM TreeVM { get; set; }
        public Entry Entry { get; set; }
        public DAL.DbContext DbContext { get; set; }
        public static ObservableCollection<object> Properties { get; set; }
        public ObservableCollection<Property> ImportantProperties { get; set; }
        public ListBoxPanelVM<Tag> TagsVM { get; set; }
        public EntryVM Parent { get; set; }
        //NOTE! I think proper way to do this, is to just MODIFY the entry,
        //BUT because the the EntryVM is bound to it, it will update itself accordingly!
        //i.e. this entryVM just has a public Entry Parent.
        //NOW, when that Entry is deleted, the EntryVM deletes itself. SO this EntryVM,
        //does not even need to know its own ParentVM???? maybe... not sure yet.
        public ObservableCollection<EntryVM> Children { get; set; }
        //DON'T want others adding to children. If I make it private, will that stop binding?
        //No, it won't, provided you KEEP this property public!!!
        //BUT problem is, then you can still add to Children...issue...
        public ListBoxPanelVM<Tag> FilterTags { get; set; }
        public bool IsExpanded { get; set; }

        //SHOULD there be an option to emanciate??? (free from parent?)

        public EntryVM() {
            //do nothing.
            //NOTE: is this called? maybe. Even if it is, does not matter.
        }

        public EntryVM(EntriesTreeVM treeVM) {
            TreeVM = treeVM;
            DbContext = treeVM.DbContext;
        }

        protected void initialize(Entry entry, EntriesTreeVM treeVM) {
            TreeVM = treeVM;
            Children = new ObservableCollection<EntryVM>();
            FilterTags = treeVM.FilterPanelVM;
            DbContext = treeVM.DbContext;
            bindToEntry(entry);
            //PROPERTIES
            initializePropertyList();
            //SUBSCRIBE
            Entry.PropertyChanged += Entry_PropertyChanged;
            //TagsInputVM.InputConfirmed += TagsVM.This_Add;
            //OH MY GOSH! So you are saying, should register to all lists
            //from here? What a pain in the bum!
            //SURELY it is much easier to PASS reference of this EntryVM,
            //TO the inputVM (or just use the ListBoxPanelVM!).
            //AND call the EntryVM to add, from there!!!
            //WELL, yes I am...
            //BUT this way is more flexible!
            //BUT maybe also a bit stupid.
            //COZ only reason did it, was so did not have to PASS an EntryVM.
            //AND you could just make anything with lists attached,
            //an EntryVM. Or a ListAttachedObject...
        }

        protected virtual void initializePropertyList() {
            ImportantProperties = new ObservableCollection<Property>();
            ImportantProperties.Add(new Property("EntryID", Entry.EntryID, InfoType.TextBlock, false, DbContext));
            ImportantProperties.Add(new Property("CreationDate", Entry.CreationDate, InfoType.TextBlock, false, DbContext));
            ImportantProperties.Add(new Property("Name", Entry.Name, InfoType.TextBox, false, DbContext));
            ImportantProperties.Add(new Property("Parent", Entry.Parent, InfoType.LinkedTextBlock, false, DbContext));
            ImportantProperties.Add(new Property("Children", Children, InfoType.ListBox, false, DbContext));
            ImportantProperties.Add(new Property("Tags", TagsVM, InfoType.ListBox, true, DbContext));
        }

        public void adoptChild(EntryVM childVM) {
            //NOTE: This method should only be used if want to ADD children to ENTRY!!
            Children.Add(childVM); //Does this do the trick? Yes it seems to...
            childVM.Parent = this;
            Entry.Children.Add(childVM.Entry);
        }

        public void adoptSibling(EntryVM entryVM) {
            Parent.Children.Add(entryVM);
            entryVM.Parent = Parent;
            Parent.Entry.Children.Add(entryVM.Entry);
        }

        public void adoptChildFromTreeVM() {
            Entry.Children.Add(TreeVM.Orphan);
        }

        public void bindToEntry(Entry entry) {
            Entry = entry;
            TagsVM = new ListBoxPanelVM<Tag>(this);
            TagsVM.SelectableItems = TreeVM.AllTags; 
            TagsVM.SelectedItems = Entry.Tags; //YES! This actually seems to
            //create a BINDING between ThisVM and the Entry.Tags!!!
            //NOW bind to this, in the XAML!
        }

        public void changeParent() {
            Entry parent = Entry.Parent; //just to check if it is anything! if EF works.
            //(EVEN if it does work, its a fluke. MUST be a way of FURTHER specifying/Explicitfying link between parent and child).
            Entry.Parent = null;
            TreeVM.waitForParentSelection(Entry);
        }

        public void deleteEntry() {
            DbContext.Entries.Remove(Entry);
            if (Parent != null) {//IF it has a parent: delete self from children
                Parent.Children.Remove(this);
            }
            else { //else delete self from FirstLevelVMs
                TreeVM.FirstGenEntryVMs.Remove(this);
            }
            DbContext.SaveChanges(); //ALSO lazy. BUT more distributed!
            //Easier you just do these things, IN THE VIEWMODEL!
            //TreeVM.ParentVM.UpdateEntries();
            //REVISIT above it lazy, coz does whole tree, rather than just approp ones.
            //BUT  ok for now.
        }

        public void Entry_PropertyChanged(object sender, PropertyChangedEventArgs args) {
            DbContext.SaveChanges();
        }

        public virtual void insertEntry(EntryVM selectedVM) {
            //Do nothing
        }

        public void insertEntry(EntryVM entryVM, EntryVM selectedVM) {
            if (Parent != null) {//IF it has a parent: make new one a sibling.
                adoptSibling(entryVM);
            } else { //else put it in FirstLevelVMs
                TreeVM.FirstGenEntryVMs.Add(entryVM);
            }
            foreach (Tag tag in FilterTags.SelectedItems) {
                entryVM.Entry.Tags.Add(tag);
            }
            DbContext.SaveChanges();
            //ParentVM.updateEntries(); //not needed? shouldn't be. Not needed for inserting tags...
        }

        public virtual void insertSubEntry(EntryVM selectedVM) {
            //Do nothing
        }

        public void insertSubEntry(EntryVM entryVM, EntryVM selectedVM) {
            adoptChild(entryVM);
            foreach (Tag tag in FilterTags.SelectedItems) {
                entryVM.Entry.Tags.Add(tag);
            }
            DbContext.SaveChanges();
            //ParentVM.updateEntries(); //nec //is it though? well yes. Is there another way?
            //is entry.Children observable? Yes. BUT real issue is: is it adding a VM? No!
            //so make it do so!
            //DONE! CBTL, works, but could be more issues here...
        }

        public override string ToString() {
            return Entry.ToString();
        }

        public void updateSelectedEntry(EntryVM entryVM) {
            TreeVM.ParentVM.SelectedEntryVM = entryVM;
        }

    }
}
