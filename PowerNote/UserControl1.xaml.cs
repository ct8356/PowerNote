using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace PowerNote {
    public partial class UserControl1 : UserControl {
        public ObservableCollection<String> stringList { get; set; }

        public UserControl1() {
            stringList = new ObservableCollection<String>() { "one", "one" };
            InitializeComponent(); //OK! SO ORDER OF THIS is really important! the ViewModel,
            //if referenced by the XAML, needs to exist BEFORE this is called!
            //myTextBox = new TextBox(); //AHAH! So when I make NEW one (a new treeview), it messes things up!
            //myTextBox.Text = "yoyo";
        }

        public void addTo() {
            stringList.Add("add");
        }

    }
}
