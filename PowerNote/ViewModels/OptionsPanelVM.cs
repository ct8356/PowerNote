using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Migrations;
using System.Windows.Data;
using CJT.Models;

namespace PowerNote.ViewModels {
    public class OptionsPanelVM {
        DAL.DbContext context;
        public bool ShowAllEntries { get; set; }
        public bool ShowAllChildren { get; set; }

        public OptionsPanelVM(MainVM parentVM) {
            this.context = parentVM.DbContext;
        }

    }
}