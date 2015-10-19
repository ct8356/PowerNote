using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.ViewModels;
using PowerNote.Migrations;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;

namespace PowerNote {
    partial class EntriesTreeView : TreeView {
        Type type;
        DisplayPanel displayPanel;
        FilterPanelVM filter;
        //IQueryable<Entry> filteredEntries;
        //IQueryable<Task> filteredStudents;
        public ObservableCollection<EntryVM> FirstGenEntryVMs { get; set; } 
        //needed for treeview to bind to
        public ObservableCollection<EntryVM> AllEntryVMs { get; set; }
        //needed to keep track of EntryVMs shown in the treeView (without having to scan tree).
        MyContext context;
        ToDoPanel newEntry; //keep for now
        int childLevel;
        public Entry Orphan {get; set;}
        public bool WaitingForParentSelection { get; set; }

        public EntriesTreeView() {
            //DO nought.
        }

        public EntriesTreeView(MyContext context, DisplayPanel displayPanel) {
            this.context = context;
            this.displayPanel = displayPanel;
            FirstGenEntryVMs = new ObservableCollection<EntryVM>();
            AllEntryVMs = new ObservableCollection<EntryVM>();
            filterSortAndShowEntries();
            //filterToDos();
            //SUBSCRIBE
            displayPanel.OptionsPanel.ShowAllEntriesCBox.Click += ShowAllEntries_Click; //subscribe
            displayPanel.OptionsPanel.ShowAllChildrenCBox.Click += ShowAllChildren_Click; //subscribe
            //INITIALIZE
            InitializeComponent();
            int count = FirstGenEntryVMs.Count();
            //newEntry.LostFocus += new RoutedEventHandler(newEntry_LostFocus);
            //newEntry.KeyUp += new KeyEventHandler(newEntry_KeyUp);
        }

        public void filterSortAndShowEntries() {
            Type selectedType = (displayPanel.TypePanel.DataContext as TypePanelVM).SelectedObjects.First() as Type;
            if (selectedType == typeof(Entry)) ;
            //processEntries<Entry>(context.Entrys);
            if (selectedType == typeof(PartClass)) ;//is does not work here. it says selected type is Type.
                //processEntries<PartClass>(context.Parts);
            if (selectedType == typeof(PartInstance))
                processEntries<PartInstance>(context.PartInstances);
            if (selectedType == typeof(Task)) ;
                //processEntries<Task>(context.ToDos);
            //SO filteredEntries is filled. BUT can it be converted back to parts?
            //I IMAGINE, not EASILY!
            //NO! It certainly is not a simple cast. SO lets try MORE generic methods...
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
            Type type = (displayPanel.TypePanel.DataContext as TypePanelVM).SelectedObjects.First() as Type;
            IEnumerable<Entry> filteredEntriesEnum = filteredEntries.AsEnumerable<Entry>()
                .Where(e => e.GetType() == type);
            return filteredEntriesEnum;
        }//LATER if decide it takes too long to do several queries,
        //can turn this method into a "construct SQL" query, maybe...

        public IQueryable<T> filterByTag<T>(IQueryable<T> entries) where T : Entry {
            filter = displayPanel.FilterPanel.DataContext as FilterPanelVM;
            IEnumerable<int> filterCourseIDs = filter.Objects.Select(o => (o as Tag).TagID);
            if (!displayPanel.OptionsPanel.ShowAllEntries)
                return entries.Where(s => !filterCourseIDs.Except(s.Tags.Select(c => c.TagID)).Any());
            else return null;
        }

        public void orderBy() {
            //ORDERBY
            switch (displayPanel.SortPanel.ComboBox.SelectedItem.ToString()) {
                case "ID": break;
                case "Priority": break;
            }
        }

        public void processEntries<T>(IQueryable<T> entries) where T : PartInstance {
            filterByTag<T>(entries);
            //orderBy(); //does nothing yet
            showLevels<T>(entries);
        }

