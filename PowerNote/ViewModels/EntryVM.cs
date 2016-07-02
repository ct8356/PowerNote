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

namespace PowerNote.ViewModels {
    public class EntryVM : INotifyPropertyChanged {
        public EntriesTreeVM TreeVM { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public Entry Entry { get; set; }
        public DAL.DbContext DbContext { get; set; }
        public ObservableCollection<Property> AllProperties { get; set; }
        public ObservableCollection<Tag> AllTags { get; set; }
        public ListBoxVM<object> TagsVM { get; set; }
        //WANT to make this GENERIC, AND bind it to a particular LIST, in Entry... CBTL CURRENT
        public String MyString { get; set; }
        public ObservableCollection<string> TestString { get; set; } = new ObservableCollection<string>() { "fah", "lah" };

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
        public FilterPanelVM FilterPanelVM { get; set; }
        public bool IsExpanded { get; set; }

        //SHOULD there be an option to emanciate??? (free from parent?)

        public EntryVM() {
            //do nothing.
            //NOTE: is this called? maybe. Even if it is, does not matter.
        }

        public void initialize(Entry entry, EntriesTreeVM treeVM) {
            TreeVM = treeVM;
            Children = new ObservableCollection<EntryVM>();
            FilterPanelVM = treeVM.Filter;
            DbContext = treeVM.DbContext;
            DbContext.Tags.Load();
            AllTags = DbContext.Tags.Local;
            bindToEntry(entry);
            //PROPERTIES
            initializePropertyList();
            //SUBSCRIBE
            Entry.PropertyChanged += Entry_PropertyChanged;
        }

        public virtual void initializePropertyList() {
            AllProperties = new ObservableCollection<Property>();
            AllProperties.Add(new Property("Entry ID", Entry.EntryID, InfoType.TextBox, false, this));
            AllProperties.Add(new Property("Creation date", Entry.CreationDate, InfoType.TextBox, false, this));
            AllProperties.Add(new Property("Parent", Entry.Parent, InfoType.TextBox, false, this));
            AllProperties.Add(new Property("Children", Entry.Children, InfoType.ListBox, false, this));
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
            TagsVM = new ListBoxVM<object>(TreeVM.ParentVM);
            //AND need to BIND THIS to the ENTRYs Tags!
            foreach (Tag tag in DbContext.Tags.Local) {
                TagsVM.Objects.Add(tag);
            }
            foreach (Tag tag in Entry.Tags) {
                TagsVM.SelectedObjects.Add(tag); //Not a proper binding really...
            }
            //NOW bind to this, in the XAML!
        }

        public void changeParent() {
            Entry parent = Entry.Parent; //just to check if it is anything! if EF works.
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

        public void insertEntry(EntryVM entryVM, EntryVM selectedVM) {
            if (Parent != null) {//IF it has a parent: make new one a sibling.
                adoptSibling(entryVM);
            } else { //else put it in FirstLevelVMs
                TreeVM.FirstGenEntryVMs.Add(entryVM);
            }
            foreach (Tag tag in FilterPanelVM.SelectedObjects) {
                entryVM.Entry.Tags.Add(tag);
            }
            DbContext.SaveChanges();
            //ParentVM.updateEntries(); //not needed? shouldn't be. Not needed for inserting tags...
        }

        public void insertSubEntry(EntryVM entryVM, EntryVM selectedVM) {
            adoptChild(entryVM);
            foreach (Tag tag in FilterPanelVM.SelectedObjects) {
                entryVM.Entry.Tags.Add(tag);
            }
            DbContext.SaveChanges();
            //ParentVM.updateEntries(); //nec //is it though? well yes. Is there another way?
            //is entry.Children observable? Yes. BUT real issue is: is it adding a VM? No!
            //so make it do so!
            //DONE! CBTL, works, but could be more issues here...
        }

        protected void NotifyPropertyChanged(String propertyName) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void updateSelectedEntry(EntryVM entryVM) {
            TreeVM.ParentVM.SelectedEntryVM = entryVM;
        }
    }
}
