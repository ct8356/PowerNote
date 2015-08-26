using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel; //this allows INotifyPropertyChanged
using System.Collections.ObjectModel;
using PowerNote.Models;
using PowerNote.DAL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PowerNote.ViewModels {
    public class PartVM : EntryVM {
        //HELL! Just have a go at BINDING the VIEW (own EntryPanel for PARTS),
        //STRAIGHT to the MODEL. (BUT, the model contained HERE!!!).
        //OR of course, the model just binds to THIS VIEWMODEL:
        //THEN this VIEWMODEL NATURALLY just tells view how to show stuff.
        //I.E. this viewModel, acts bit like a view!!

        public PartVM(Part part, MainPanel mainPanel) : base(mainPanel) {
            //NOTE: this constructor just WRAPS a student in a VM.
            bindToPart(part);
            //Children = Student.Children; Children should be filled elsewhere.
        }

        public PartVM(String name, MainPanel mainPanel) : base(mainPanel) {
            //NOTE: this one creates the Student, and THEN wraps it!!!
            Part part = new Part(name);
            Context.Parts.Add(part); //YES!!
            Context.SaveChanges(); //remember this.
            bindToPart(part);
        }

        public void bindToPart(Part part) {
            Entry = part as Entry;
        }

        public void insertPart() {
            MainPanel.DisplayPanel.EntriesView.insertPart();
        }

        public void insertSubNote(StudentVM studentVM) {
            MainPanel.DisplayPanel.EntriesView.insertSubNote(studentVM);
        }

    }
}
