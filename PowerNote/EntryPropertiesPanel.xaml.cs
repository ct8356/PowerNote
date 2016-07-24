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
using CJT;
using ListBox = CJT.ListBox;
using TextBlock = CJT.TextBlock;

namespace PowerNote {
    public partial class EntryPropertiesPanel : StackPanel {

        public EntryPropertiesPanel() {
            InitializeComponent();
            //SUBSCRIBE
            DataContextChanged += this_DataContextChanged;
            //Loaded += this_DataContextChanged;
        }

        protected void bindListBox(ListBox listBox, Entry entry, string propertyName) {
            Binding binding = new Binding(propertyName);
            binding.Source = entry;
            listBox.SetBinding(ListBox.ItemsSourceProperty, binding);
        } //YES it works!

        protected void bindTextBlock(TextBlock textBlock, Entry entry, string propertyName) {
            Binding binding = new Binding(propertyName);
            binding.Source = entry;
            textBlock.SetBinding(TextBlock.TextProperty, binding);
        }

        protected void bindTextBox(TextBox textBox, Entry entry, string propertyName) {
            Binding binding = new Binding(propertyName); //This is the MODEL property it binds to.
            binding.Source = entry; // the binding source (which must fire a PROP CHANGED event).
            textBox.SetBinding(TextBox.TextProperty, binding); //fortunately, textBox already fires an event when changed.
            //YOU created the event for the dataSource. SO HOPEFULLY, we have 2 way binding now... we do :)
        }

        public void DataContext_PropertyChanged(object sender, EventArgs e) {
        }

        public void this_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            updateControls();
        }

        public void updateControls() { //KEEP incase decide to use later.
            Children.Clear();
            Children.Add(new TextBlock() { Text = "Properties", FontWeight = FontWeights.Bold });
            EntryVM selectedEntryVM = (DataContext as EntryVM);
            foreach (Property property in selectedEntryVM.ImportantProperties) {
                PropertyPanel propertyPanel = new PropertyPanel(property.Name);
                Children.Add(propertyPanel);
                if (property.Value != null) {
                    switch (property.Type) {
                        case InfoType.TextBlock:
                            TextBlock textBlock = new TextBlock();
                            bindTextBlock(textBlock, selectedEntryVM.Entry, property.Name);
                            propertyPanel.Children.Add(textBlock);
                            break;
                        case InfoType.LinkedTextBlock:
                            LinkedTextBlock linkedTextBlock = new LinkedTextBlock();
                            bindTextBlock(linkedTextBlock, selectedEntryVM.Entry, property.Name);
                            linkedTextBlock.DataContext = selectedEntryVM.TreeVM.WrapInCorrectVM(property.Value as Entry);
                            propertyPanel.Children.Add(linkedTextBlock);
                            break;
                        case InfoType.TextBox:
                            propertyPanel.Children.Add(new TextBox() 
                            { Text = property.Value.ToString() });
                            break;
                        case InfoType.ComboBox:
                            break;
                        case InfoType.ListBox:
                            ListBox listBox = new ListBox();
                            bindListBox(listBox, selectedEntryVM.Entry, property.Name);
                            //ABOVE line is needed to bind the listBox to the list.
                            listBox.DataContext = property.Value; //AHAH! really easy solve!
                            //JUST set property.Value for lists, to the TagsVM etc. easy!
                            //ABOVE line is needed, otherwise pretty sure it just inherits DC from its EntryVM.
                            propertyPanel.Children.Add(listBox);
                            FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(LinkedTextBlock));
                            Binding binding = new Binding("Name");
                            textBlockFactory.SetBinding(LinkedTextBlock.TextProperty, binding); //not needed?
                            //I think the above line is needed to bind to the textblock to the entry in the list!
                            DataTemplate dataTemplate = new DataTemplate();
                            dataTemplate.VisualTree = textBlockFactory;
                            listBox.ItemTemplate = dataTemplate;
                            //FUNNY THING is, DataContext of ListBox,
                            //Is coming out as a EntryVM.
                            //When it SHOULD be a ListBoxPanelVM...
                            break;
                        case InfoType.CheckBox:
                            break;
                    }
                }

            }
        }



    }
}
