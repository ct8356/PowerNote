using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PowerNote.Models;
using PowerNote.ViewModels;
using PowerNote.DAL;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace PowerNote {
    public partial class ToDoPanel : EntryPanel {

        public ToDoPanel() {
            InitializeComponent();
            //RIGHT CLICKS
            //Could do this in treeView. Does not really matter.
            //PROBS doing it in treeView means less instances...
            //AUTOCOMPLETEBOX
            MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

        public void insertTask_Click(Object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem != null) {
                (DataContext as TaskVM).insertTask(DataContext as TaskVM);
            }
        }

        public void insertSubTask_Click(Object sender, EventArgs e) {
            MenuItem menuItem = (MenuItem)sender;
            if (menuItem != null) {
                (DataContext as TaskVM).insertSubTask(DataContext as TaskVM);
            }
        }

    }
}
