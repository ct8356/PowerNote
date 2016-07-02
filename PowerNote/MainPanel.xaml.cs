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
using PowerNote.ViewModels;

namespace PowerNote {
    public partial class MainPanel : DockPanel {
        public ControlPanel ControlPanel { get; set; }
        public List<DisplayPanel> DisplayPanels { get; set; }

        public MainPanel() {
            //ADD DISPLAY PANELS
            DisplayPanels = new List<DisplayPanel>();
            //SIDE-NOTE
            //SideNotePanel sideNotePanel = new SideNotePanel(context, this);
            //Children.Add(sideNotePanel);
            //SetDock(sideNotePanel, Dock.Bottom);
            InitializeComponent();
        }

        public void updateEntries() {
            foreach (DisplayPanel panel in DisplayPanels) {
                panel.updateEntries(); //Should not have to, since I will only delete ones with NO attachments. For now.
            }
        }

        public void Seed_Click(object sender, RoutedEventArgs args) {
            (DataContext as MainVM).seedDatabase();
        }

        public void DoTheThing(object sender, EventArgs args) {
            //PropsPanel.updateControls();
            (DisplayPanel.ComboBox.DataContext as TypePanelVM).SelectedObjects.Add("poop");

        }
    }
}
