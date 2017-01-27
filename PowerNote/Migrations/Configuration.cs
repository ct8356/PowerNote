using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using PowerNote.ViewModels;
using PowerNote.DAL;
using CJT.ViewModels;
using CJT.Models;
using System.Collections.ObjectModel;
using EFContext = PowerNote.DAL.EFContext;

namespace PowerNote.Migrations {

    internal sealed class Configuration : DbMigrationsConfiguration<EFContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = true; //was False, but error suggested
            //I change it to true
        }

        public EFContext DbContext { get; set; }

        public void callSeed(EFContext context) {
            Seed(context);
        }
        //RIGHT, so this configuration, WON'T DO nice migrations.
        //BUT, can look into that, when really need it.
        //For now, can just delete, and recreate the database.
        //NOW I have updated to EF v6, will it do nice migs now?

        protected override void Seed(EFContext context) {
            DbContext = context;
            //OK! Perhaps, it is actually THIS method, that creates the database.
            //NOT the create context instantiation...
            //(I guess create(), will still do the trick. but NOT context instantiation...nec.) ??

            string type = "Type";
            string gender = "Gender";
            string parent = "Parent";
            string friend = "Friend";

            var relationships = new List<Relationship> {
                newRelation("Type", "Type", type),
                newRelation("Type", "Person", type),
                newRelation("Type", "Gender", type),
                newRelation("Gender", "Male", type),
                newRelation("Gender", "Female", type),
                newRelation("Person", "Michael", type),
                newRelation("Person", "Valerie", type),
                newRelation("Person", "Meryan", type),
                newRelation("Person", "Josh", type),
                newRelation("Person", "Chris", type),
                newRelation("Person", "Anya", type),
                newRelation("Person", "Ella", type),
                newRelation("Male", "Michael", gender),
                newRelation("Male", "Josh", gender),
                newRelation("Male", "Chris", gender),
                newRelation("Female", "Valerie", gender),
                newRelation("Female", "Meryan", gender),
                newRelation("Female", "Anya", gender),
                newRelation("Female", "Ella", gender),
                newRelation("Michael", "Meryan", parent),
                newRelation("Valerie", "Meryan", parent),
                newRelation("Meryan", "Josh", parent),
                newRelation("Meryan", "Chris", parent),
                newRelation("Meryan", "Anya", parent),
                newRelation("Meryan", "Ella", parent),
                newRelation("Josh", "Chris", friend)
                //createEntry(Name,Type)
                //createRelation(entry, entry, relationName);
                //NOTE: above is the way forward (i.e. using strings).
                //COZ THEN, does not matter WHERE you define the variable first!
            };
            relationships.ForEach(en => context.Relationships.AddOrUpdate(e => e.EntryID, en));
            context.SaveChanges();

            // OTHER WAY. Maybe works, if you just start with parents, and work down.
            //FIRST, need to make a treeVM. (so hs DBContext).
            Node typeNode = DbContext.Nodes.Where(n => n.Name == "Type").First();
            string device = "Device";
            string job = "Job";
            string signal = "Signal";
            //TYPES
            Entry jobNode = new NodeVM("Job", DbContext).AddRelation(type, typeNode).Entry;
            Entry input = new NodeVM("Input", DbContext).AddRelation(type, typeNode).Entry;
            Entry output = new NodeVM("Output", DbContext).AddRelation(type, typeNode).Entry;
            Entry deviceNode = new NodeVM("Device", DbContext).AddRelation(type, typeNode).Entry;
            //JOBS
            Entry j5944 = new NodeVM("5944", DbContext).AddRelation(type, jobNode).Entry;
            //IO SIGNALS
            Entry lhClampDown = new NodeVM("LH clamp down", DbContext).AddRelation(type, input)
                .AddRelation(job, j5944).Entry;
            Entry rhClampDown = new NodeVM("RH clamp down", DbContext).AddRelation(type, input)
                .AddRelation(job, j5944).Entry;
            Entry extendShotBolts = new NodeVM("Extend shot bolts", DbContext).AddRelation(type, output)
                .AddRelation(job, j5944).Entry;
            //DEVICES (NOTE, these I think should one day be automatically generated!)
            //NOTE yo! Names cannot be unique! ONLY unique in the context.
            Entry proxy1 = new NodeVM("Proxy 1", DbContext).AddRelation(type, deviceNode)
                .AddRelation(signal, lhClampDown).Entry;
            Entry proxy2 = new NodeVM("Proxy 2", DbContext).AddRelation(type, deviceNode)
                .AddRelation(signal, rhClampDown).Entry;
            Entry valve1 = new NodeVM("Valve 1", DbContext).AddRelation(type, deviceNode)
                .AddRelation(signal, extendShotBolts).Entry;
        } //NOTE YO! Making a builderClass, would actually be BETTER than using constructors I think!
        //YES! Seems to be way forward! 
        //DEFO the way to go! COZ EntryVMs require MainVMs! That's too painful!
        //ALSO, scripts, i.e. methods for here, can be very effective...

        protected Relationship newRelation(string parent, string child, string relationType) {
            Node entry1;
            Node entry2;
            if (DbContext.Nodes.Select(e => e.Name).Contains(parent)) {
                entry1 = DbContext.Nodes.Where(e => e.Name == parent).First();
            } else {
                entry1 = new Node(parent);
                DbContext.Nodes.AddOrUpdate(e => e.EntryID, entry1);
            }
            DbContext.SaveChanges();
            if (DbContext.Nodes.Select(e => e.Name).Contains(child)) {
                entry2 = DbContext.Nodes.Where(e => e.Name == child).First();
            }
            else {
                entry2 = new Node(child);
                DbContext.Nodes.AddOrUpdate(e => e.EntryID, entry2);
            }
            DbContext.SaveChanges();
            Relationship relation = new Relationship(entry1, entry2, relationType);
            DbContext.SaveChanges();
            return relation;
        }

    }
}
