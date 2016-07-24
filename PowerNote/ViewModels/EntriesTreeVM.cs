using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Migrations;
using PowerNote.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CJT;
using System.Reflection;
using System.Linq.Expressions;
using AutoCompleteBox = CJT.AutoCompleteBox;

namespace PowerNote.ViewModels {
    public class EntriesTreeVM {
        public MainVM ParentVM { get; set; }
        public DAL.DbContext DbContext { get; set; }
        Type type;
        public EntryVM FilterEntryVM { get; set; }
        public ComboBoxVM TypePanelVM { get; set; }
        public ComboBoxVM StructurePanelVM { get; set; }
        public ListBoxPanelVM<Tag> FilterPanelVM { get; set; }
        public OptionsPanelVM OptionsPanelVM { get; set; }
        public ObservableCollection<EntryVM> FirstGenEntryVMs { get; set; }
        //needed for treeview to bind to
        public ObservableCollection<EntryVM> AllEntryVMs { get; set; }
        //needed to keep track of EntryVMs shown in the treeView (without having to scan tree).
        public ObservableCollection<Tag> AllTags { get; set; }
        int childLevel;
        public Entry Orphan { get; set; }
        public bool WaitingForParentSelection { get; set; }

        public EntriesTreeVM(MainVM parentVM) {
            ParentVM = parentVM;
            DbContext = parentVM.DbContext;
            //New stuff
            FilterEntryVM = new EntryVM(this);
            TypePanelVM = new ComboBoxVM(parentVM);
            TypePanelVM.Objects = new ObservableCollection<object> { typeof(Entry), typeof(PartClass), typeof(PartInstance), typeof(Task), typeof(Tag) };
            TypePanelVM.SelectedObject = TypePanelVM.Objects.First();
            StructurePanelVM = new ComboBoxVM(parentVM);
            StructurePanelVM.Objects = new ObservableCollection<object>() { "Parent", "Sensor" };
            StructurePanelVM.SelectedObject = StructurePanelVM.Objects.First();
            AllTags = new ObservableCollection<Tag>();
            DbContext.Tags.Load();
            foreach (Tag tag in DbContext.Tags.Local) {
                AllTags.Add(tag);
            }
            FilterPanelVM = new ListBoxPanelVM<Tag>(FilterEntryVM); //NOTE does not seem to have anything in its lists REVISIT CURRENT
            FilterPanelVM.SelectableItems = new ObservableCollection<Tag>(AllTags);
            OptionsPanelVM = new OptionsPanelVM(parentVM);
            //OLD SUSTFF
            FirstGenEntryVMs = new ObservableCollection<EntryVM>();
            AllEntryVMs = new ObservableCollection<EntryVM>();
            filterSortAndShowEntries();
            //filterToDos();
            //INITIALIZE
            //newEntry.LostFocus += new RoutedEventHandler(newEntry_LostFocus);
            //newEntry.KeyUp += new KeyEventHandler(newEntry_KeyUp);
        }

        public void filterSortAndShowEntries() {
            if (TypePanelVM.SelectedObject != null) {
                Type selectedType = TypePanelVM.SelectedObject as Type; //REVISIT CURRENT here is problem!
                if (selectedType == typeof(Entry))
                    processEntries<Entry>(DbContext.Entries);
                if (selectedType == typeof(PartClass)) //is does not work here. it says selected type is Type.
                    processEntries<PartClass>(DbContext.Parts);
                if (selectedType == typeof(PartInstance))
                    processEntries<PartInstance>((DbContext.PartInstances));
                if (selectedType == typeof(Task))
                    processEntries<Task>(DbContext.Tasks);
                if (selectedType == typeof(Tag))
                    processEntries<Tag>(DbContext.Tags);
            }
        }

        protected IQueryable<T> filterEntries<T>(IQueryable<T> entries, Expression<Func<T, bool>> exp) {
            IQueryable<T> filteredEntries = entries.Where(exp);
            return filteredEntries;
        }

        public IEnumerable<Entry> filterByType(IQueryable<Entry> filteredEntries) {
            //string typeName = ((displayPanel.TypePanel.DataContext as TypePanelVM).SelectedObjects.First() as Type).FullName;
            //Type type = Type.GetType(typeName);
            Type type = TypePanelVM.SelectedObject as Type;
            IEnumerable<Entry> filteredEntriesEnum = filteredEntries.AsEnumerable<Entry>()
                .Where(e => e.GetType() == type);
            return filteredEntriesEnum;
        }//LATER if decide it takes too long to do several queries,
        //can turn this method into a "construct SQL" query, maybe...
  
