using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using PowerNote.ViewModels;
using PowerNote.Models;

namespace PowerNote {
    public partial class EntryPropertiesPanel : StackPanel {

        public EntryPropertiesPanel() {
            InitializeComponent();
            //SUBSCRIBE
            DataContextChanged += this_DataContextChanged;
        }

        public void DataContext_PropertyChanged(object sender, EventArgs e) {
            
        }

        public void this_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            updateControls();
        }

        public void updateControls() {
            Children.Clear();
            Children.Add(new Label() { Content = "Properties", FontWeight = FontWeights.Bold });
            EntryVM selectedEntryVM = (DataContext as EntryVM);
            foreach (Property property in selectedEntryVM.AllProperties) {
                PropertyPanel propertyPanel = new PropertyPanel(property.Name);
                Children.Add(propertyPanel);
                if (property.Value != null) {
                    switch (property.Type) {
                        case InfoType.TextBox:
                            propertyPanel.Children.Add(new TextBox() { Text = property.Value.ToString() });
                            break;
                        case InfoType.ComboBox:
                            break;
                        case InfoType.ListBox:
                            propertyPanel.Children.Add(new ListBox() { ItemsSource = property.Value as ObservableCollection<Entry> });
                            break;
                        case InfoType.CheckBox:
                            break;
                        case InfoType.Link:
                            //Children.Add(new LinkListBox() { ItemsSource = property.Value as ObservableCollection<Entry> });
                            break;
                    }
                }

            }
        }

        //public void bindTextBox(Entry student) {
        //    this.Student = student;
        //    Binding binding = new Binding("Contents"); //This is the MODEL property it binds to.
        //    binding.Source = Student; // the binding source (which must fire a PROP CHANGED event).
        //    textBox.SetBinding(TextBox.TextProperty, binding); //fortunately, textBox already fires an event when changed.
        //    //YOU created the event for the dataSource. SO HOPEFULLY, we have 2 way binding now... we do :)
        //}

    }
}
