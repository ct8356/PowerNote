using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using PowerNote.DAL;
using System.ComponentModel;

namespace PowerNote {
    class ControlPanel : DockPanel {
        SchoolContext schoolContext;

        public ControlPanel(SchoolContext schoolContext) {
            this.schoolContext = schoolContext;
            //LABEL 1
            Label label1 = new Label();
            label1.Content = "Control panel";
            Children.Add(label1);
            SetDock(label1, Dock.Top);
            //NEW ENTRY BUTTON
            Button newEntryButton = new Button();
            newEntryButton.Content = "New entry";
            Children.Add(newEntryButton);
            SetDock(newEntryButton, Dock.Top);
            //SAVE CHANGES BUTTON
            Button saveChangesButton = new Button();
            saveChangesButton.Content = "Save changes";
            Children.Add(saveChangesButton);
            //SUBSCRIBE TO EVENT
            //necessary?
            saveChangesButton.Click += saveChangesButton_Click;
            //YES! IT WORKS! perhaps, the delegate, already exists? so don't need the new keyword?
        }

        private void saveChangesButton_Click(object sender, EventArgs e) {
            //Ah, note, a delegate, calling this method,
            //is automatically subscribed to the button event!
            //BUT guess what, it is not calling this...
            schoolContext.SaveChanges();
        }
    }
}
