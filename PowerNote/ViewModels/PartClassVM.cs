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
    public class PartClassVM : EntryVM {
        //HELL! Just have a go at BINDING the VIEW (own EntryPanel for PARTS),
        //STRAIGHT to the MODEL. (BUT, the model contained HERE!!!).
        //OR of course, the view just binds to THIS VIEWMODEL:
        //THEN this VIEWMODEL NATURALLY just tells view how to show stuff.
        //I.E. this viewModel, acts bit like a view!!

        //NOTE: Perhaps better to put some STATIC properties in here as well,
        //So can try and bind to them, and this is reflected in ALL PartClassVMs!

        //NOTE: I think COULD get away, with just ONE type of VM. But keep as is for now.

        public PartClassVM(PartClass part, EntriesTreeVM parentVM) {
            //NOTE: this constructor just WRAPS a student in a VM.
            initialize(part, parentVM);
        }

        public PartClassVM(String name, EntriesTreeVM parentVM) {
            //NOTE: this one creates the Student, and THEN wraps it!!!
            PartClass part = new PartClass(name);
            initialize(part, parentVM);
            DbContext.Parts.Add(part); //YES!!
            DbContext.SaveChanges(); //remember this.
        }//easiest to keep here. (SINCE new is difficult with T generic classes!)

        protected override void initializePropertyList() {
            base.initializePropertyList();
            ImportantProperties.Add(new Property("Nickname", (Entry as PartClass).NickName, InfoType.TextBox, true, DbContext));
            ImportantProperties.Add(new Property("Order number", (Entry as PartClass).OrderNumber, InfoType.TextBox, true, DbContext));
            ImportantProperties.Add(new Property("Manufacturer", (Entry as PartClass).Manufacturer, InfoType.TextBox, true, DbContext));
        }

        public override void insertEntry(EntryVM selectedEntryVM) {
            PartClassVM entryVM = new PartClassVM("blank", TreeVM);
            insertEntry(entryVM, selectedEntryVM);
        }//easiest to keep here. (SINCE new is difficult with T generic classes!)
        //SO conclusion is, NOT really worth making the generic EntryVM.
        //SPECIALLY when I am under such time pressure!

        public override void insertSubEntry(EntryVM parentVM) {
            PartClassVM entryVM = new PartClassVM((parentVM.Entry as PartClass).NickName + " child", TreeVM); //create part.
            insertSubEntry(entryVM, parentVM);
        }//easiest to keep here.
        //This fails as generic because CANNOT create new() in generics with ANY PARAMETER!
        //Only way to do it is to pass constructor as a FUNCTION. Functional programming.
        //TOO COMPLICATED for now!

        //NOTE: Say, ASSEMBLY, can have parts.
        //PARTS all have 1 PARENT assembly, that they belong to.
        //Could they have more than one? No, not really... maybe in very rare cases.
        //Could always have a MAIN parent anyway.
        //ASSEMBLIES, would have other assemblies, who are THEIR children. 
        //EACH assembly has one PARENT!
        //PARTS are different, BECAUSE they represent ONE part, in database.
        //They are, the LEAF!.
        //NO REAL NEED for jobs... But could just tag them with it.
        //TAG assemblies, so can just show relevant ones...
        //COULD have ASSEMBLY tags, so only get RELEVANT suggestions.
        //OR even JOB tags... //gets tricky.
        //WELL YES! job properties, SINCE each WILL DEFO have a JOB NUMBER!
        //FOR our company at least... //CAN STILL auto assign it.
        //AND SHOULD REALLY have the option, that create a part...
        //THEN you assign a CHILD...
        //SO!! YES!! way to achieve goal, is to make TAGS, entries.
        //THEN to show them, you show tags, AND all their children!
        //IF even needs to be, a parent child relationship...
        //So, assign child, the child is, or, the link is, the ASSEMBLY its part of.
        //SOMETIMES, you do need, parent child relation, to show DIRECTION.
        //TAGS, direction NOT so important...
        //the PARENT IS, the ASSEMBLY its part of...
        //THEN, can show all ASSEMBLIES as parents (BASED ON TREE TEMPLATE!!!).
        //THEN, SHOW all the relevant PARTS, as CHILDREN!!!
        //THEN EVENTUALLY, do similar thing, with some GRAPHICS!
        //GETS WAY TOO COMPLICATED FOR ME!!! //NEED GO FULL TIME!!

    }
}
