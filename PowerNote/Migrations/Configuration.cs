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
            var students = new List<Student> {
                new Student { Contents = "Carson",   LastName = "Alexander", 
                    EnrollmentDate = DateTime.Parse("2010-09-01") },
                new Student { Contents = "Meredith", LastName = "Alonso",    
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { Contents = "Arturo",   LastName = "Anand",     
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { Contents = "Gytis",    LastName = "Barzdukas", 
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { Contents = "Yan",      LastName = "Li",        
                    EnrollmentDate = DateTime.Parse("2012-09-01") },
                new Student { Contents = "Peggy",    LastName = "Justice",   
                    EnrollmentDate = DateTime.Parse("2011-09-01") },
                new Student { Contents = "Laura",    LastName = "Norman",    
                    EnrollmentDate = DateTime.Parse("2013-09-01") },
                new Student { Contents = "Nino",     LastName = "Olivetto",  
                    EnrollmentDate = DateTime.Parse("2005-08-11") }
            };
            students.ForEach(student => context.Students.AddOrUpdate(s => s.LastName, student));
            //wow, it does not like this line...
            context.SaveChanges();

            var courses = new List<Course> {
                new Course {CourseID = 1050, Title = "Chemistry",      Credits = 3, },
                new Course {CourseID = 4022, Title = "Microeconomics", Credits = 3, },
                new Course {CourseID = 4041, Title = "Macroeconomics", Credits = 3, },
                new Course {CourseID = 1045, Title = "Calculus",       Credits = 4, },
                new Course {CourseID = 3141, Title = "Trigonometry",   Credits = 4, },
                new Course {CourseID = 2021, Title = "Composition",    Credits = 3, },
                new Course {CourseID = 2042, Title = "Literature",     Credits = 4, }
            };
            courses.ForEach(course => context.Courses.AddOrUpdate(c => c.Title, course));
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
            

            AddOrUpdateCourse(context, "Alexander", "Chemistry");
            AddOrUpdateCourse(context, "Alexander", "Microeconomics");
            AddOrUpdateCourse(context, "Alexander", "Macroeconomics");
            AddOrUpdateCourse(context, "Alonso", "Calculus");
            AddOrUpdateCourse(context, "Alonso", "Trigonometry");
            AddOrUpdateCourse(context, "Alonso", "Composition");
            AddOrUpdateCourse(context, "Anand", "Chemistry");
            AddOrUpdateCourse(context, "Anand", "Microeconomics");
            AddOrUpdateCourse(context, "Barzdukas", "Chemistry");
            AddOrUpdateCourse(context, "Li", "Composition");
            AddOrUpdateCourse(context, "Justice", "Literature");
            context.SaveChanges();
        }

        public void AddOrUpdateCourse(MyContext context, string lastName, string courseTitle) {
            var student = context.Students.SingleOrDefault(s => s.LastName == lastName);
            var course = student.Courses.SingleOrDefault(c => c.Title == courseTitle);
            if (course == null)
                student.Courses.Add(context.Courses.Single(c => c.Title == courseTitle));
        }

    }
}
