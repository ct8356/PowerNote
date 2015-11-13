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
        Label title;
        //public TypePanel TypePanel { get; set; }
        //public StructurePanel StructurePanel { get; set; }
        //public FilterPanel FilterPanel { get; set; }
        //public SortPanel SortPanel { get; set; }
        //public OptionsPanel OptionsPanel { get; set; }
        public EntriesTreeView EntriesTreeView { get; set; }
        public List<String> ColumnNames { get; set; }
        public MainVM MainVM { get; set; }

        public DisplayPanel() {
            InitializeComponent(); //NEED to do this first, so EntriesTreeView can reference OptionsPanel
            //PANEL
            //Filter by TYPE
            //TypePanel = new TypePanel();
            //Children.Add(TypePanel);
            //SetDock(TypePanel, Dock.Top);
            //STRUCTURE PANEL
            //StructurePanel = new StructurePanel();
            //Children.Add(StructurePanel);
            //SetDock(StructurePanel, Dock.Top);
            //FILTER PANEL
            //FilterPanel = new FilterPanel();
            //Children.Add(FilterPanel);
            //SetDock(FilterPanel, Dock.Top);
            //SORT PANEL
            //SortPanel = new SortPanel();
            //Children.Add(SortPanel);
            //SetDock(SortPanel, Dock.Top);
            //SortPanel.ComboBox.SelectionChanged += ComboBox_SelectionChanged; //subscribe
            //OPTIONS PANEL
            //OptionsPanel = new OptionsPanel();
            //Children.Add(OptionsPanel);
            //SetDock(OptionsPanel, Dock.Top);
            //ENTRIES TREEVIEW
            EntriesTreeView = new EntriesTreeView(this);
            Children.Add(EntriesTreeView);
            SetDock(EntriesTreeView, Dock.Top);
            //COLUMN NAME PANEL
            //var colNames = typeof(Student).GetProperties().Select(a => a.Name).ToList();
            ColumnNames = new List<String>() { "Contents", "Priority" };
            //OTHER
            LastChildFill = false;

        }

        public void ComboBox_SelectionChanged(object sender, RoutedEventArgs e) {
            //START a ORDER BY QUERY. I say, just call to UPDATE methods, BUT with NEW INPUT.
            updateEntries();
        }

        public void updateEntries() {
            (EntriesTreeView.DataContext as EntriesTreeVM).updateEntries();
            //uc.addTo();
        }
    }
}

