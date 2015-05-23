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
            MyContext schoolContext = new MyContext(); //works fine. I guess table is made now.
            //schoolContext.Database.Create(); //YES! Now it works. WHy it no work without this? Me no Know.
            //ACtually, NO! the database is still not made? WTF? //MAYBE just deleting .mdf files from SQLServer data folder is bad idea.
            //IT PROBS still thinks that they exist. Yes I reckon. BUT then why when I changed Context class name, did it not make NEW one?
            //Ok, so this does indeed, create a database, if not already exist.
            //BUT WHERE is this database created? It is in ProgramFiles, in the SQLServer folder. Let's delete it.
            Configuration configuration = new Configuration();
            //configuration.callSeed(schoolContext); //Only need to do this once. Or it will overwrite data.
            //issues here. cbtl. fix it.
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
