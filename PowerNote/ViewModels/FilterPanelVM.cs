using PowerNote.Models;
using System.Data.Entity;
using System.Collections.ObjectModel;

namespace PowerNote.ViewModels {
    public class FilterPanelVM : ListBoxPanelVM {
        //public ObservableCollection<Tag> Tags { get; set; }
        //public ObservableCollection<Tag> SelectedTags { get; set; }
             
        public FilterPanelVM(MainVM parentVM) : base(parentVM) {
            DbContext.Tags.Load();
            //Tags = DbContext.Tags.Local;
            foreach (Tag tag in DbContext.Tags.Local) {
                Objects.Add(tag);
            }
            //Tags = new ObservableCollection<Tag> { new Tag("Balls") };//works.
        }

        public override void addSelectedItem(object selectedItem) {
            SelectedObjects.Add(selectedItem as Tag);
            base.addSelectedItem(selectedItem); //maybe lazy. (should use listeners?)
        }

    }
}
