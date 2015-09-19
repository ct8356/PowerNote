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
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Models;
using PowerNote.ViewModels;
using PowerNote.Migrations;
using System.Collections.ObjectModel;

namespace PowerNote.ViewModels {
    public class PartInstanceVM : EntryVM {
        public ObservableCollection<string> NickNames { get; set; }
        public PartClass PartClass { get; set; }
        public PartInstanceVM(Entry entry, MainPanel mainPanel) {
            //NOTE: this constructor just WRAPS an entry in a VM.
            initialize(entry, mainPanel);
            IQueryable<PartClass> partClasses = Context.Parts;
            PartClass partClass = partClasses.First();
            //(entry as PartInstance).PartClass = partClass;
            //WHAT THE!!??? NOW IT WORKS???
            //Weird.
        }

        public PartInstanceVM(String name, MainPanel mainPanel) {
            //NOTE: this one creates the Student, and THEN wraps it!!!
            PartInstance entry = new PartInstance(name);
            initialize(entry, mainPanel);
            Context.PartInstances.Add(entry);
            Context.SaveChanges(); //remember this.
        }

        public void initialize(Entry entry, MainPanel mainPanel) {
            base.initialize(entry, mainPanel);
            IQueryable<PartClass> partClasses = Context.Parts;
            IQueryable<string> partNickNames = partClasses.Select(pc => pc.NickName).Distinct();
            NickNames = new ObservableCollection<string>();
            foreach (string name in partNickNames) {
                NickNames.Add(name);
            }
        }

        public void addPartClassToEntry(string text) {
            IQueryable<PartClass> partClasses = Context.Parts;
            PartClass partClass = partClasses.Where(pc => pc.NickName == text).First();
            (Entry as PartInstance).PartClass = partClass;
            Context.SaveChanges();
            MainPanel.DisplayPanel.EntriesView.updateEntries();
            //CBTL, why was this not working before?
            //nav property? lack of update entries?
        }

        public void insertPart() {
            PartClassVM partVM = new PartClassVM("blank", MainPanel);
            foreach (Tag tag in MainPanel.DisplayPanel.FilterPanel.Filter.Tags) {
                partVM.Entry.Tags.Add(tag);
            }
            Context.SaveChanges();
            MainPanel.DisplayPanel.EntriesView.updateEntries();
        }

        public void insertSubPart(PartClassVM parentVM) {
            PartClassVM partVM = new PartClassVM
                ((parentVM.Entry as PartClass).NickName + " child", MainPanel); //create part.
            parentVM.Entry.Children.Add(partVM.Entry); //add it to children
            foreach (Tag tag in MainPanel.DisplayPanel.FilterPanel.Filter.Tags) {
                partVM.Entry.Tags.Add(tag); //give it tags as per filter
            }
            Context.SaveChanges();
            MainPanel.DisplayPanel.EntriesView.updateEntries(); //nec
        }

    }
}