        public void newEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                newEntry_LostFocus(sender, e);
            }
        }

        public void newEntry_LostFocus(object sender, System.Windows.RoutedEventArgs e) {
            if (newEntry.TextBox.Text != null && newEntry.TextBox.Text != "") {
                Task newStudent = new Task(newEntry.TextBox.Text);
                context.ToDos.Add(newStudent);
                context.SaveChanges();
                updateEntries(); //CBTL. Lazy way. (rather than using events). But ok for now.   
            }
        }

        public void ShowAllChildren_Click(object sender, EventArgs e) {
            updateEntries();
        }

        public void ShowAllEntries_Click(object sender, EventArgs e) {
            updateEntries();
        }

        private Expression<Func<T, bool>> EnumContainsPropertyEntryID<T>(IEnumerable<int> enumerable, string propertyName) {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression member = Expression.Property(parameter, propertyName);
            MemberExpression member2 = Expression.Property(member, "EntryID");
            //MethodInfo method = typeof(Enumerable).GetMethod("Contains", new Type[] { typeof(int) });
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
            //ParameterExpression childParam = Expression.Parameter(typeof(T));
            //MemberExpression property = Expression.Property(childParam, propertyName);
            PropertyInfo property = child.GetType().GetProperty(propertyName);
            //ConstantExpression constant = Expression.Constant(property, typeof(Entry));
            ConstantExpression constant = Expression.Constant(property.GetValue(child, null), typeof(Entry));
            //YES! THIS WORKS!
            //and the rest
            Expression exp = Expression.Equal(entry, constant);
            return Expression.Lambda<Func<EntryVM, bool>>(exp, entryVM);
        } //NOT QUITE working. CBTL. CURRENT.

        private Expression<Func<T, bool>> PropertyEqualsNull<T>(string propertyName) {
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            //NOTE: using reflection below, SO may as well use it above!
            MemberExpression member = Expression.Property(parameter, propertyName);
            ConstantExpression constant = Expression.Constant(null);
            Expression exp = Expression.Equal(member, constant);
            return Expression.Lambda<Func<T, bool>>(exp, parameter);
        }

        private Expression<Func<T, bool>> PropertyNotNull<T>(string propertyName) {
            //type = (displayPanel.TypePanel.DataContext as TypePanelVM).SelectedObjects.First() as Type;
            ParameterExpression parameter = Expression.Parameter(typeof(T));
            MemberExpression member = Expression.Property(parameter, propertyName); //propName sensor is not defined for entry!
            ConstantExpression constant = Expression.Constant(null);
            Expression exp = Expression.NotEqual(member, constant);
            //parameter = Expression.Convert(parameter, typeof(Entry));
            //sure the above won't help anyway.
            return Expression.Lambda<Func<T, bool>>(exp, parameter);
        } //only way to make this generic, is to use ENTRY!
          //SO just have to somehow AVOID sending it anything other than entrys.
          //THAT WON'T be any good! MUST have to have ONE OF THESE, for each type.
          //SO, need a PartInstanceTypeVM, (or do it in static PartInstanceVM, would be fine if works, and easier),
          //AND put a method like this in each... BUT that is Copy and Paste! Bad!
          //BUT surely no other way... if PropertyName "sensor" is not defined for entry.

        public void sortTasks() {
            switch (displayPanel.SortPanel.ComboBox.SelectedItem.ToString()) {
                case "ID":
                    break;
                case "Priority":
                    //filteredStudents = filteredStudents.OrderBy(s => s.Priority);
                    break;
            }
        }

        public void showLevels<T>(IQueryable<T> entries) where T : PartInstance {
            //string columnName = (displayPanel.StructurePanel.DataContext as StructurePanelVM).SelectedObjects.First() as string;
            string columnName = "Sensor"; //HARDWIRE
            IQueryable<T> filteredParents = showFirstLevel(entries, columnName);
            Expression<Func<T, bool>> expression = PropertyNotNull<T>(columnName);
            for (int gen = 2; gen <= 5; gen++) {
                //filteredParents = showMoreLevels(filteredParents, child => child.Parent != null);
                filteredParents = showMoreLevels(entries, filteredParents, columnName, expression.Compile());
            }
        }

        public IQueryable<T> showFirstLevel<T>(IQueryable<T> entries, string columnName) where T : Entry {
            IEnumerable<int> filterTagIDs = filter.Objects.Select(o => (o as Tag).TagID);
            IQueryable<T> filteredParents = null;
            Type selectedType = (displayPanel.TypePanel.DataContext as TypePanelVM).SelectedObjects.First() as Type;
            if (displayPanel.OptionsPanel.ShowAllEntries) {
                filteredParents = filterEntries<T>(entries, PropertyEqualsNull<T>(columnName));
                //PERHAPS, really DO want to do 3 filters, one for each type...
                //HELL, may as well just DO it for now... Can change later.
                //NOTE: the SENSOR is NOT getting into filteredParents. It SHOULD!!
            }
            else {
                filteredParents = entries.Where(e => filterTagIDs.Except(e.Parent.Tags.Select(t => t.TagID)).Any());
                //WHERE it has NO parent that matches the filter.
            }
            foreach (T entry in filteredParents) {
                //CBTL: NEED to do a TYPE check here!!!
                EntryVM entryVM = wrapInCorrectVM(entry);
                FirstGenEntryVMs.Add(entryVM);
                AllEntryVMs.Add(entryVM);
                //PART PRESENT should have 1 sensed part. Does it?
                //YES it does!
                //THIS IS GREAT, actually! SENSORS come first.
            } 
            return filteredParents;
        }

        public IQueryable<T> showMoreLevels<T>(IQueryable<T> entries, IQueryable<T> filteredParents, 
            string columnName, Func<T, bool> hasParent) where T : PartInstance {
            childLevel++;
            IQueryable<T> filteredChildren = null;
            IEnumerable<int> parentEntryIDs = filteredParents.Select(e => e.EntryID);
            //filteredChildren = entries.Where(e => parentEntryIDs.Contains(e.Sensor.EntryID));
            filteredChildren = entries.Where(EnumContainsPropertyEntryID<T>(parentEntryIDs, columnName));
            //CBTL! HOPEFULLY (CURRENT) just got to figure out how to do above with Expression!!!
            //NOTE! innefficiency above, in that it does this for parents too...
            //perhaps in previous cycle, could REMOVE parents. once placed. BUT then they 
            //only show once. MIGHT not want that. 
            foreach (T child in filteredChildren) {
                if (hasParent(child)) { //if child has a parent
                    //BUT surely BOUND to have a parent!??? if filtered children???
                    try {
                        //EntryVM parentVM = AllEntryVMs.Where(eVM => eVM.Entry == child.Sensor).Single();
                        EntryVM parentVM = AllEntryVMs.Where(EntryEqualsChildsProperty<T>(child, columnName).Compile()).First();
                        //OH YEAH! I remember! queryables can take expressions, Enumerables must take delegates!
                        //HERE WAS the bit you missed!
                        EntryVM entryVM = wrapInCorrectVM(child);
                        parentVM.Children.Add(entryVM);
                        AllEntryVMs.Add(entryVM);
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
                entryVM = new PartClassVM(entry as PartClass, displayPanel.MainPanel);
            if (entry is PartInstance)
                entryVM = new PartInstanceVM(entry as PartInstance, displayPanel.MainPanel);
            if (entry is Task)
                entryVM = new TaskVM(entry as Task, displayPanel.MainPanel);
            return entryVM;
        }

        //public void showUntaggedEntries() {
        //    IQueryable<Task> untaggedStudents = filteredStudents.Where(s => !s.Tags.Any()); //Will this work?
        //    foreach (Task student in untaggedStudents) {
        //        //EntryNode entryNode = new EntryNode(student, displayPanel.MainPanel);
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
