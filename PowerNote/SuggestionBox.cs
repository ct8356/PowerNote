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
using System.Windows.Controls.Primitives;

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
            //Now, I THINK itemsSource, does a binding, as long as you fire a PropChanged event from Courses class. Done.
            //UpdateSelection();
            IsTextCompletionEnabled = true; //YES! Works a treat. Just backspace if dont want text completion.
            //BUT it would be good if it would highlight the box below too. for easy up and down arrowing. //Not important though.
        }

        private void UpdateSelection() {
            // get the source of the ListBox control inside the template
            var enumerator = ((Selector)GetTemplateChild("Selector")).ItemsSource.GetEnumerator();
            // update Selecteditem with the first item in the list
            enumerator.Reset();
            if (enumerator.MoveNext()) {
                var item = enumerator.Current;
                SelectedItem = item;
                // close the popup, highlight the text
                IsDropDownOpen = false;
                //(TextBox)GetTemplateChild("Text").SelectAll();
            }
        }

    }
}
