using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using PowerNote.Models;
using PowerNote.DAL;
using System.Collections.ObjectModel;

namespace PowerNote.Migrations {

    internal sealed class Configuration : DbMigrationsConfiguration<MyContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = false;
        }

        public void callSeed(MyContext context) {
            Seed(context);
        }
        //RIGHT, so this configuration, WON'T DO nice migrations.
        //BUT, can look into that, when really need it.
        //For now, can just delete, and recreate the database.

        protected override void Seed(MyContext context) {
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
            tasks.ForEach(student => context.ToDos.AddOrUpdate(s => s.Contents, student));
            context.SaveChanges();

            var parts = new List<PartClass>();
            parts.Add(new PartClass { NickName = "FU54", Manufacturer = "Keyence"});
            parts.Add(new PartClass { NickName = "FU35", Manufacturer = "Keyence"});
            parts.Add(new PartClass { NickName = "IE5827", Manufacturer = "IFM"});
            parts.Add(new PartClass { NickName = "MFS200", Manufacturer = "IFM"});
            parts.Add(new PartClass { NickName = "ME5010", Manufacturer = "IFM"});
            parts.Add(new PartClass { NickName = "M8 Proxy", Manufacturer = "Balluff"});
            //CustomerParts
            PartClass tempPart;
            parts.Add(tempPart = new PartClass("Customer part"));
            parts.Add(new PartClass("LH mid HIC 1", tempPart));
            parts.ForEach(part => context.Parts.AddOrUpdate(p => p.NickName, part));
            context.SaveChanges();

            createPartInstance("Part present", "FU54", new List<string> { "J5693" }, context);
            createPartInstance("Part correctly oriented", "FU35", new List<string> { "J5693" }, context);
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

        public void createPartInstance(string functionText, string partClassNickName, List<string> tags, MyContext context) {
            PartInstance part = new PartInstance(functionText);
            context.PartInstances.AddOrUpdate(p => p.FunctionText, part);
            context.SaveChanges();
            part.PartClass = context.Parts.Where(pc => pc.NickName == partClassNickName).First();
            int eid = part.EntryID;
            foreach (string tag in tags) {
                AddOrUpdateTag(context, part.EntryID, tag);
            }
        }

        public void AddOrUpdateTag(MyContext context, int entryID, string tagTitle) {
            var entry = context.Entrys.SingleOrDefault(e => e.EntryID == entryID);
            var tag = entry.Tags.SingleOrDefault(t => t.Title == tagTitle);
            if (tag == null)
                entry.Tags.Add(context.Tags.Single(t => t.Title == tagTitle));
        }

    }
}
