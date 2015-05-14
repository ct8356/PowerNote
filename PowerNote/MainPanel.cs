namespace PowerNote {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Data.Entity;
    using PowerNote.DAL;
    using PowerNote.Migrations;

    class MainPanel : DockPanel {

        public MainPanel() {
            //DATA
            SchoolContext schoolContext = new SchoolContext();
            //Ok, so this does indeed, create a database, if not already exist.
            //BUT WHERE is this database created?
            //Configuration configuration = new Configuration();
            //configuration.callSeed(schoolContext); Onlyt need to do this once. Or it will overwrite data.
            //PANEL
            ControlPanel controlPanel = new ControlPanel(schoolContext);
            DisplayPanel displayPanel = new DisplayPanel(schoolContext);
            Children.Add(controlPanel);
            Children.Add(displayPanel);
            SetDock(controlPanel, Dock.Left);
            //SetDock(displayPanel, Dock.Right);
        }

    }
}
