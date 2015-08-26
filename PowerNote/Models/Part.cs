using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;

namespace PowerNote.Models {

    public class Part : Entry {
        string nickName;
        public string NickName {
            get { return nickName; }
            set { nickName = value; NotifyPropertyChanged("Name"); }
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

        public Part() {
            Tags = new ObservableCollection<Tag>();
            CreationDate = DateTime.Parse("2010-09-01");
        }

        public Part(string nickName) : this() {
            this.nickName = nickName;
        }

    }
}
