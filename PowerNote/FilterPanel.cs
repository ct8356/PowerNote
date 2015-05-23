﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity;
using PowerNote.DAL;
using PowerNote.Migrations;
using System.Windows.Data;
using PowerNote.Models;

namespace PowerNote {
    class FilterPanel : StackPanel {
        Label title;
        MyContext context;
        public TaggedObject Filter { get; set; }

        public FilterPanel(MyContext context) {
            this.context = context;
            //PANEL
            Orientation = Orientation.Horizontal;
            title = new Label();
            title.Content = "Filter by tag:";
            Children.Add(title);
            //TAG LIST PANEL
            Filter = new TaggedObject();
            Filter.Courses = new List<Course>();
            //filter.Courses.Add(new Course("Chemistry"));
            TagListPanel tagListPanel = new TagListPanel(Filter, context);
            Children.Add(tagListPanel);
        }


    }
}
