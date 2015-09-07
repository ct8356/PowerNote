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

    public class MainPanel : DockPanel {
        public ControlPanel ControlPanel { get; set; }
        public List<DisplayPanel> DisplayPanels { get; set; }
        public DisplayPanel DisplayPanel { get; set; }
        public MyContext Context { get; set; }

        public MainPanel() {
            //DATA
            Context = new MyContext(); //works fine. I guess table is made now.
            Context.Database.Delete(); Context.Database.Create(); Configuration configuration = new Configuration(); configuration.callSeed(Context);
            //Ok, so this does indeed, create a database, if not already exist.
            //BUT WHERE is this database created? It is in ProgramFiles, in the SQLServer folder. Let's delete it.
            //ACtually, NO! the database is still not made? WTF? //MAYBE just deleting .mdf files from SQLServer data folder is bad idea.
            //IT PROBS still thinks that they exist. Yes I reckon. BUT then why when I changed Context class name, did it not make NEW one?
            //YES! Now it works. WHy it no work without this? Me no Know.
            //CONTROL
            ControlPanel = new ControlPanel(Context, this);
            Children.Add(ControlPanel);
            SetDock(ControlPanel, Dock.Left);
            //ADD DISPLAY PANELS
            DisplayPanels = new List<DisplayPanel>();
            //DISP 1
            DisplayPanel = new DisplayPanel(Context, this);
            DisplayPanels.Add(DisplayPanel);
            Children.Add(DisplayPanel);
            SetDock(DisplayPanel, Dock.Left);
            //DISP 2
            //DisplayPanel disp2 = new DisplayPanel(context, this);
            //DisplayPanels.Add(disp2);
            //Children.Add(disp2);
            //SetDock(disp2, Dock.Left);
            //SIDE-NOTE
            //SideNotePanel sideNotePanel = new SideNotePanel(context, this);
            //Children.Add(sideNotePanel);
            //SetDock(sideNotePanel, Dock.Bottom);
        }

        public void updateEntries() {
            foreach (DisplayPanel panel in DisplayPanels) {
                panel.updateEntries(); //Should not have to, since I will only delete ones with NO attachments. For now.
            }
        }

    }
}
