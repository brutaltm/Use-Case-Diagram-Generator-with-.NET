using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using ProjektBartoszRuta.DAL;
using ProjektBartoszRuta.Models;

namespace ProjektBartoszRuta.Controllers
{
    public class ActorsController : Controller
    {
        private ProjectContext db = new ProjectContext();

        // GET: Actors
        [Authorize(Roles = "Admin, User")]
        public ActionResult Index(int? id)
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (id == null)
            {
                var act = roles.Contains("Admin") ? db.Actors : db.Actors.Where(_ => _.UseCaseDiagram.Profile.UserName == User.Identity.Name);
                return View(act.ToList());
            }
            
            var useCaseDiagram = roles.Contains("Admin") ? db.UseCaseDiagrams.Find(id) : db.UseCaseDiagrams.FirstOrDefault(_ => _.ID == id && _.Profile.UserName == User.Identity.Name);
            
            if (useCaseDiagram == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var actors = useCaseDiagram.Actors;
            //var actors = db.Actors.Include(a => a.UseCaseDiagram);
            return View(actors.ToList());
        }

        // GET: Actors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            Actor actor = db.Actors.Find(id);
            if (actor == null)
            {
                return HttpNotFound();
            }
            if (!roles.Contains("Admin"))
            {
                actor = actor.UseCaseDiagram.Profile.UserName == User.Identity.Name ? actor : null;
                if (actor == null)
                    return HttpNotFound();
            }

            return View(actor);
        }

        // GET: Actors/Create
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create(int? id)
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var useCaseDiagrams = db.UseCaseDiagrams.Where(_ => _.Profile.UserName == User.Identity.Name);
            if (useCaseDiagrams.Count() == 0)
            {
                return HttpNotFound();
            }
            ViewBag.UseCaseDiagramID = new SelectList(useCaseDiagrams, "ID", "Name", id);
            return View();
        }

        // POST: Actors/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UseCaseDiagramID,Name")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                db.Actors.Add(actor);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = actor.UseCaseDiagramID });
            }

            ViewBag.UseCaseDiagramID = new SelectList(db.UseCaseDiagrams, "ID", "Name", actor.UseCaseDiagramID);
            return View(actor);
        }

        // GET: Actors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            Actor actor = roles.Contains("Admin") ? db.Actors.Find(id) : db.Actors.FirstOrDefault(_ => _.ID == id && _.UseCaseDiagram.Profile.UserName == User.Identity.Name);
            var useCaseDiagrams = db.UseCaseDiagrams.Where(_ => _.Profile.UserName == User.Identity.Name);
            if (actor == null)
            {
                return HttpNotFound();
            }
            ViewBag.UseCaseDiagramID = new SelectList(useCaseDiagrams, "ID", "Name", actor.UseCaseDiagramID);

            return View(actor);
        }

        // POST: Actors/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UseCaseDiagramID,Name")] Actor actor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(actor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = actor.ID });
            }
            ViewBag.UseCaseDiagramID = new SelectList(db.UseCaseDiagrams, "ID", "Name", actor.UseCaseDiagramID);
            return View(actor);
        }

        // GET: Actors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            Actor actor = roles.Contains("Admin") ? db.Actors.Find(id) : db.Actors.FirstOrDefault(_ => _.ID == id && _.UseCaseDiagram.Profile.UserName == User.Identity.Name);
            if (actor == null)
            {
                return HttpNotFound();
            }
            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Actor actor = db.Actors.Find(id);
            db.Actors.Remove(actor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
