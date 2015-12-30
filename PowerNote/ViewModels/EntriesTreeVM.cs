using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.ViewModels;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq.Expressions;

namespace PowerNote.ViewModels {
    public class EntriesTreeVM {
        public MainVM ParentVM { get; set; }
        public MyContext DbContext { get; set; }
        Type type;
        public TypePanelVM TypePanelVM { get; set; }
        public StructurePanelVM StructurePanelVM { get; set; }
        public FilterPanelVM Filter { get; set; }
        public OptionsPanelVM OptionsPanelVM { get; set; }
        public ObservableCollection<EntryVM> FirstGenEntryVMs { get; set; }
        //needed for treeview to bind to
        public ObservableCollection<EntryVM> AllEntryVMs { get; set; }
        //needed to keep track of EntryVMs shown in the treeView (without having to scan tree).
        int childLevel;
        public Entry Orphan { get; set; }
        public bool WaitingForParentSelection { get; set; }

        public EntriesTreeVM(MainVM parentVM) {
            ParentVM = parentVM;
            DbContext = parentVM.DbContext;
            TypePanelVM = parentVM.TypePanelVM;
            StructurePanelVM = parentVM.StructurePanelVM;
            Filter = parentVM.FilterPanelVM;
            OptionsPanelVM = parentVM.OptionsPanelVM;
            FirstGenEntryVMs = new ObservableCollection<EntryVM>();
            AllEntryVMs = new ObservableCollection<EntryVM>();
            filterSortAndShowEntries();
            //filterToDos();
            //INITIALIZE
            //newEntry.LostFocus += new RoutedEventHandler(newEntry_LostFocus);
            //newEntry.KeyUp += new KeyEventHandler(newEntry_KeyUp);
        }

        public void filterSortAndShowEntries() {
            if (TypePanelVM.SelectedObjects.Count > 0) {
                Type selectedType = TypePanelVM.SelectedObjects.First() as Type;
                if (selectedType == typeof(Entry))
                    processEntries<Entry>(DbContext.Entrys);
                if (selectedType == typeof(PartClass)) //is does not work here. it says selected type is Type.
                    processEntries<PartClass>(DbContext.Parts);
                if (selectedType == typeof(PartInstance))
                    processEntries<PartInstance>((DbContext.PartInstances));
                if (selectedType == typeof(Task))
                    processEntries<Task>(DbContext.ToDos);
            }
        }

        public IQueryable<T> filterEntries<T>(IQueryable<T> entries, Expression<Func<T, bool>> exp) {
            IQueryable<T> filteredEntries = entries.Where(exp);
            return filteredEntries;
        }

        //public IQueryable<T> filterByType<T>(IQueryable<Entry> entries) {
        //    IQueryable<T> filteredEntries = entries.Where(e => e.Type == typeof(T).FullName);
        //    return filteredEntries;
        //} //OMG! DUH! THERE IS NO NEED TO FILTER BY TYPE! YOU JUST QUERY THE RIGHT TABLE IN FIRST PLACE!

        public IEnumerable<Entry> filterByType(IQueryable<Entry> filteredEntries) {
            //string typeName = ((displayPanel.TypePanel.DataContext as TypePanelVM).SelectedObjects.First() as Type).FullName;
            //Type type = Type.GetType(typeName);
            Type type = TypePanelVM.SelectedObjects.First() as Type;
            IEnumerable<Entry> filteredEntriesEnum = filteredEntries.AsEnumerable<Entry>()
                .Where(e => e.GetType() == type);
            return filteredEntriesEnum;
        }//LATER if decide it takes too long to do several queries,
        //can turn this method into a "construct SQL" query, maybe...

        public IQueryable<T> filterByTag<T>(IQueryable<T> entries) where T : Entry {
            IEnumerable<int> filterCourseIDs = Filter.SelectedObjects.Select(o => (o as Tag).TagID);
            if (!OptionsPanelVM.ShowAllEntries)
                return entries.Where(s => !filterCourseIDs.Except(s.Tags.Select(c => c.TagID)).Any());
            //NOTE: can I think of EXCEPTS means EXCLUDES?
            //SO above means, show it if FILTERCOURSEIDs iNCLUDES it, includes EVERY one!...YES!
            //OR show it if it does NOT EXCLUDE ANY of them!! i.e. has to include ALL of them!
            //ALTHOUGH, not quite right... other way round. entryTAGS, has to include ALL of filterTags!
            //SO really saying, show it, ONLY if filterCourse does not ESCAPE ANY of them!
            //IF it ESCAPES any of them, (i.e. it has a Tag, that entry does NOT have) then don't show entry!
            else return entries;
        } //CBTL HERE LIES THE PROBLEM! returning null!

