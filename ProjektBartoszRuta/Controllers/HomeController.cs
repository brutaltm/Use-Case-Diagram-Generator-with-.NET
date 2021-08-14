using ProjektBartoszRuta.DAL;
using ProjektBartoszRuta.Models;
using ProjektBartoszRuta.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjektBartoszRuta.Controllers
{
    public class HomeController : Controller
    {
        private ProjectContext db = new ProjectContext();

        public ActionResult Index(ProjectContext context)
        {
            //Profile profile = db.Profiles.Single(_ => _.UserName == User.Identity.Name);
            //ViewBag.Diagrams = profile.UseCaseDiagrams;
            //Console.WriteLine(User.Identity.Name);
            //context.Profiles.Single(_ => _.UserName == User.Identity.Name);
            //ViewBag.Profile = context.Profiles.SingleOrDefault(_ => _.UserName.Length > 0);
            //ViewBag.UseCaseDiagram = context.UseCaseDiagrams.First(_ => _ != null);
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult About()
        {
            ViewBag.Message = "Zestawienie liczby diagramów do użytkowników.";
            IQueryable<DateGroup> data = from diagram in db.UseCaseDiagrams
                group diagram by diagram.ProfileID into dateGroup
                select new DateGroup()
                {
                    ProfileID = dateGroup.Key,
                    DiagramCount = dateGroup.Count()
                };
            return View(data.ToList());
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}