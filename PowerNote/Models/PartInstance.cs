using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PowerNote.Models {
    public class PartInstance : Entry {
        string functionText;
        public string FunctionText {
            get { return functionText; }
            set { functionText = value; NotifyPropertyChanged("FunctionText"); }
        }
        public String MyString { get { return "my string"; } set { value = value; } }
        public PartClass PartClass { get; set; }
        public virtual PartInstance ParentPartInstance { get; set; }
        public virtual ObservableCollection<PartInstance> ChildPartInstances { get; set; }      
        public virtual ObservableCollection<Task> ChildTasks { get; set; }
        
        public PartInstance()
            : base() {
        }//FOR some bizzare reason, AddOrUpdate method requires this...
        //even though, it is never used.

        public PartInstance(string functionText)
            : base() {
                this.functionText = functionText;
        }

        public override string ToString() {
            return functionText;
        }
    }
}
