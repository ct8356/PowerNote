using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using PowerNote.Models;
using PowerNote.DAL;

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
                new Tag {Title = "Question"},
                new Tag {Title = "IT"},
                new Tag {Title = "Bike"},
                new Tag {Title = "Room"},
                new Tag {Title = "Ebay"},
                new Tag {Title = "Laura"},
                new Tag {Title = "Jon"},
                new Tag {Title = "Part"},
                new Tag("J5693"),
                new Tag("Nissan infinity headliner NR")
            };
            tags.ForEach(tag => context.Tags.AddOrUpdate(c => c.Title, tag));
            context.SaveChanges();
            
            var tasks = new List<Task> {
                new Task { Contents = "Return ipod"},
                new Task { Contents = "Check ipod on Jon's PC"},
                new Task { Contents = "Fix bottom bracket"},
                new Task { Contents = "Straighten back wheel"},
                new Task { Contents = "Ask for roller chair"},
                new Task { Contents = "Ask for TV remote"},
                new Task { Contents = "Ask for shelves"},
                new Task { Contents = "Fix your pannier"}
            };
            tasks.ForEach(student => context.Tasks.AddOrUpdate(s => s.Contents, student));
            context.SaveChanges();

            var partClasses = new List<PartClass>();
            partClasses.Add(new PartClass { NickName = "FU54", Manufacturer = "Keyence"});
            partClasses.Add(new PartClass { NickName = "FU35", Manufacturer = "Keyence"});
            partClasses.Add(new PartClass { NickName = "IE5827", Manufacturer = "IFM"});
            partClasses.Add(new PartClass { NickName = "MFS200", Manufacturer = "IFM"});
            partClasses.Add(new PartClass { NickName = "ME5010", Manufacturer = "IFM"});
            partClasses.Add(new PartClass { NickName = "M8 Proxy", Manufacturer = "Balluff"});
            partClasses.Add(new PartClass { NickName = "LH mid HIC", Manufacturer = "Grupo" });
            partClasses.ForEach(partClass => context.Parts.AddOrUpdate(p => p.NickName, partClass));
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

            AddOrUpdateTag(context, 1, "Ebay");
            AddOrUpdateTag(context, 2, "Question");
            AddOrUpdateTag(context, 3, "Bike");
            AddOrUpdateTag(context, 4, "Bike");
            AddOrUpdateTag(context, 5, "Question");
            AddOrUpdateTag(context, 6, "Question");
            //AddOrUpdateTag(context, 7, "Question");
            //AddOrUpdateTag(context, 8, "Question");
            AddOrUpdateTag(context, 2, "IT");
            AddOrUpdateTag(context, 6, "Room");
            //AddOrUpdateTag(context, 7, "Room");
            //AddOrUpdateTag(context, 8, "Part");
            AddOrUpdateTag(context, 9, "Part");
            AddOrUpdateTag(context, 10, "Part");
            AddOrUpdateTag(context, 11, "Part");
            AddOrUpdateTag(context, 12, "Part");
            AddOrUpdateTag(context, 13, "Part");
            AddOrUpdateTag(context, 14, "Part");
            context.SaveChanges();

            //FORCE SOME CHILDREN IN
            var entry = context.Entrys.SingleOrDefault(e => e.EntryID == 9);
            Entry newEntry;
            entry.Children.Add( newEntry = new PartClass { NickName = "child" } );
            newEntry.Children.Add(new PartClass { NickName = "child child" });
            context.SaveChanges();
            //CHILD is defo there, it just is not being shown. //AHAH
            //ACTUALLY, it was NOT there, not in the DATABASE at least!
            context.SaveChanges();
        }

        public PartInstance createPartInstance(string functionText, string partClassNickName, List<string> tags, DbContext context) {
            PartInstance part = new PartInstance(functionText);
            context.PartInstances.AddOrUpdate(p => p.FunctionText, part);
            context.SaveChanges(); //must save it before adding partClass and tags?
            part.PartClass = context.Parts.Where(pc => pc.NickName == partClassNickName).First();
            foreach (string tag in tags) {
                AddOrUpdateTag(context, part.EntryID, tag);
            }
            context.SaveChanges();
            return part;
        }

        public void AddOrUpdateTag(DbContext context, int entryID, string tagTitle) {
            var entry = context.Entrys.SingleOrDefault(e => e.EntryID == entryID);
            var tag = entry.Tags.SingleOrDefault(t => t.Title == tagTitle);
            if (tag == null)
                entry.Tags.Add(context.Tags.Single(t => t.Title == tagTitle));
        }

    }
}
