using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using PowerNote.Models;
using PowerNote.DAL;
using CJT.Models;
using System.Collections.ObjectModel;
using DbContext = PowerNote.DAL.DbContext;

namespace PowerNote.Migrations {

    internal sealed class Configuration : DbMigrationsConfiguration<DbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = true; //was False, but error suggested
            //I change it to true
        }

        public void callSeed(DbContext context) {
            Seed(context);
        }
        //RIGHT, so this configuration, WON'T DO nice migrations.
        //BUT, can look into that, when really need it.
        //For now, can just delete, and recreate the database.
        //NOW I have updated to EF v6, will it do nice migs now?

        protected override void Seed(DbContext context) {
            //OK! Perhaps, it is actually THIS method, that creates the database.
            //NOT the create context instantiation...
            //(I guess create(), will still do the trick. but NOT context instantiation...nec.) ??
            var tags = new List<Tag> {
                new Tag("Question"),
                new Tag("IT"),
                new Tag("Bike"),
                new Tag("Room"),
                new Tag("Ebay"),
                new Tag("Laura"),
                new Tag("Jon"),
                new Tag("Part"),
                new Tag("J5693"),
                new Tag("Nissan infinity headliner NR")
            };
            tags.ForEach(tag => context.Tags.AddOrUpdate(t => t.Name, tag));
            context.SaveChanges();

            var tasks = new List<Task> {
                CreateEntryWithTags<Task>("Return ipod nano", "Ebay", context),
                CreateEntryWithTags<Task>("Check ipod nano on Jon's PC", "Ebay", context),
                CreateEntryWithTags<Task>("Fix bottom bracket", "Bike", context),
                CreateEntryWithTags<Task>("Straighten back wheel", "Bike", context),
                CreateEntryWithTags<Task>("Ask for roller chair", "Question", context),
                CreateEntryWithTags<Task>("Ask for TV remote", "Question", context),
                CreateEntryWithTags<Task>("Ask for shelves", "Room", context),
                CreateEntryWithTags<Task>("Fix your pannier", "Bike", context)
            };
            tasks.ForEach(student => context.Tasks.AddOrUpdate(s => s.Name, student));
            context.SaveChanges();

            var partClasses = new List<PartClass>();
            partClasses.Add(new PartClass { Name = "FU54", Manufacturer = "Keyence"});
            partClasses.Add(new PartClass { Name = "FU35", Manufacturer = "Keyence"});
            partClasses.Add(new PartClass { Name = "IE5827", Manufacturer = "IFM"});
            partClasses.Add(new PartClass { Name = "MFS200", Manufacturer = "IFM"});
            partClasses.Add(new PartClass { Name = "ME5010", Manufacturer = "IFM"});
            partClasses.Add(new PartClass { Name = "M8 Proxy", Manufacturer = "Balluff"});
            partClasses.Add(new PartClass { Name = "LH mid HIC", Manufacturer = "Grupo" });
            partClasses.ForEach(partClass => context.Parts.AddOrUpdate(p => p.Name, partClass));
            context.SaveChanges();
            PartInstance parent = createPartInstance("Parent", "FU54", new List<string> { "J5693" }, context);
            PartInstance part = createPartInstance("Part present", "FU54", new List<string> { "J5693" }, context);
            createPartInstance("Part correctly oriented", "FU35", new List<string> { "J5693" }, context);
            //CustomerParts
            PartInstance custPart = createPartInstance("LH mid HIC 1", "LH mid HIC", new List<string> { "Nissan infinity headliner NR" }, context);
            part.SensedParts.Add(custPart);
            custPart.Sensor = part;
            parent.Children.Add(part);
            context.SaveChanges();

            //FORCE SOME CHILDREN IN
            var entry = context.Entries.SingleOrDefault(e => e.EntryID == 9);
            Entry newEntry;
            entry.Children.Add( newEntry = new PartClass { Name = "child" } );
            newEntry.Children.Add(new PartClass { Name = "child child" });
            context.SaveChanges();
            //CHILD is defo there, it just is not being shown. //AHAH
            //ACTUALLY, it was NOT there, not in the DATABASE at least!
            context.SaveChanges();
        }

        public PartInstance createPartInstance(string functionText, string partClassNickName, List<string> tags, DbContext context) {
            PartInstance part = new PartInstance(functionText);
            context.PartInstances.AddOrUpdate(p => p.Name, part);
            context.SaveChanges(); //must save it before adding partClass and tags?
            part.PartClass = context.Parts.Where(pc => pc.Name == partClassNickName).First();
            foreach (string tag in tags) {
                AddOrUpdateTag(context, part.EntryID, tag);
            }
            context.SaveChanges();
            return part;
        }

        public void AddOrUpdateTag(DbContext context, int entryID, string tagTitle) {
            var entry = context.Entries.SingleOrDefault(e => e.EntryID == entryID);
            var tag = entry.Tags.SingleOrDefault(t => t.Name == tagTitle);
            if (tag == null)
                entry.Tags.Add(context.Tags.Single(t => t.Name == tagTitle));
        }

        public T CreateEntryWithTags<T>(string entryName, string tagTitle, DbContext context) 
            where T : Entry, new() {
            T entry = new T();
            entry.Name = entryName;
            Tag tag = context.Tags.First(t => t.Name == tagTitle);
            if (tag == null)
                context.Tags.Add(context.Tags.Single(t => t.Name == tagTitle));
            entry.Tags.Add(tag);
            context.SaveChanges();
            return entry;
        }

    }
}
