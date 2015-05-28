using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContosoUniversity.DAL;
using ContosoUniversity.Models;
using PagedList;
using System.Data.Entity.Infrastructure;
using Newtonsoft.Json;


namespace ContosoUniversity.Controllers
{
    public class BidController : Controller
    {
        private ProcurementEntities db = new ProcurementEntities();

        // GET: Bid
        public ViewResult Index()
        {
            return View();
        }


        // GET: Bid/Table
        public ActionResult Table(int? _tenderId)
        {
            _tenderId = Tender.QueryAll().First().id;
            return View(new BidContainer(_tenderId));
        }

        // POST: Bid/Table
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Table(string table, int tenderId)
        {
            
            try
            {
                Dictionary<int, Dictionary<int, double>> bids = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, double>>>(table);
                Tender.byId(tenderId).setBids(bids);
                db.SaveChanges();
                return View("Tender", Tender.byId(tenderId));

            }
            catch //(RetryLimitExceededException)// dex)
            {
                
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Произошла ошибка при сохранении записи. При повторении ошибки обратитесь к администратору");
                return View("Error");
            }
            return View();
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
