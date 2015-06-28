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

    class DisplayPanel : DockPanel {
            Label title;
            MyContext context;
            public FilterPanel FilterPanel { get; set; }
            public SortPanel SortPanel { get; set; }
            public OptionsPanel OptionsPanel { get; set; }
            public List<String> ColumnNames { get; set; }
            MyEntriesView EntriesView { get; set; }
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
                ColumnNamePanel columnNamePanel = new ColumnNamePanel(this);
                Children.Add(columnNamePanel);
                SetDock(columnNamePanel, Dock.Top);
                //ENTRY PANEL
                //UserControl1 fad = new UserControl1();
                //Children.Add(fad);
                EntriesView = new MyEntriesView();
                Children.Add(EntriesView);
                SetDock(EntriesView, Dock.Top);
                //OTHER
                //label2 = new Label();
                //Binding binding = new Binding("FirstMidName"); //This is the MODEL property it binds to.
                //binding.Source = context.Students.ToList()[0]; // the binding source (which must fire a PROP CHANGED event).
                //label2.SetBinding(Label.ContentProperty, binding);
                //Children.Add(label2);
                LastChildFill = false;
            }

            public void ComboBox_SelectionChanged(object sender, RoutedEventArgs e) {
                //START a ORDER BY QUERY. I say, just call to UPDATE methods, BUT with NEW INPUT.
                updateEntries();
            }

            public void updateEntries() {
                EntriesView.updateEntries();
            }
    }
}
