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
using System.ComponentModel;
using PowerNote.Models;
using PowerNote.ViewModels;
using System.Collections.ObjectModel;

namespace PowerNote {
    public partial class MyListBox : ListBox {
        public MyListBox() {
            InitializeComponent();
            Items.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Ascending)); //"" is for property name.
            ContextMenu = new ContextMenu();
            MenuItem delete_menuItem = new MenuItem();
            ContextMenu.Items.Add(delete_menuItem); //PROBLEM! menu belongs to listBox, not ITEM!
            delete_menuItem.Click += delete_menuItem_Click;
            delete_menuItem.Header = "Remove item";
        }

        public void delete_menuItem_Click(Object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem != null) {
                if (SelectedIndex == -1) return;
                //IF this listBox is bound to student.courses, will deleting an item, delete the course from student?
                //MAYBE, BUT still, it probably would NOT do a SAVECHANGES...
                //MAybe could add a LISTENER for this though...
                object selectedItem = Items[SelectedIndex] as object;
                if (selectedItem != null) {
                    //ObservableCollection<object> myList = (ItemsSource as ObservableCollection<object>);
                    //myList.Remove(selectedItem);
                    //maybe object is just TOO generic to work? Not sure why though.
                    //JUST going to have to delete from the ACTUAL SOURCE.
                    //WHICH IS, SelectedObjects! (needs to be this generic one, if want THIS to be generic).
                    (DataContext as ListBoxPanelVM).SelectedObjects.Remove(selectedItem);
                    (DataContext as ListBoxPanelVM).ParentVM.updateEntries(); //lazy?
                }
                //MAYBE fire an event here, so that ENTRYPANEL, or TREEVIEW knows to save changes?
                //MAYBE an event is already fired by this, you just need to LISTEN for it?
                //context.SaveChanges(); //ALSO lazy. CBTL.
                //mainPanel.updateEntries();
            } //CBTL! CURRENT! have to call a method here, to refresh entries, when delete clicked.
        }
    }
}
