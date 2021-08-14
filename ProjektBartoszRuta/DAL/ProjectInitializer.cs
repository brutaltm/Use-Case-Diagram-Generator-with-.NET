using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProjektBartoszRuta.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ProjektBartoszRuta.DAL
{
    public class ProjectInitializer : DropCreateDatabaseIfModelChanges<ProjectContext>
    {
        protected override void Seed(ProjectContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

            roleManager.Create(new IdentityRole("Admin"));
            roleManager.Create(new IdentityRole("User"));

            var user = new ApplicationUser { UserName = "test@test.pl" };
            var user2 = new ApplicationUser { UserName = "test2@test2.pl" };
            var user3 = new ApplicationUser { UserName = "test3@test3.pl" };
            string password = "password";

            var userC = userManager.Create(user, password);
            var userC1 = userManager.Create(user2, password);
            var userC2 = userManager.Create(user3, password);
            userC.Errors.ToList().ForEach(_ => System.Diagnostics.Debug.WriteLine(_));
            userC1.Errors.ToList().ForEach(_ => System.Diagnostics.Debug.WriteLine(_));
            userC2.Errors.ToList().ForEach(_ => System.Diagnostics.Debug.WriteLine(_));
            
            userManager.AddToRole(user.Id, "Admin");
            userManager.AddToRole(user2.Id, "User");
            userManager.AddToRole(user3.Id, "User");

            var profiles = new List<Profile>
            {
                new Profile { UserName = user.UserName },
                new Profile { UserName = user2.UserName },
                new Profile { UserName = user3.UserName }
            };
            profiles.ForEach(_ => context.Profiles.Add(_));
            context.SaveChanges();


            var useCaseDiagrams = new List<UseCaseDiagram>
            {
                new UseCaseDiagram { Name = "Przykładowy diagram p. użycia", Profile = profiles[0], CreatedAt = DateTime.Parse("2019-05-15"), ModifiedAt = DateTime.Parse("2019-05-15") },
                new UseCaseDiagram { Name = "Diagram przypadków użycia", Profile = profiles[0], CreatedAt = DateTime.Parse("2020-01-01"), ModifiedAt = DateTime.Now },
                new UseCaseDiagram { Name = "Przykładowy diagram 2", Profile = profiles[1], CreatedAt = DateTime.Parse("2021-05-15"), ModifiedAt = DateTime.Parse("2021-05-15") }
            };

            //delegata 
            useCaseDiagrams[0].OnDescriptionChanging += (ucd) => Debug.WriteLine(ucd.ID + " - Previous Description: " + ucd.Description);
            useCaseDiagrams[0].OnDescriptionChanged += (ucd) => Debug.WriteLine(ucd.ID + " - Current Description: " + ucd.Description);

            useCaseDiagrams[0].Description = "Nowy opis";

            useCaseDiagrams.ForEach(_ => context.UseCaseDiagrams.Add(_));
            context.SaveChanges();

            var actors = new List<Actor>
            {
                new Actor { Name = "Użytkownik", UseCaseDiagram = useCaseDiagrams[0] },
                new Actor { Name = "Administrator", UseCaseDiagram = useCaseDiagrams[0]},
                new Actor { Name = "Pracownik", UseCaseDiagram = useCaseDiagrams[0]},
            };

            actors.ForEach(_ => context.Actors.Add(_));
            context.SaveChanges();

            var useCases = new List<UseCase>
            {
                new UseCase { Name = "Składanie zamówień", UseCaseDiagram = useCaseDiagrams[0] },
                new UseCase { Name = "Edycja zamówień", UseCaseDiagram = useCaseDiagrams[0] },
                new UseCase { Name = "Zarządzanie personelem", UseCaseDiagram = useCaseDiagrams[0] },
                new UseCase { Name = "Zarządzanie produktami", UseCaseDiagram = useCaseDiagrams[0] },
                new UseCase { Name = "Wyświetlenie informacji o produkcie", UseCaseDiagram = useCaseDiagrams[0] },
            };

            useCases.ForEach(_ => context.UseCases.Add(_));
            context.SaveChanges();

            var useCasePlusActorList = new List<UseCaseActorJoin>
            {
                new UseCaseActorJoin { Actor = actors[0], UseCase = useCases[0] },
                new UseCaseActorJoin { Actor = actors[0], UseCase = useCases[4] },
                new UseCaseActorJoin { Actor = actors[1], UseCase = useCases[0] },
                new UseCaseActorJoin { Actor = actors[1], UseCase = useCases[1] },
                new UseCaseActorJoin { Actor = actors[1], UseCase = useCases[2] },
                new UseCaseActorJoin { Actor = actors[1], UseCase = useCases[3] },
                new UseCaseActorJoin { Actor = actors[1], UseCase = useCases[4] },
                new UseCaseActorJoin { Actor = actors[2], UseCase = useCases[0] },
                new UseCaseActorJoin { Actor = actors[2], UseCase = useCases[1] },
                new UseCaseActorJoin { Actor = actors[2], UseCase = useCases[3] },
                new UseCaseActorJoin { Actor = actors[2], UseCase = useCases[4] },
            };

            useCasePlusActorList.ForEach(_ => context.UseCaseActorJoins.Add(_));
            context.SaveChanges();
        } 
    }
}