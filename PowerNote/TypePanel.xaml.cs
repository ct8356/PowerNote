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
        public DisplayPanel DisplayPanel { get; set; }

        public TypePanel(MyContext context, DisplayPanel displayPanel) {
            DisplayPanel = displayPanel;
            InitializeComponent();
            MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

    }
}
