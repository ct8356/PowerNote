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

        public TaskVM(Task student, EntriesTreeVM treeVM) {
            //NOTE: this constructor just WRAPS a student in a VM.
            initialize(student, treeVM);
        }

        public TaskVM(String name, EntriesTreeVM treeVM) {
            //NOTE: this one creates the Entry, and THEN wraps it!!!
            Task newTask = new Task(name);
            initialize(newTask, treeVM);
            DbContext.ToDos.Add(newTask);
        }

        public void insertTask(TaskVM selectedVM) {
            TaskVM entryVM = new TaskVM("blank", TreeVM);
            insertEntry(entryVM, selectedVM);
        }

        public void insertSubTask(TaskVM selectedVM) {
            TaskVM entryVM = new TaskVM((selectedVM.Entry as Task).Contents + " child", TreeVM); 
            //this creates an entry too!
            insertSubEntry(entryVM, selectedVM);
        }
    }
}
