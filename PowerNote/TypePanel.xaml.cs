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

        public TypePanel() {
            InitializeComponent();
            MyChoiceBox.KeyUp += autoCompleteBox_KeyUp;
            MyListBox.DisplayMemberPath = "Name";
            MyChoiceBox.ValueMemberPath = "Name";
        }

    }
}