        public IQueryable<T> filterByTag<T>(IQueryable<T> entries) where T : Entry {
            IEnumerable<int> filterTagIDs = FilterPanelVM.SelectedItems.Select(o => (o as Tag).EntryID);
            if (!OptionsPanelVM.ShowAllEntries)
                return entries.Where(e => !filterTagIDs.Except(e.Tags.Select(t => t.EntryID)).Any());  
            //entryTAGS, has to include ALL of filterTags!
            //SO really saying, show it, ONLY if filterTags does not ESCAPE ANY of them!
            //IF it ESCAPES any of them, (i.e. it has a Tag, that entry does NOT have) then don't show entry!
            else return entries;
        } //CBTL HERE LIES THE PROBLEM! returning null!

        public void processEntries<T>(IQueryable<T> entries) where T : Entry {
            //IQueryable<T> filteredEntries = entries;//REVISIT HACK!
            IQueryable<T> filteredEntries = filterByTag<T>(entries);
            //orderBy(); //does nothing yet
            showLevels<T>(filteredEntries);
        }

        private Expression<Func<T, bool>> EnumContainsPropertyEntryID<T>(IEnumerable<int> enumerable, string propertyName) {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression member = Expression.Property(parameter, propertyName);
            MemberExpression member2 = Expression.Property(member, "EntryID");
            MethodInfo method = typeof(Enumerable).GetMethods().
                    Where(x => x.Name == "Contains").
                    Single(x => x.GetParameters().Length == 2).
                    MakeGenericMethod(typeof(int));
            ConstantExpression constant = Expression.Constant(enumerable, typeof(IEnumerable<int>));
            Expression exp = Expression.Call(method, constant, member2);
            return Expression.Lambda<Func<T, bool>>(exp, parameter);
        }

        private Expression<Func<EntryVM, bool>> EntryEqualsChildsProperty<T>(T child, string propertyName) {
            //Get entry
            ParameterExpression entryVM = Expression.Parameter(typeof(EntryVM));
            MemberExpression entry = Expression.Property(entryVM, "Entry");
            //Get constant
            PropertyInfo property = child.GetType().GetProperty(propertyName);
            ConstantExpression constant = Expression.Constant(property.GetValue(child, null), typeof(Entry));
            //and the rest
            Expression exp = Expression.Equal(entry, constant);
            return Expression.Lambda<Func<EntryVM, bool>>(exp, entryVM);
        }

