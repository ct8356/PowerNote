using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel;
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
        [InverseProperty("Sensors")]
        public virtual ObservableCollection<PartClass> SensedParts { get; set; }
        public PartInstance()
            : base() {
        }//FOR some bizzare reason, AddOrUpdate method requires this...
        //even though, it is never used.

        public PartInstance(string functionText)
            : base() {
            FunctionText = functionText;
        }

        public PartInstance(string functionText, PartClass partClass, ObservableCollection<Tag> tags) 
            : this (functionText) {
            PartClass = partClass;
            Tags = tags;
        }

        public override string ToString() {
            return functionText;
        }
    }
}
