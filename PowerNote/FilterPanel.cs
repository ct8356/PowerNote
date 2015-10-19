using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Migrations;
using System.Windows.Data;
using PowerNote.Models;
using PowerNote.ViewModels;

namespace PowerNote {
    public class FilterPanel : ListBoxPanel {
        Label title;
        MyContext context;
        public FilterPanelVM Filter { get; set; }

        public FilterPanel(MyContext context) {
            this.context = context;
            //PANEL
            Orientation = Orientation.Horizontal;
            title = new Label();
            title.Content = "Filter by tags:";
            Children.Add(title);
            //TAG LIST PANEL
            MyListBox myListBox = new MyListBox();
            DataContext = new FilterPanelVM();
            Filter = DataContext as FilterPanelVM;
            myListBox.ItemsSource = (DataContext as FilterPanelVM).SelectedObjects; //cbtl
            Children.Add(myListBox);
            MyAutoCompleteBox myAutoCompleteBox = new MyAutoCompleteBox();
            context.Tags.Load();
            myAutoCompleteBox.ItemsSource = context.Tags.Local;
            Children.Add(myAutoCompleteBox);
            myAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        //OK! might need to make a COMMON viewModel too, CBTL
        //THAT ALL things can bind too. (I guess that is what makes inheritance easier with XAML???)

    }
}
