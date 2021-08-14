using ProjektBartoszRuta.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ProjektBartoszRuta.DAL
{
    public class ProjectContext : DbContext
    {
        public ProjectContext() : base("DefaultConnection")
        {
            //Database.SetInitializer(new ProjectInitializer());
        }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<UseCase> UseCases { get; set; }
        public DbSet<UseCaseDiagram> UseCaseDiagrams { get; set; }
        public DbSet<UseCaseActorJoin> UseCaseActorJoins { get; set; }
        public DbSet<Profile> Profiles { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            //modelBuilder.Entity<UseCaseDiagram>().HasMany(d => d.Actors).WithOptional().WillCascadeOnDelete(true);
            //modelBuilder.Entity<Actor>().HasRequired(d => d.UseCaseDiagram).WithMany().WillCascadeOnDelete(true);
            //modelBuilder.Entity<UseCase>().HasRequired(d => d.UseCaseDiagram).WithMany().WillCascadeOnDelete(true);
            //modelBuilder.Entity<UseCaseActorJoin>().HasRequired(d => d.Actor).WithMany().WillCascadeOnDelete(true);
            //modelBuilder.Entity<UseCaseActorJoin>().HasRequired(d => d.UseCase).WithMany().WillCascadeOnDelete(true);
        }

    }
}