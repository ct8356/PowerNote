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
    public class EntryVM<T> : BaseClass where T : Entry {
        public GenericTreeVM<T> TreeVM { get; set; }
        public T Entry { get; set; }
        public DAL.DbContext DbContext { get; set; }
        public ObservableCollection<Property> AllProperties { get; set; }
        public ListBoxVM<Tag> TagsVM { get; set; }
        public InputVM<Tag> TagsInputVM { get; set; }
        public EntryVM<T> Parent { get; set; }
        //NOTE! I think proper way to do this, is to just MODIFY the entry,
        //BUT because the the EntryVM is bound to it, it will update itself accordingly!
        //i.e. this entryVM just has a public Entry Parent.
        //NOW, when that Entry is deleted, the EntryVM deletes itself. SO this EntryVM,
        //does not even need to know its own ParentVM???? maybe... not sure yet.
        public ObservableCollection<EntryVM<T>> Children { get; set; }
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

        protected void initialize(T entry, GenericTreeVM<T> treeVM) {
            TreeVM = treeVM;
            Children = new ObservableCollection<EntryVM<T>>();
            FilterTags = treeVM.Filter;
            DbContext = treeVM.DbContext;
            bindToEntry(entry);
            //PROPERTIES
            initializePropertyList();
            //SUBSCRIBE
            Entry.PropertyChanged += Entry_PropertyChanged;
            TagsInputVM.InputConfirmed += TagsVM.This_Add;
        }

        protected virtual void initializePropertyList() {
            AllProperties = new ObservableCollection<Property>();
            AllProperties.Add(new Property("Entry ID", Entry.EntryID, InfoType.TextBox, false, DbContext));
            AllProperties.Add(new Property("Creation date", Entry.CreationDate, InfoType.TextBox, false, DbContext));
            AllProperties.Add(new Property("Parent", Entry.Parent, InfoType.TextBox, false, DbContext));
            AllProperties.Add(new Property("Children", Entry.Children, InfoType.ListBox, false, DbContext));
        }

        public void addNewTagToEntry(object sender, string text) {
            Tag newTag = new Tag(); newTag.Title = text;
            DbContext.Tags.Add(newTag); DbContext.SaveChanges();
            addTagToEntry(sender, newTag);
        }

        public void addTagToEntry(object sender, Tag selectedCourse) {
            if (Entry.Tags.Contains(selectedCourse)) {
                //do nothing
            }
            else {
                Entry.Tags.Add(selectedCourse);
                DbContext.SaveChanges();
                //ParentVM.ParentVM.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.  
                //above is bad, because deletes all entryVMs.
                (sender as AutoCompleteBox).Text = null;
            }
        }

        public void adoptChild(EntryVM<T> childVM) {
            //NOTE: This method should only be used if want to ADD children to ENTRY!!
            Children.Add(childVM); //Does this do the trick? Yes it seems to...
            childVM.Parent = this;
            Entry.Children.Add(childVM.Entry);
        }

        public void adoptSibling(EntryVM<T> entryVM) {
            Parent.Children.Add(entryVM);
            entryVM.Parent = Parent;
            Parent.Entry.Children.Add(entryVM.Entry);
        }

        public void adoptChildFromTreeVM() {
            Entry.Children.Add(TreeVM.Orphan);
        }

        public void bindToEntry(T entry) {
            Entry = entry;
            DbContext.Tags.Load();
            TagsInputVM = new InputVM<Tag>();
            TagsInputVM.Objects = DbContext.Tags.Local;
            TagsVM = new ListBoxVM<Tag>(); 
            //AND need to BIND THIS to the ENTRYs Tags!
            foreach (Tag tag in DbContext.Tags.Local) {
                TagsInputVM.Objects.Add(tag);
            }
            foreach (Tag tag in Entry.Tags) {
                TagsVM.Objects.Add(tag); //Not a proper binding really...
            }
            //NOW bind to this, in the XAML!
        }

        public void changeParent() {
            T parent = Entry.Parent as T; //just to check if it is anything! if EF works.
            //(EVEN if it does work, its a fluke. MUST be a way of FURTHER specifying/Explicitfying link between parent and child).
            Entry.Parent = null;
            TreeVM.waitForParentSelection(Entry);
        }

        public void deleteEntry() {
            DbContext.Entrys.Remove(Entry);
            if (Parent != null) {//IF it has a parent: delete self from children
                Parent.Children.Remove(this);
            }
            else { //else delete self from FirstLevelVMs
                TreeVM.FirstGenEntryVMs.Remove(this);
            }
            DbContext.SaveChanges(); //ALSO lazy. CBTL.
            //SO NOTE: OF COURSE, easier you just do these things, IN THE VIEWMODEL!
            //TreeVM.ParentVM.updateEntries(); //CBTL. Lazy way to do it. (rather than using events). But ok for now.
        }

        public void Entry_PropertyChanged(object sender, PropertyChangedEventArgs args) {
            DbContext.SaveChanges();
        }

        public virtual void insertEntry(EntryVM<T> selectedVM) {
            //Do nothing
        }

        public void insertEntry(EntryVM<T> entryVM, EntryVM<T> selectedVM) {
            if (Parent != null) {//IF it has a parent: make new one a sibling.
                adoptSibling(entryVM);
            } else { //else put it in FirstLevelVMs
                TreeVM.FirstGenEntryVMs.Add(entryVM);
            }
            foreach (Tag tag in FilterTags.SelectedObjects) {
                entryVM.Entry.Tags.Add(tag);
            }
            DbContext.SaveChanges();
            //ParentVM.updateEntries(); //not needed? shouldn't be. Not needed for inserting tags...
        }

        public virtual void insertSubEntry(EntryVM selectedVM) {
            //Do nothing
        }

        public void insertSubEntry(EntryVM<T> entryVM, EntryVM<T> selectedVM) {
            adoptChild(entryVM);
            foreach (Tag tag in FilterTags.SelectedObjects) {
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
