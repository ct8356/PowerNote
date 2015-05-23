using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using PowerNote.Models;
using PowerNote.DAL;

namespace PowerNote {
    class SuggestionBox : AutoCompleteBox {
        MyContext context;
        Student student;

        public SuggestionBox(MyContext context)
            : base() {
            this.context = context;
            //autoCompleteBox.ItemsSource = new List<String> {"qwer","asdf", "zxcv"};//this works...
            context.Courses.Load();
            ItemsSource = context.Courses.Local; //This gives better bind that one below? Yes, certainly. For how can you bind to result of a query? Makes no sense.
            //ItemsSource = context.Courses.Select(c => c);
            //WOULD be good, to set this to Courses, rather than just strings. CBTL.
            //Now, I THINK itemsSource, does a binding, as long as you fire a PropChanged event from Courses class. Done.
            //NOW I need to research, how to NICELY add something to the ENROLLMENT TABLE. See Contoso tutorial.
            //autoCompleteBox.SelectedItem = schoolContext.Courses.ToList()[0].Title; //This does not matter really.
            //Binding binding2 = new Binding("Title"); //This is the MODEL property it binds to.
            //binding2.Source = course; // the binding source (which must fire a PROP CHANGED event).
            //autoCompleteBox.SetBinding(AutoCompleteBox., binding2);
        }

    }
}
