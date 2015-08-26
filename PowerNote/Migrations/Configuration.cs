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
            var todos = new List<ToDo> {
                new ToDo { Contents = "Return ipod",            EnrollmentDate = DateTime.Parse("2010-09-01") },
                new ToDo { Contents = "Check ipod on Jon's PC", EnrollmentDate = DateTime.Parse("2012-09-01") },
                new ToDo { Contents = "Fix bottom bracket",     EnrollmentDate = DateTime.Parse("2013-09-01") },
                new ToDo { Contents = "Straighten back wheel",  EnrollmentDate = DateTime.Parse("2012-09-01") },
                new ToDo { Contents = "Ask for roller chair",   EnrollmentDate = DateTime.Parse("2012-09-01") },
                new ToDo { Contents = "Ask for TV remote",      EnrollmentDate = DateTime.Parse("2011-09-01") },
                new ToDo { Contents = "Ask for shelves",        EnrollmentDate = DateTime.Parse("2013-09-01") },
                new ToDo { Contents = "Fix your pannier",       EnrollmentDate = DateTime.Parse("2005-08-11") }
            };
            todos.ForEach(student => context.ToDos.AddOrUpdate(s => s.Contents, student));
            //wow, it does not like this line...
            context.SaveChanges();

            var parts = new List<Part> {
                    new Part { NickName = "FU54", Manufacturer = "Keyence"},
                    new Part { NickName = "FU35", Manufacturer = "Keyence"},
                    new Part { NickName = "IE5827", Manufacturer = "IFM"},
                    new Part { NickName = "MFS200", Manufacturer = "IFM"},
                    new Part { NickName = "ME5010", Manufacturer = "IFM"},
                    new Part { NickName = "M8 Proxy", Manufacturer = "Balluff"},
            };
            parts.ForEach(part => context.Parts.AddOrUpdate(p => p.NickName, part));
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

            //foreach (Enrollment enrollment in enrollments) {
            //    var enrollmentInDataBase = context.Enrollments.Where(
            //        e =>
            //             e.Student.StudentID == enrollment.StudentID &&
            //             e.Course.CourseID == enrollment.CourseID).SingleOrDefault();
            //    if (enrollmentInDataBase == null) {
            //        context.Enrollments.Add(enrollment);
            //    }
            //}

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
            AddOrUpdateTag(context, 1, "Part");
            AddOrUpdateTag(context, 2, "Part");
            AddOrUpdateTag(context, 3, "Part");
            AddOrUpdateTag(context, 4, "Part");
            AddOrUpdateTag(context, 5, "Part");
            AddOrUpdateTag(context, 6, "Part");
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
