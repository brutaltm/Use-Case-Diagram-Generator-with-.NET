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
    [Authorize]
    public class UseCasesController : Controller
    {
        private ProjectContext db = new ProjectContext();

        // GET: UseCases
        public ActionResult Index(int? id)
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            if (id == null)
            {
                var act = roles.Contains("Admin") ? db.UseCases : db.UseCases.Where(_ => _.UseCaseDiagram.Profile.UserName == User.Identity.Name);
                return View(act.ToList());
            }

            var useCaseDiagram = roles.Contains("Admin") ? db.UseCaseDiagrams.Find(id) : db.UseCaseDiagrams.FirstOrDefault(_ => _.ID == id && _.Profile.UserName == User.Identity.Name);

            if (useCaseDiagram == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var useCases = useCaseDiagram.UseCases;
            return View(useCases.ToList());
        }

        // GET: UseCases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var usecase = db.UseCases.Find(id);
            if (usecase == null)
            {
                return HttpNotFound();
            }
            if (!roles.Contains("Admin"))
            {
                usecase = usecase.UseCaseDiagram.Profile.UserName == User.Identity.Name ? usecase : null;
                if (usecase == null)
                    return HttpNotFound();
            }

            return View(usecase);
        }

        // GET: UseCases/Create
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

        // POST: UseCases/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,UseCaseDiagramID,Name,Description")] UseCase useCase)
        {
            if (ModelState.IsValid)
            {
                db.UseCases.Add(useCase);
                db.SaveChanges();
                //return RedirectToAction("Details", "UseCaseDiagrams", new { id = useCase.UseCaseDiagramID });
                return RedirectToAction("Index", new { id = useCase.UseCaseDiagramID });
            }

            ViewBag.UseCaseDiagramID = new SelectList(db.UseCaseDiagrams, "ID", "Name", useCase.UseCaseDiagramID);
            return View(useCase);
        }

        // GET: UseCases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            UseCase useCase = roles.Contains("Admin") ? db.UseCases.Find(id) : db.UseCases.FirstOrDefault(_ => _.ID == id && _.UseCaseDiagram.Profile.UserName == User.Identity.Name);
            var useCaseDiagrams = db.UseCaseDiagrams.Where(_ => _.Profile.UserName == User.Identity.Name);
            if (useCase == null)
            {
                return HttpNotFound();
            }
            ViewBag.UseCaseDiagramID = new SelectList(useCaseDiagrams, "ID", "Name", useCase.UseCaseDiagramID);
            return View(useCase);
        }

        // POST: UseCases/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,UseCaseDiagramID,Name,Description")] UseCase useCase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(useCase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UseCaseDiagramID = new SelectList(db.UseCaseDiagrams, "ID", "Name", useCase.UseCaseDiagramID);
            return View(useCase);
        }

        // GET: UseCases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UseCase useCase = db.UseCases.Find(id);
            if (useCase == null)
            {
                return HttpNotFound();
            }
            return View(useCase);
        }

        // POST: UseCases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UseCase useCase = db.UseCases.Find(id);
            db.UseCases.Remove(useCase);
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
