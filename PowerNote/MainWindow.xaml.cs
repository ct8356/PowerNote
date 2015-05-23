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

namespace PowerNote {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window{
        Grid grid;
        MainPanel mainPanel;
        Button button1;
        Button button2;

        public MainWindow() {
            WindowState = WindowState.Maximized;
            Background = SystemColors.ControlLightBrush;
            //WINDOW STUFF
            //grid = new MyGrid();
            button1 = new Button();
            button2 = new Button();
            button1.Content = "one";
            button2.Content = "two";
            mainPanel = new MainPanel();
            //NOTE, this window can only have one child.
            //SO Main panel is always, the main panel.
            AddChild(mainPanel);
            InitializeComponent();
        }
    }
}
