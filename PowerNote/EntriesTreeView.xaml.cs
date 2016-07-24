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
        DisplayPanel DisplayPanel { get; set; }
        public Entry Orphan { get; set; }
        public bool WaitingForParentSelection { get; set; }

        public EntriesTreeView() {
            InitializeComponent();
        }

        public void ShowAllChildren_Click(object sender, EventArgs e) {
            (DataContext as EntriesTreeVM).UpdateEntries();
        }

        public void ShowAllEntries_Click(object sender, EventArgs e) {
            (DataContext as EntriesTreeVM).UpdateEntries();
        }

    }
}
