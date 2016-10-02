using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.ViewModels;

namespace PowerNote {
    public partial class TypePanel : ListBoxPanel {
        public TextBlock TextBlock { get; set; }

        public TypePanel() {
            InitializeComponent();
            AutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
            ListBox.DisplayMemberPath = "Name";
            AutoCompleteBox.ValueMemberPath = "Name";
            //AutoCompleteBox.ItemTemplate = new DataTemplate(TextBlock);
        }

    }
}
