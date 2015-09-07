using System;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using PowerNote.Models;
using PowerNote.DAL;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PowerNote.ViewModels {

    public class TaskVM : EntryVM {

        public TaskVM(Task student, MainPanel mainPanel) {
            //NOTE: this constructor just WRAPS a student in a VM.
            initialize(student, mainPanel);
        }

        public TaskVM(String name, MainPanel mainPanel) {
            //NOTE: this one creates the Student, and THEN wraps it!!!
            Task newStudent = new Task(name);
            initialize(newStudent, mainPanel);
            Context.ToDos.Add(newStudent);
        }

        public void insertTask() {
            TaskVM entryVM = new TaskVM("blank", MainPanel);
            foreach (Tag tag in MainPanel.DisplayPanel.FilterPanel.Filter.Tags) {
                entryVM.Entry.Tags.Add(tag);
            }
            Context.SaveChanges();
            MainPanel.DisplayPanel.EntriesView.updateEntries();
        }

        public void insertSubTask(TaskVM parentVM) {
            TaskVM entryVM = new TaskVM
                ((parentVM.Entry as Task).Contents + " child", MainPanel);
            parentVM.Entry.Children.Add(entryVM.Entry);
            foreach (Tag tag in MainPanel.DisplayPanel.FilterPanel.Filter.Tags) {
                entryVM.Entry.Tags.Add(tag);
            }
            Context.SaveChanges();
            MainPanel.DisplayPanel.EntriesView.updateEntries(); //nec
        }
    }
}
