using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.ViewModels;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq.Expressions;
using CJT;

namespace PowerNote {
    public partial class EntriesTreeView : TreeView {
        Type type;
        DisplayPanel DisplayPanel { get; set; }
        List<Tag> filter;
        EntryPanel newEntry; //keep for now
        public Entry Orphan {get; set;}
        public bool WaitingForParentSelection { get; set; }

        public EntriesTreeView(DisplayPanel displayPanel) {
            InitializeComponent();
            DisplayPanel = displayPanel;
            //DisplayPanel = Parent as DisplayPanel; //OBS won't work, since not added to children until AFTER its construction.
            //DataContext = (DataContext as MainVM).EntriesTreeVM; //SADLY, looks like DataContext not set until AFTER construction.
            //SUBSCRIBE
            DisplayPanel.OptionsPanel.ShowAllEntriesCBox.Click += ShowAllEntries_Click; //subscribe
            DisplayPanel.OptionsPanel.ShowAllChildrenCBox.Click += ShowAllChildren_Click; //subscribe
            //INITIALIZE
            
            //newEntry.LostFocus += new RoutedEventHandler(newEntry_LostFocus);
            //newEntry.KeyUp += new KeyEventHandler(newEntry_KeyUp);
        }

        public void newEntry_KeyUp(object sender, KeyEventArgs e) {
            if (e.Key == Key.Return) {
                newEntry_LostFocus(sender, e);
            }
        }

        public void newEntry_LostFocus(object sender, System.Windows.RoutedEventArgs e) {
            if (newEntry.TextBlock.Text != null && newEntry.TextBlock.Text != "") {
                Task newStudent = new Task(newEntry.TextBlock.Text);
                (DataContext as EntriesTreeVM).DbContext.Tasks.Add(newStudent);
                (DataContext as EntriesTreeVM).DbContext.SaveChanges();
                (DataContext as EntriesTreeVM).updateEntries(); //CBTL. Lazy way. (rather than using events). But ok for now.   
            }
        }

        public void ShowAllChildren_Click(object sender, EventArgs e) {
            (DataContext as EntriesTreeVM).updateEntries();
        }

        public void ShowAllEntries_Click(object sender, EventArgs e) {
            (DataContext as EntriesTreeVM).updateEntries();
        }

    }
}
