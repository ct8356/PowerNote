using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;

namespace PowerNote.Models {

    public class PartClass : Entry {
        //PARENTS for this, should be INHERITANCE parents.
        //PARENTS for partInstance, should be LOCATION parents.
        //general PARENTs for Entry, should be, location parents.
        //WELL actually, I don't think they should exist.
        //WELL, maybe, for NOTE taking purposes.
        //WELL YES, I think, SHOULD be no GENERAL parent child relations.
        //ALL should have a SPECIFIC purpose.
        //SO should be NONE in Entry, unless can think of specific reason for it.
        //PARTS, SHOULD have ToDo, or NOTE children. thats how to relate them. NOT general children.
        string myString = "my String";
        public string MyString { get { return myString; } set { myString = value; } }
        string nickName;
        public string NickName {
            get { return nickName; }
            set { nickName = value; NotifyPropertyChanged("NickName"); }
        }

        string deviceType = "Undefined";
        public string DeviceType {
            get { return deviceType; }
            set { deviceType = value; NotifyPropertyChanged("DeviceType"); }
        }

        string manufacturer = "Undefined";
        public string Manufacturer {
            get { return manufacturer; }
            set { manufacturer = value; NotifyPropertyChanged("Manufacturer"); }
        }

        string orderNumber = "Undefined";
        public string OrderNumber {
            get { return orderNumber; }
            set { orderNumber = value; NotifyPropertyChanged("OrderNumber"); }
        }

        string description = "Undefined";
        public string Description {
            get { return description; }
            set {description = value; NotifyPropertyChanged("Description");}
        }
        public virtual ObservableCollection<PartInstance> PartInstances { get; set; }
        public virtual PartClass ParentPartClass { get; set; }
        public virtual ObservableCollection<PartClass> ChildPartClasses { get; set; }

        public PartClass() : base() {
            PartInstances = new ObservableCollection<PartInstance>();
        } //IF got it, have to assume people will use it.

        public PartClass(string nickName) : this() {
            this.nickName = nickName;
            
        }

        public override string ToString() {
            return NickName;
        } //THIS is not even called!

    }
}
