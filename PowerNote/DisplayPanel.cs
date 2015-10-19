using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.Migrations;
using System.Windows.Data;
using System.Windows.Input;

namespace PowerNote {

    public class DisplayPanel : DockPanel {
            Label title;
            MyContext context;
            public TypePanel TypePanel { get; set; }
            public StructurePanel StructurePanel { get; set; }
            public FilterPanel FilterPanel { get; set; }
            public SortPanel SortPanel { get; set; }
            public OptionsPanel OptionsPanel { get; set; }
            public List<String> ColumnNames { get; set; }
            public EntriesTreeView EntriesView { get; set; }
            public MainPanel MainPanel { get; set; }

            public DisplayPanel(MyContext context, MainPanel mainPanel) {
                this.context = context;
                this.MainPanel = mainPanel;
                //PANEL
                title = new Label();
                title.Content = "Display panel";
                title.FontWeight = FontWeights.Bold;
                Children.Add(title);
                SetDock(title, Dock.Top);
                //Filter by TYPE
                TypePanel = new TypePanel(context, this);
                Children.Add(TypePanel);
                SetDock(TypePanel, Dock.Top);
                //TREE LAYOUT
                Label treeLayout = new Label();
                treeLayout.Content = "Children to show:";
                Children.Add(treeLayout);
                SetDock(treeLayout, Dock.Top);
                //STRUCTURE PANEL
                StructurePanel = new StructurePanel(context, this);
                Children.Add(StructurePanel);
                SetDock(StructurePanel, Dock.Top);
                //FILTER PANEL
                FilterPanel = new FilterPanel(context);
                Children.Add(FilterPanel);
                SetDock(FilterPanel, Dock.Top);
                //SORT PANEL
                SortPanel = new SortPanel(context);
                Children.Add(SortPanel);
                SetDock(SortPanel, Dock.Top);
                SortPanel.ComboBox.SelectionChanged += ComboBox_SelectionChanged; //subscribe
                //OPTIONS PANEL
                OptionsPanel = new OptionsPanel(context);
                Children.Add(OptionsPanel);
                SetDock(OptionsPanel, Dock.Top);
                //COLUMN NAME PANEL
                //var colNames = typeof(Student).GetProperties().Select(a => a.Name).ToList();
                ColumnNames = new List<String>() { "Contents", "Priority" };
                //ENTRY PANEL
                EntriesView = new EntriesTreeView(context, this);
                Children.Add(EntriesView);
                SetDock(EntriesView, Dock.Top);
                //OTHER
                LastChildFill = false;
            }

            public void ComboBox_SelectionChanged(object sender, RoutedEventArgs e) {
                //START a ORDER BY QUERY. I say, just call to UPDATE methods, BUT with NEW INPUT.
                updateEntries();
            }

            public void updateEntries() {
                EntriesView.updateEntries();
                //uc.addTo();
            }
    }
}
