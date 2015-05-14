namespace PowerNote.Migrations {
    using System;
    using System.Data.Entity.Migrations;
    //NOTE:
    //This file needs to be called, to CREATE the database. (the up method, that is).
    //THEN: configuration.cs, the see method is called, to fill the database, with data.
    //PROBLEM IS: this file only MATCHES your data model (i.e. code first) if use package manager console.
    //TO use that, in C# express, you need VS Express 2010...
    //SO, are you happy writing this file yourself?
    //Not that hard, tbh...

    //BUT, its possible that the DbContext class (the clever one here, that allows easy ORM stuff),
    //CAN create these things for you... (actually, sure that is not possible. Need a VStudio EXTENSION to auto-gen code..).
    //YES BUT, does not need to gen code... Just needs to gen the database!!! With the info, it has!!!
    //BUT anyway, see the DbContext webpage, see if it does have any createDb methods, OR interesting methods...
    //SEE DbMigration class too...
    public partial class InitialCreate : DbMigration {
        public override void Up() {
            CreateTable(
                "dbo.Student",
                c => new {
                    StudentID = c.Int(nullable: false, identity: true),
                    LastName = c.String(),
                    FirstMidName = c.String(),
                    EnrollmentDate = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.StudentID);

            CreateTable(
                "dbo.Enrollment",
                c => new {
                    EnrollmentID = c.Int(nullable: false, identity: true),
                    CourseID = c.Int(nullable: false),
                    StudentID = c.Int(nullable: false),
                    Grade = c.Int(),
                })
                .PrimaryKey(t => t.EnrollmentID)
                .ForeignKey("dbo.Course", t => t.CourseID, cascadeDelete: true)
                .ForeignKey("dbo.Student", t => t.StudentID, cascadeDelete: true)
                .Index(t => t.CourseID)
                .Index(t => t.StudentID);

            CreateTable(
                "dbo.Course",
                c => new {
                    CourseID = c.Int(nullable: false),
                    Title = c.String(),
                    Credits = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.CourseID);

        }

        public override void Down() {
            DropIndex("dbo.Enrollment", new[] { "StudentID" });
            DropIndex("dbo.Enrollment", new[] { "CourseID" });
            DropForeignKey("dbo.Enrollment", "StudentID", "dbo.Student");
            DropForeignKey("dbo.Enrollment", "CourseID", "dbo.Course");
            DropTable("dbo.Course");
            DropTable("dbo.Enrollment");
            DropTable("dbo.Student");
        }
    }
}
