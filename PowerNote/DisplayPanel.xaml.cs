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
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.ViewModels;
using PowerNote.Migrations;

namespace PowerNote {
    public partial class DisplayPanel : DockPanel {
        //public SortPanel SortPanel { get; set; }
        public EntriesTreeView EntriesTreeView { get; set; }
        public List<String> ColumnNames { get; set; }
        public MainVM MainVM { get; set; }

        public DisplayPanel() {
            InitializeComponent(); //NEED to do this first, so EntriesTreeView can reference OptionsPanel
            TypePanel.DisplayMemberPath = "Name";
            //ComboBox.ValueMemberPath = "Name";
            //PANEL
            //SORT PANEL
            //SortPanel = new SortPanel();
            //Children.Add(SortPanel);
            //SetDock(SortPanel, Dock.Top);
            //SortPanel.ComboBox.SelectionChanged += ComboBox_SelectionChanged; //subscribe
            //COLUMN NAME PANEL
            //var colNames = typeof(Student).GetProperties().Select(a => a.Name).ToList();
            ColumnNames = new List<String>() { "Contents", "Priority" };
            //OTHER
            LastChildFill = false;
        }

        public void ComboBox_SelectionChanged(object sender, RoutedEventArgs e) {
            //START a ORDER BY QUERY. I say, just call to UPDATE methods, BUT with NEW INPUT.
            ComboBox comboBox = sender as ComboBox;
            (DataContext as EntriesTreeVM).TypePanelVM.updateSelectedObject(comboBox.SelectedItem);
            updateEntries();
        }

        public void StructurePanel_SelectionChanged(object sender, EventArgs e) {
            ComboBox comboBox = sender as ComboBox;
            (DataContext as EntriesTreeVM).StructurePanelVM.updateSelectedObject(comboBox.SelectedItem);
            updateEntries();
        }

        public void updateEntries() {
            if (EntriesTreeView != null && EntriesTreeView.DataContext != null) {//REVISIT. HACK.
                (EntriesTreeView.DataContext as EntriesTreeVM).UpdateEntries();
            }
            //uc.addTo();
        }
    }
}

