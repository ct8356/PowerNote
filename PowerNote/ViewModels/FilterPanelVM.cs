using PowerNote.Models;
using System.Data.Entity;
using System.Collections.ObjectModel;

namespace PowerNote.ViewModels {
    public class FilterPanelVM : ListBoxVM<object> {
             
        public FilterPanelVM(MainVM parentVM) : base(parentVM) {
            DbContext.Tags.Load();
            foreach (object tag in DbContext.Tags.Local) {
                Objects.Add(tag);
            }
        }

        public override void addSelectedItem(object selectedItem) {
            SelectedObjects.Add(selectedItem);
            base.addSelectedItem(selectedItem); //maybe lazy. (should use listeners?)
        }

    }
}