        private Expression<Func<T, bool>> PropertyEqualsNull<T>(string propertyName) {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            //NOTE: using reflection below, SO may as well use it above!
            MemberExpression member = Expression.Property(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(null);
            Expression exp = Expression.Equal(member, constant);
            return Expression.Lambda<Func<T, bool>>(exp, parameter);
        }

        private Expression<Func<T, bool>> PropertyNotNull<T>(string propertyName) {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression member = Expression.Property(parameter, propertyName); //propName sensor is not defined for entry!
            ConstantExpression constant = Expression.Constant(null);
            Expression exp = Expression.NotEqual(member, constant);
            return Expression.Lambda<Func<T, bool>>(exp, parameter);
        }

        public void showLevels<T>(IQueryable<T> entries) where T : Entry {
            string columnName = null;
            //if (StructurePanelVM.SelectedObjects.Count > 0 )
                columnName = StructurePanelVM.SelectedObject as string;
            //string columnName = "Sensor"; //HARDWIRE
            IQueryable<T> filteredParents = showFirstLevel<T>(entries, columnName);
            Expression<Func<T, bool>> expression = PropertyNotNull<T>(columnName);
            for (int gen = 2; gen <= 5; gen++) {
                //filteredParents = showMoreLevels(filteredParents, child => child.Parent != null);
                filteredParents = showMoreLevels<T>(entries, filteredParents, columnName, expression.Compile());
            }
        }

        public IQueryable<T> showFirstLevel<T>(IQueryable<T> entries, string columnName) where T : Entry {
            //IEnumerable<int> filterTagIDs = Filter.SelectedObjects.Select(o => (o as Tag).TagID);
            IQueryable<T> filteredParents = null;
            Type selectedType = TypePanelVM.SelectedObject as Type;
            if (OptionsPanelVM.ShowAllEntries) {
                filteredParents = filterEntries<T>(entries, PropertyEqualsNull<T>(columnName));
                //PERHAPS, really DO want to do 3 filters, one for each type...
            }
            else {
                filteredParents = filterEntries<T>(entries, PropertyEqualsNull<T>(columnName));
                //filteredParents = entries.Where(e => filterTagIDs.Except(e.Parent.Tags.Select(t => t.TagID)).Any());
                //WHERE it has NO parent that matches the filter.
            }
            int count0 = entries.Count();
            int count = filteredParents.Count();
            foreach (T entry in filteredParents) {
                EntryVM entryVM = WrapInCorrectVM(entry); //SHOULD make this into Constructor really.
                FirstGenEntryVMs.Add(entryVM);
                AllEntryVMs.Add(entryVM); //CURRENT PROBLEM! Code does not reach this line! REVISIT!
            }
            return filteredParents;
        }

        public IQueryable<T> showMoreLevels<T>(IQueryable<T> entries, IQueryable<T> filteredParents,
            string columnName, Func<T, bool> hasParent) where T : Entry {
            childLevel++;
            IQueryable<T> filteredChildren = null;
            IEnumerable<int> parentEntryIDs = filteredParents.Select(e => e.EntryID);
            //filteredChildren = entries.Where(e => parentEntryIDs.Contains(e.Sensor.EntryID));
            filteredChildren = entries.Where(EnumContainsPropertyEntryID<T>(parentEntryIDs, columnName));
            //NOTE! innefficiency above, in that it does this for parents too...
            //perhaps in previous cycle, could REMOVE parents. once placed. BUT then they 
            //only show once. MIGHT not want that. 
            foreach (T child in filteredChildren) {
                if (hasParent(child)) { //if child has a parent
                    //BUT surely BOUND to have a parent!??? if filtered children???
                    try {
                        //EntryVM parentVM = AllEntryVMs.Where(eVM => eVM.Entry == child.Sensor).Single();
                        EntryVM parentVM = AllEntryVMs.Where(EntryEqualsChildsProperty<T>(child, columnName).Compile()).First();
                        //OH YEAH! Queryables can take expressions, Enumerables must take delegates!
                        EntryVM childVM = parentVM.TreeVM.WrapInCorrectVM(child);
                        parentVM.Children.Add(childVM);
                        childVM.Parent = parentVM;
                        AllEntryVMs.Add(childVM);
                    }
                    catch (Exception e) {
                        //Do nothing
                    }
                }
            }
            return filteredChildren;
        }

        //public void showUntaggedEntries() {
        //    IQueryable<Task> untaggedStudents = filteredStudents.Where(s => !s.Tags.Any()); //Will this work?
        //    foreach (Task student in untaggedStudents) {
        //    }
        //}

        public void UpdateEntries() {
            FirstGenEntryVMs.Clear();
            AllEntryVMs.Clear();
            filterSortAndShowEntries();
        }

        public void waitForParentSelection(Entry entry) {
            WaitingForParentSelection = true;
            Orphan = entry;
            UpdateEntries();
            //this will update the treeView, AND if this is true,
            //it will be redrawn with certain colour scheme, to make the waiting, obvious.
            //drag and drop won't be too hard, BUT start here. easier.
            //ALSO, WHEN TRUE, need to create a LISTENER,
            //THAT subscribes now to the itemClick event.
            //and WHEN fires,
            //it will call that PANELs dataContext, and its method adoptChild(orphan);
            //WILL JUST HAVE TO UNSUBSCRIBE! It is poss. -=...
        }

        public EntryVM WrapInCorrectVM(Entry entry) {
            EntryVM entryVM = null;
            if (entry is PartClass)
                entryVM = new PartClassVM(entry as PartClass, this);
            if (entry is PartInstance)
                entryVM = new PartInstanceVM(entry as PartInstance, this);
            if (entry is Task)
                entryVM = new TaskVM(entry as Task, this);
            if (entry is Tag)
                entryVM = new TagVM(entry as Tag, this);
            return entryVM;
        }

    }
}
