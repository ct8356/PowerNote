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
using System.Windows.Input;
using PowerNote.Models;
using PowerNote.ViewModels;

namespace PowerNote {
    public partial class StructurePanel : ListBoxPanel {
        public DisplayPanel DisplayPanel { get; set; }

        public StructurePanel(MyContext context, DisplayPanel displayPanel) {
            DisplayPanel = displayPanel;
            InitializeComponent();
            MyAutoCompleteBox.KeyUp += autoCompleteBox_KeyUp;
        }

    }
}
