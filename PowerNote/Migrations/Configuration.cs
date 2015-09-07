using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using PowerNote.Models;
using PowerNote.DAL;

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
            var todos = new List<Task> {
                new Task { Contents = "Return ipod"},
                new Task { Contents = "Check ipod on Jon's PC"},
                new Task { Contents = "Fix bottom bracket"},
                new Task { Contents = "Straighten back wheel"},
                new Task { Contents = "Ask for roller chair"},
                new Task { Contents = "Ask for TV remote"},
                new Task { Contents = "Ask for shelves"},
                new Task { Contents = "Fix your pannier"}
            };
            todos.ForEach(student => context.ToDos.AddOrUpdate(s => s.Contents, student));
            //wow, it does not like this line...
            context.SaveChanges();

            var parts = new List<PartClass> {
                    new PartClass { NickName = "FU54", Manufacturer = "Keyence"},
                    new PartClass { NickName = "FU35", Manufacturer = "Keyence"},
                    new PartClass { NickName = "IE5827", Manufacturer = "IFM"},
                    new PartClass { NickName = "MFS200", Manufacturer = "IFM"},
                    new PartClass { NickName = "ME5010", Manufacturer = "IFM"},
                    new PartClass { NickName = "M8 Proxy", Manufacturer = "Balluff"},
            };
            parts.ForEach(part => context.Parts.AddOrUpdate(p => p.NickName, part));
            context.SaveChanges();

            var partInstances = new List<PartInstance> {
                    new PartInstance("Part present"),
                    new PartInstance("Part correctly oriented"),
            };
            partInstances.ForEach(pI => context.PartInstances.AddOrUpdate(PI => PI.FunctionText, pI));
            context.SaveChanges();

            var tags = new List<Tag> {
                new Tag {TagID = 1050, Title = "Question"},
                new Tag {TagID = 4022, Title = "IT"},
                new Tag {TagID = 4041, Title = "Bike"},
                new Tag {TagID = 1045, Title = "Room"},
                new Tag {TagID = 3141, Title = "Ebay"},
                new Tag {TagID = 2021, Title = "Laura"},
                new Tag {TagID = 2042, Title = "Jon"},
                new Tag {TagID = 2043, Title = "Part"}
            };
            tags.ForEach(course => context.Tags.AddOrUpdate(c => c.Title, course));
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

        public void AddOrUpdateTag(MyContext context, int entryID, string tagTitle) {
            var entry = context.Entrys.SingleOrDefault(e => e.EntryID == entryID);
            var tag = entry.Tags.SingleOrDefault(t => t.Title == tagTitle);
            if (tag == null)
                entry.Tags.Add(context.Tags.Single(t => t.Title == tagTitle));
        }

    }
}
