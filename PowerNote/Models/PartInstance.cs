using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

using System.ComponentModel;
namespace PowerNote.Models {
    public class PartInstance : Entry {
       
        string functionText;
        public string FunctionText {
            get { return functionText; }
            set { functionText = value; NotifyPropertyChanged("FunctionText"); }
        }
        [Display(Name = "My String Display Name")]
        public String MyString { get { return "my string"; } set { value = value; } }
        public virtual PartClass PartClass { get; set; }
        public virtual PartInstance ParentPartInstance { get; set; }
        //virtual is there to take advantage of lazy loading
        //Navigation properties HAVE to be marked virtual.
        public virtual ObservableCollection<PartInstance> ChildPartInstances { get; set; }      
        public virtual ObservableCollection<Task> ChildTasks { get; set; }
        [InverseProperty("Sensor")]
        public virtual ObservableCollection<PartInstance> SensedParts { get; set; }
        [InverseProperty("SensedParts")]
        public virtual PartInstance Sensor { get; set; }
        //NOTE: A PartInstance, CAN be an EXAMPLE partInstance. i.e. imaginary.
        //Does not actually have to exist. //THIS way, is bit more work, but more flexible, I think.

        public PartInstance()
            : base() {
            Type = "PowerNote.Models.PartInstance";
            SensedParts = new ObservableCollection<PartInstance>();
        }//FOR some bizzare reason, AddOrUpdate method requires this...
        //even though, it is never used.

        public PartInstance(string functionText)
            : this() {
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
