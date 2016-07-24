using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PowerNote.DAL;
using System.ComponentModel; //this allows INotifyPropertyChanged
using PowerNote.Models;
using CJT;

namespace PowerNote.ViewModels {
    public class ListBoxPanelVM<T> : BaseClass, INotifyInputConfirmed, IListVM where T : Entry, new() {
        //NOTE: this class is not so flexible,
        //BUT it is used so often, that it is worth using!
        public delegate void ObjectEventHandler(object sender, ObjectEventArgs<T> e);
        public event ObjectEventHandler InputConfirmed;
        public MainVM MainVM { get; set; }
        public EntryVM EntryVM { get; set; }
        public DAL.DbContext DbContext { get; set; }
        public DbSet<T> DbSet { get; set; }
        public ObservableCollection<T> SelectableItems { get; set; }
        private ObservableCollection<T> selectedItems;
        public ObservableCollection<T> SelectedItems {
            get { return selectedItems; }
            set { selectedItems = value; NotifyPropertyChanged("SelectedItems"); }
        }

        public ListBoxPanelVM(EntryVM VM) {
            //OK, at one style of ListBoxPanelVM (the style that goes in entryVMs)
            //NEEDS to know about EntryVM,
            //OTHERWISE, it means the EntryVM has to subscribe to all the ListBoxPanVMs,
            //WHICH is a pain in the A!
            //ACTUALLY no it does not!
            //IF you just make THIS VM take care of adding, and saving Db!
            //AND NOTE: there DOES have to be a difference between 
            //ListBoxPanelVM for Filters,
            //AND for entries.
            //BECAUSE one for ENTRIES, WILL add it to the dataBase!
            //Entry one could EXTEND the other one though...
            SelectableItems = new ObservableCollection<T>();
            SelectedItems = new ObservableCollection<T>();
            EntryVM = VM;
            MainVM = VM.TreeVM.ParentVM;
            DbContext = VM.DbContext;
            ChooseCorrectDbSet();
            //SUBSCRIBE
            InputConfirmed += This_InputConfirmed;
        }

        public ListBoxPanelVM(EntryVM VM, ObservableCollection<T> list) : this(VM) { //hopefully calls this(VM) first?
            //PERHAPS this should wrap a list of tags, in a VM?
            //AND maybe that is easier, than doing it in the calling method? yes is.
            SelectedItems = list;
        }

        public void addNewItem(object sender, string text) {
            T newItem = new T();
            //HEY WOW! THOUGHT it was a real problem that I could not pass arguments to contruct.
            //BUT IS IT? COuld you get away with setting properties afterwards? REVISIT
            newItem.Name = text;
            ChooseCorrectDbSet();
            DbSet.Add(newItem as T);
            DbContext.SaveChanges();
            addItem(sender, newItem);
            //IS this method even needed???
        }

        public void addItem(object sender, T selectedItem) {
            if (!EntryVM.Entry.Tags.Contains(selectedItem as Tag)) {
                EntryVM.Entry.Tags.Add(selectedItem as Tag);
                //AHAH! What if I pass it DbSet<T> dbSet??
                //REVISIT, here might be where you need to use expressions again!
                //OR maybe, all I need to do is save,
                //SINCE selected items should just be reference to Tags or whatever.
                DbContext.SaveChanges();
                //ParentVM.ParentVM.updateEntries();//bad, because deletes all entryVMs.
                (sender as AutoCompleteBox).Text = null;
            }
        }

        public void ChooseCorrectDbSet() {
            if (typeof(T) == typeof(Entry))
                DbSet = DbContext.Entries as DbSet<T>; 
            //WHAT? Why is this allowed? Maybe its allowed, but wont actually work in runtime?
            //AH MAYBE IT DOES WORK, if Ts actually Match!!! COOL! REVISIT
            if (typeof(T) == typeof(PartClass))
                DbSet = DbContext.Parts as DbSet<T>;
            if (typeof(T) == typeof(PartInstance))
                DbSet = DbContext.PartInstances as DbSet<T>;
            if (typeof(T) == typeof(Task))
                DbSet = DbContext.Tasks as DbSet<T>;
            if (typeof(T) == typeof(Tag))
                DbSet = DbContext.Tags as DbSet<T>;
        }

        public void NotifyInputConfirmed(string input) {
            if (InputConfirmed != null)
                InputConfirmed(this, new ObjectEventArgs<T>(input as T));
        }

        public void GoTo(object item) {
            MainVM.SelectedEntryVM = EntryVM.TreeVM.WrapInCorrectVM(item as T);
            //MainVM.UpdateEntries(); //PERHAPS don't want to do this...
            //SINCE it updates whole list!
        }

        public void Remove(object item) {
            SelectedItems.Remove(item as T);
            MainVM.UpdateEntries();
            //AGAIN perhaps dont want to do this,
            //SINCE it updates whole list? slow!
        }

        public void This_InputConfirmed(object sender, ObjectEventArgs<T> e) {
            T item = e.Object as T;
            if (!SelectableItems.Contains(item)) SelectableItems.Add(item);
            if (!SelectedItems.Contains(item)) {
                SelectedItems.Add(item);
                //FIRE an event... or not.... use NotifyPropChanged?
                //ACTUALLY, FOR better DISTRIBUTION,
                //BETTER if this class just calls TreeVM to update itself!
                DbContext.SaveChanges();
                MainVM.UpdateEntries();
            }
        }

    }
}
