using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ProjektBartoszRuta.DAL;
using ProjektBartoszRuta.Models;
using PagedList;
using System.Security.Claims;
using ProjektBartoszRuta.Services;
using System.Web;
using System.Diagnostics;

namespace ProjektBartoszRuta.Controllers
{
    public delegate void DiagramDelegate(UseCaseDiagram ucd);
    public class UseCaseDiagramsController : Controller
    {
        public event DiagramDelegate OnCreated;
        public event DiagramDelegate OnCreating;
        public event DiagramDelegate OnEdited;
        public event DiagramDelegate OnEditing;
        public event DiagramDelegate OnDeleted;
        public event DiagramDelegate OnDeleting;
        private ProjectContext db = new ProjectContext();
        private IDiagramService diagramService;

        public UseCaseDiagramsController()
        {
            OnCreated += (ucd) => Debug.WriteLine("Utworzono nowy diagram: " + ucd.Name + ", " + ucd.Description);
            OnDeleting += (ucd) => Debug.WriteLine("Usuwanie diagramu: " + ucd.Name + ", " + ucd.Description);
            OnEditing += (ucd) => Debug.WriteLine("Edytowanie diagramu. Przed zapisem w bazie.");
            OnEdited += (ucd) => Debug.WriteLine("Edytowanie diagramu. Stan po: " + ucd.Name + ", " + ucd.Description);

            diagramService = new DiagramService();
            diagramService.onPDFCreating += (path, width, height) => Debug.WriteLine("Stworzono pdf w lokalizacji: " + path + ". Wymiary strony: " + width + "x" + height);
            diagramService.onPNGCreated += (path, width, height, fi) => Debug.WriteLine("Stworzono PNG w lokalizacji: " + path + ". Wymiary obrazka: " + width + "x" + height + ". Rozmiar(bytes): " + fi.Length);
        }

        // GET: UseCaseDiagrams
        [Authorize(Roles = "Admin, User")]
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DescSortParm = sortOrder == "desc" ? "desc_desc" : "desc";
            ViewBag.DateSortParm = sortOrder == "date" ? "date_desc" : "date";
            ViewBag.ModifiedSortParm = sortOrder == "modified" ? "modified_desc" : "modified";

            if (searchString != null)
                page = 1;
            else
                searchString = currentFilter;
            ViewBag.CurrentFilter = searchString;

            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            var useCaseDiagrams = roles.Contains("Admin") ? db.UseCaseDiagrams.Include(u => u.Profile) : db.UseCaseDiagrams.Where(_ => _.Profile.UserName == User.Identity.Name).Include(u => u.Profile);

            if (!String.IsNullOrEmpty(searchString))
            {
                useCaseDiagrams = useCaseDiagrams.Where(s => s.Name.Contains(searchString) || s.Description.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    useCaseDiagrams = useCaseDiagrams.OrderByDescending(s => s.Name);
                    break;
                case "desc":
                    useCaseDiagrams = useCaseDiagrams.OrderBy(s => s.Description);
                    break;
                case "desc_desc":
                    useCaseDiagrams = useCaseDiagrams.OrderByDescending(s => s.Description);
                    break;
                case "date":
                    useCaseDiagrams = useCaseDiagrams.OrderBy(s => s.CreatedAt);
                    break;
                case "date_desc":
                    useCaseDiagrams = useCaseDiagrams.OrderByDescending(s => s.CreatedAt);
                    break;
                case "modified":
                    useCaseDiagrams = useCaseDiagrams.OrderBy(s => s.ModifiedAt);
                    break;
                case "modified_desc":
                    useCaseDiagrams = useCaseDiagrams.OrderByDescending(s => s.ModifiedAt);
                    break;
                default:
                    useCaseDiagrams = useCaseDiagrams.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            //return View(useCaseDiagrams.ToList());
            return View(useCaseDiagrams.ToPagedList(pageNumber, pageSize));
        }

        // GET: UseCaseDiagrams/Details/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            UseCaseDiagram useCaseDiagram = roles.Contains("Admin") ? db.UseCaseDiagrams.Find(id) : db.UseCaseDiagrams.FirstOrDefault(_ => _.ID == id && _.Profile.UserName == User.Identity.Name);
            if (useCaseDiagram == null)
            {
                return HttpNotFound();
            }
            return View(useCaseDiagram);
        }

        // GET: UseCaseDiagrams/Create
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create()
        {
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            ViewBag.ProfileID = roles.Contains("Admin") ? new SelectList(db.Profiles, "ID", "UserName") : new SelectList(db.Profiles.Where(_ => _.UserName == User.Identity.Name), "ID", "UserName");
            return View();
        }

