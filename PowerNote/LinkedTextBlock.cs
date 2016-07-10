using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CJT;
using PowerNote.ViewModels;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Data;
using TextBlock = CJT.TextBlock;

namespace PowerNote {
    public class LinkedTextBlock : TextBlock {
        public MainVM MainVM { get; set; }

        public LinkedTextBlock() {
            Loaded += This_Loaded;
        }

        public void This_Loaded(object sender, EventArgs e) {
            EntryVM dataContext = DataContext as EntryVM; //NEED to set this OUTSIDE it!
            //REVISIT CURRENT!
            //IDEALLY would do this in XAML!
            //PERHAPS with a DataTemplate or ControlTemplate????
            //i.e. set the DataTemplate to a XAML LinkedTextBlock which has
            //DataContext = Entry.Parent NO! would not work! AND using string anyway,
            //SO may as well use reflection! Surely that is what XAML is anyway?
            //ALL bindings use strings, SO surely that is reflection?
            //OR perhaps easier, can access it through the BINDING?
            MenuItem goToEntry = new MenuItem() { Header = "Go to entry" };
            goToEntry.Click += delegate { MainVM.SelectedEntryVM = dataContext; };//USE polymorphism!
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.Items.Add(goToEntry);
            ContextMenu = contextMenu;
        }

    }
}
