using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CJT;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using PowerNote.DAL;
using DbContext = PowerNote.DAL.DbContext;

namespace PowerNote.ViewModels {
    public class Property : BaseClass {
        //NOTE: probs easiest for MOMENT to use Property,
        //rather than dictionary of properties.
        //BECAUSE might find need to add MORE props to the property!
        //NOt sure yet.
        //MAYBE change later though, coz it is turning a bit heavy weight!
        //NOTE: does not have to hold Entries.
        //If know it has an entry, can just cast it later.
        public InfoType Type { get; set; }
        public string Name { get; set; }
        public bool IsVisibleInEntryPanel { get; set; }
        public bool IsVisibleInPropertyPanel { get; set; }
        public object Value { get; set; }
        public DbContext DbContext { get; set; }
        public Property() {
            //do nothing
        }

        public Property(string name, object value, InfoType type, bool isVisible, DbContext dbContext) {
            Name = name;
            Value = value;
            Type = type;
            IsVisibleInEntryPanel = isVisible;
            IsVisibleInPropertyPanel = true;
            DbContext = dbContext;
            PropertyChanged += This_PropertyChanged;
        }

        public void This_PropertyChanged(object sender, PropertyChangedEventArgs args) {
            DbContext.SaveChanges();
        } //HMMM! probs best to TRY this in separate project,
        //to see if its possible (binding to list entries i mean, dynamically!).
        //IN MEAN TIME, make good model, make genericvm with an entry,
        //make specific xaml templates that BIND to entry. NOT classes... ok, try it.

    }
}