        // POST: UseCaseDiagrams/Create
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Create([Bind(Include = "ID,ProfileID,Name,Description,CreatedAt,ModifiedAt")] UseCaseDiagram useCaseDiagram)
        {
            if (ModelState.IsValid)
            {
                if (useCaseDiagram.ID % 2 == 1) // dla nieparzystych 
                { 
                    useCaseDiagram.OnDescriptionChanging += (ucd) => Debug.WriteLine(ucd.ID + " - Previous Description: " + ucd.Description);
                    useCaseDiagram.OnDescriptionChanged += (ucd) => Debug.WriteLine(ucd.ID + " - Current Description: " + ucd.Description);
                }
                useCaseDiagram.CreatedAt = DateTime.Now;
                useCaseDiagram.ModifiedAt = DateTime.Now;
                OnCreating?.Invoke(useCaseDiagram);
                db.UseCaseDiagrams.Add(useCaseDiagram);
                db.SaveChanges();
                OnCreated?.Invoke(useCaseDiagram);
                return RedirectToAction("Index");
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", useCaseDiagram.ProfileID);
            return View(useCaseDiagram);
        }

        // GET: UseCaseDiagrams/Edit/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            UseCaseDiagram useCaseDiagram = roles.Contains("Admin") ? db.UseCaseDiagrams.Find(id) : db.UseCaseDiagrams.FirstOrDefault(_ => _.ID == id && _.Profile.UserName == User.Identity.Name);
            if (useCaseDiagram == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProfileID = new SelectList(roles.Contains("Admin") ? db.Profiles : db.Profiles.Where(_ => _.UserName == User.Identity.Name), 
                "ID", "UserName", useCaseDiagram.ProfileID);
            return View(useCaseDiagram);
        }

        // POST: UseCaseDiagrams/Edit/5
        // Aby zapewnić ochronę przed atakami polegającymi na przesyłaniu dodatkowych danych, włącz określone właściwości, z którymi chcesz utworzyć powiązania.
        // Aby uzyskać więcej szczegółów, zobacz https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, User")]
        public ActionResult Edit([Bind(Include = "ID,ProfileID,Name,Description")] UseCaseDiagram useCaseDiagram)
        {
            if (ModelState.IsValid)
            {
                useCaseDiagram.CreatedAt = db.UseCaseDiagrams.AsNoTracking().Where(_ => _.ID == useCaseDiagram.ID).First().CreatedAt;
                useCaseDiagram.ModifiedAt = DateTime.Now;
                OnEditing?.Invoke(useCaseDiagram);

                db.Entry(useCaseDiagram).State = EntityState.Modified;
                db.SaveChanges();
                OnEdited?.Invoke(useCaseDiagram);
                return RedirectToAction("Index");
            }
            ViewBag.ProfileID = new SelectList(db.Profiles, "ID", "UserName", useCaseDiagram.ProfileID);
            return View(useCaseDiagram);
        }

        // GET: UseCaseDiagrams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UseCaseDiagram useCaseDiagram = db.UseCaseDiagrams.Find(id);
            if (useCaseDiagram == null)
            {
                return HttpNotFound();
            }
            return View(useCaseDiagram);
        }

        // POST: UseCaseDiagrams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UseCaseDiagram useCaseDiagram = db.UseCaseDiagrams.Find(id);
            OnDeleting?.Invoke(useCaseDiagram);
            db.UseCaseDiagrams.Remove(useCaseDiagram);
            db.SaveChanges();
            OnDeleted?.Invoke(useCaseDiagram);
            return RedirectToAction("Index");
        }

        // GET: UseCaseDiagrams/GeneratePDF/5
        [Authorize(Roles = "Admin, User")]
        public ActionResult GeneratePDF(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var roles = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            UseCaseDiagram useCaseDiagram = roles.Contains("Admin") ? db.UseCaseDiagrams.Find(id) : db.UseCaseDiagrams.FirstOrDefault(_ => _.ID == id && _.Profile.UserName == User.Identity.Name);
            if (useCaseDiagram == null)
            {
                return HttpNotFound();
            }
            var pathPDF = HttpContext.Server.MapPath("~/Images/") + "diagram" + id + ".pdf";
            diagramService.DiagramToPdf(pathPDF, useCaseDiagram);
            diagramService.PdfToPNG(pathPDF, HttpContext.Server.MapPath("~/Images/") + "diagramSmall" + id + ".png", 300, 200);
            diagramService.PdfToPNG(pathPDF, HttpContext.Server.MapPath("~/Images/") + "diagramLarge" + id + ".png", 700, 600);
            useCaseDiagram.IsGenerated = true;
            useCaseDiagram.ModifiedAt = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Details", "UseCaseDiagrams", new { id = id });
        }
        public ActionResult DownloadExampleFiles(string fileName)
        {
            var path = HttpContext.Server.MapPath("~/Images/") + fileName;
            byte[] filedata = System.IO.File.ReadAllBytes(path);
            string contentType = MimeMapping.GetMimeMapping(path);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = fileName,
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        public ActionResult GenerateCustomPNG(int ID, int width)
        {
            var pathPDF = HttpContext.Server.MapPath("~/Images/") + "diagram" + ID + ".pdf";
            var path = HttpContext.Server.MapPath("~/Images/") + "diagramCustom" + ID + ".png";
            diagramService.PdfToPNG(pathPDF, path, width, width);
            //string filename = "File.pdf";
            //string filepath = AppDomain.CurrentDomain.BaseDirectory + "/Path/To/File/" + filename;
            byte[] filedata = System.IO.File.ReadAllBytes(path);
            string contentType = MimeMapping.GetMimeMapping(path);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "diagramCustom" + ID + ".png",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
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
