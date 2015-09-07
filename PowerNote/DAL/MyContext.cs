using PowerNote.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel; //this allows INotifyPropertyChanged

namespace PowerNote.DAL { //DAL stands for Data Access Layer.
    //NOTE: what you cannot see here,
    //is that when this class is instantiated, a connection is made.
    //The "namespace + class name" are used to identify the database.
    //If database does not exist, is it created? yes!
    //NOTE, it also has a Database property.
    //Apparently, if call database.Create, creates a database that matches this schema.
    //SO, don't even need InitialCreate file??? No, you don't.
    public class MyContext : DbContext {
        public DbSet<Task> ToDos { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PartClass> Parts { get; set; }
        public DbSet<PartInstance> PartInstances { get; set; }
        public DbSet<Entry> Entrys { get; set; }

        public delegate PropertyChangedEventHandler Handler(PropertyChangedEventArgs args);

        public MyContext() : base() {
            //Problem is, I want it to handle prop changed every time ANY student changes.
            //which means, subscribing to EVERY student.
            //AND don't just want to save database,
            //have to UPDATE a row as well.
            //MUST be easier way to do this?
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        //HANDLERS
        //Now, I want this Class to respond to an event.
        public void handlePropertyChanged(PropertyChangedEventArgs args) {

        }
    }
}
