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
    public class UseCaseActorJoinsController : Controller
    {
        private ProjectContext db = new ProjectContext();

        // GET: UseCaseActorJoins
        public ActionResult Index(int? id)
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (id == null)
            {
                var joins = roles.Contains("Admin") ? db.UseCaseActorJoins : db.UseCaseActorJoins.Where(_ => _.Actor.UseCaseDiagram.Profile.UserName == User.Identity.Name);
                return View(joins.ToList());
            }

            var useCaseDiagram = roles.Contains("Admin") ? db.UseCaseDiagrams.Find(id) : db.UseCaseDiagrams.FirstOrDefault(_ => _.ID == id && _.Profile.UserName == User.Identity.Name);

            if (useCaseDiagram == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var useCaseActorJoins = useCaseDiagram.Actors.SelectMany(_ => _.UseCaseActorJoins);
            //var actors = db.Actors.Include(a => a.UseCaseDiagram);
            return View(useCaseActorJoins.ToList());
            //var useCaseActorJoins = db.UseCaseActorJoins.Include(u => u.Actor).Include(u => u.UseCase);
            //return View(useCaseActorJoins.ToList());
        }

        // GET: UseCaseActorJoins/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UseCaseActorJoin useCaseActorJoin = db.UseCaseActorJoins.Find(id);

            if (useCaseActorJoin == null)
            {
                return HttpNotFound();
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (!roles.Contains("Admin"))
            {
                useCaseActorJoin = useCaseActorJoin.Actor.UseCaseDiagram.Profile.UserName == User.Identity.Name ? useCaseActorJoin : null;
                if (useCaseActorJoin == null)
                    return HttpNotFound();
            }
            return View(useCaseActorJoin);
        }

        // GET: UseCaseActorJoins/Create
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create(int? id, int? actor, int? useCase)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            UseCaseDiagram useCaseDiagram = db.UseCaseDiagrams.FirstOrDefault(_ => _.ID == id && _.Profile.UserName == User.Identity.Name);
            if (useCaseDiagram == null)
            {
                return HttpNotFound();
            }
            //ViewBag.UseCaseDiagramID = useCaseDiagram.ID;
            ViewBag.ActorID = actor == null ? new SelectList(useCaseDiagram.Actors, "ID", "Name") : new SelectList(useCaseDiagram.Actors, "ID", "Name", actor);
            ViewBag.UseCaseID = useCase == null ? new SelectList(useCaseDiagram.UseCases, "ID", "Name") : new SelectList(useCaseDiagram.UseCases, "ID", "Name", useCase);
            return View();
        }

        // POST: UseCaseActorJoins/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,ActorID,UseCaseID")] UseCaseActorJoin useCaseActorJoin)
        {
            if (ModelState.IsValid)
            {
                db.UseCaseActorJoins.Add(useCaseActorJoin);
                db.SaveChanges();
                return RedirectToAction("Details", "UseCaseDiagrams", new { id = db.Actors.Find(useCaseActorJoin.ActorID).UseCaseDiagramID });
            }

            ViewBag.ActorID = new SelectList(db.Actors, "ID", "Name", useCaseActorJoin.ActorID);
            ViewBag.UseCaseID = new SelectList(db.UseCases, "ID", "Name", useCaseActorJoin.UseCaseID);
            return View(useCaseActorJoin);
        }

        // GET: UseCaseActorJoins/Edit/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UseCaseActorJoin useCaseActorJoin = db.UseCaseActorJoins.Find(id);
            if (useCaseActorJoin == null)
            {
                return HttpNotFound();
            }
            UseCaseDiagram useCaseDiagram = useCaseActorJoin.Actor.UseCaseDiagram;
            if (useCaseDiagram == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActorID = new SelectList(useCaseDiagram.Actors, "ID", "Name", useCaseActorJoin.ActorID);
            ViewBag.UseCaseID = new SelectList(useCaseDiagram.UseCases, "ID", "Name", useCaseActorJoin.UseCaseID);
            return View(useCaseActorJoin);
        }

        // POST: UseCaseActorJoins/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,ActorID,UseCaseID")] UseCaseActorJoin useCaseActorJoin)
        {
            if (ModelState.IsValid)
            {
                db.Entry(useCaseActorJoin).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActorID = new SelectList(db.Actors, "ID", "Name", useCaseActorJoin.ActorID);
            ViewBag.UseCaseID = new SelectList(db.UseCases, "ID", "Name", useCaseActorJoin.UseCaseID);
            return View(useCaseActorJoin);
        }

        // GET: UseCaseActorJoins/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UseCaseActorJoin useCaseActorJoin = db.UseCaseActorJoins.FirstOrDefault(_ => _.ID == id && _.Actor.UseCaseDiagram.Profile.UserName == User.Identity.Name);
            if (useCaseActorJoin == null)
            {
                return HttpNotFound();
            }
            return View(useCaseActorJoin);
        }

        // POST: UseCaseActorJoins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UseCaseActorJoin useCaseActorJoin = db.UseCaseActorJoins.Find(id);
            db.UseCaseActorJoins.Remove(useCaseActorJoin);
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
