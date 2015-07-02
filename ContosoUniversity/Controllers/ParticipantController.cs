using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity.Infrastructure;


namespace ContosoUniversity.Controllers
{
    public class ParticipantController : Controller
    {
        private ProcurementEntities db = new ProcurementEntities();
        
        // GET: Participant
        public ViewResult Index(string sortOrder, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            List<Participant> participants = Participant.QueryAll();
            switch (sortOrder)
            {
                case "name_desc":
                    participants = participants.OrderByDescending(p => p.name).ToList();
                    break;
                default:  // Name ascending 
                    participants = participants.OrderBy(p => p.name).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(participants.ToPagedList(pageNumber, pageSize));
        }


        // GET: Participant/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Participant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name")]Participant participant)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Participant.Add(participant);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)// dex)
            {
                
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Произошла ошибка при сохранении записи. При повторении ошибки обратитесь к администратору");
            }
            return View(participant);
        }

        // GET: Participant/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant Participant = db.Participant.Find(id);
            if (Participant == null)
            {
                return HttpNotFound();
            }
            return View(Participant);
        }

        // POST: Participant/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant ParticipantToUpdate = db.Participant.Find(id);
            if (TryUpdateModel(ParticipantToUpdate, "",  new string[] { "name" }))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)// dex )
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Произошла ошибка при изменении записи. При повторении ошибки обратитесь к администратору");
                }
            }
            return View(ParticipantToUpdate);
        }

        // GET: Participant/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Participant Participant = db.Participant.Find(id);
            if (Participant == null)
            {
                return HttpNotFound();
            }
            return View(Participant);
        }

        // GET: Participant/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Произошла ошибка при удалении записи. При повторении ошибки обратитесь к администратору";
            }
            Participant Participant = db.Participant.Find(id);
            if (Participant == null)
            {
                return HttpNotFound();
            }
            return View(Participant);
        }

        // POST: Participant/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Participant Participant = db.Participant.Find(id);
                db.Participant.Remove(Participant);
                db.SaveChanges();
            }
            catch (RetryLimitExceededException )// dex )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
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