        //public void orderBy() {
        //    //ORDERBY
        //    switch (SortPanelVM.ComboBox.SelectedItem.ToString()) {
        //        case "ID": break;
        //        case "Priority": break;
        //    }
        //}

        public void processEntries<T>(IQueryable<T> entries) where T : Entry {
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

        //public void sortTasks() {
        //    switch (displayPanel.SortPanel.ComboBox.SelectedItem.ToString()) {
        //        case "ID":
        //            break;
        //        case "Priority":
        //            //filteredStudents = filteredStudents.OrderBy(s => s.Priority);
        //            break;
        //    }
        //}

        public void showLevels<T>(IQueryable<T> entries) where T : Entry {
            string columnName = null;
            if (StructurePanelVM.SelectedObjects.Count > 0 )
                columnName = StructurePanelVM.SelectedObjects.First() as string;
            //string columnName = "Sensor"; //HARDWIRE
            IQueryable<T> filteredParents = showFirstLevel(entries, columnName);
            Expression<Func<T, bool>> expression = PropertyNotNull<T>(columnName);
            for (int gen = 2; gen <= 5; gen++) {
                //filteredParents = showMoreLevels(filteredParents, child => child.Parent != null);
                filteredParents = showMoreLevels(entries, filteredParents, columnName, expression.Compile());
            }
        }

        public IQueryable<T> showFirstLevel<T>(IQueryable<T> entries, string columnName) where T : Entry {
            IEnumerable<int> filterTagIDs = Filter.Objects.Select(o => (o as Tag).TagID);
            IQueryable<T> filteredParents = null;
            Type selectedType = TypePanelVM.SelectedObjects.First() as Type;
            if (OptionsPanelVM.ShowAllEntries) {
                filteredParents = filterEntries<T>(entries, PropertyEqualsNull<T>(columnName));
                //PERHAPS, really DO want to do 3 filters, one for each type...
            }
            else {
                filteredParents = filterEntries<T>(entries, PropertyEqualsNull<T>(columnName));
                //filteredParents = entries.Where(e => filterTagIDs.Except(e.Parent.Tags.Select(t => t.TagID)).Any());
                //WHERE it has NO parent that matches the filter.
            }
            foreach (T entry in filteredParents) {
                EntryVM entryVM = wrapInCorrectVM(entry);
                FirstGenEntryVMs.Add(entryVM);
                AllEntryVMs.Add(entryVM);
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
                        EntryVM childVM = wrapInCorrectVM(child);
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

        public EntryVM wrapInCorrectVM(Entry entry) {
            EntryVM entryVM = null;
            if (entry is PartClass)
                entryVM = new PartClassVM(entry as PartClass, this);
            if (entry is PartInstance)
                entryVM = new PartInstanceVM(entry as PartInstance, this);
            if (entry is Task)
                entryVM = new TaskVM(entry as Task, this);
            return entryVM;
        } //AHAH! CURRENT! Really need to AVOID creating NEW vms here, or lose your VM metadata!

        //public void showUntaggedEntries() {
        //    IQueryable<Task> untaggedStudents = filteredStudents.Where(s => !s.Tags.Any()); //Will this work?
        //    foreach (Task student in untaggedStudents) {
        //    }
        //}

        public void updateEntries() {
            FirstGenEntryVMs.Clear();
            AllEntryVMs.Clear();
            filterSortAndShowEntries();
        }

        public void waitForParentSelection(Entry entry) {
            WaitingForParentSelection = true;
            Orphan = entry;
            updateEntries();
            //this will update the treeView, AND if this is true,
            //it will be redrawn with certain colour scheme, to make the waiting, obvious.
            //drag and drop won't be too hard, BUT start here. easier.
            //ALSO, WHEN TRUE, need to create a LISTENER,
            //THAT subscribes now to the itemClick event.
            //and WHEN fires,
            //it will call that PANELs dataContext, and its method adoptChild(orphan);
            //WILL JUST HAVE TO UNSUBSCRIBE! It is poss. -=...
        }

    }
}
