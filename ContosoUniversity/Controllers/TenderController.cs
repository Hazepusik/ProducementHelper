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


namespace ContosoUniversity.Controllers
{
    public class TenderController : Controller
    {
        private ProcurementEntities db = new ProcurementEntities();



        // GET: Tender
        public ViewResult Index(string sortOrder, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            List<Tender> tenders = Tender.QueryAll();
            switch (sortOrder)
            {
                case "name_desc":
                    tenders = tenders.OrderByDescending(p => p.name).ToList();
                    break;
                default:  // Name ascending 
                    tenders = tenders.OrderBy(p => p.name).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(tenders.ToPagedList(pageNumber, pageSize));
        }


        // GET: Tender/Create
        public ActionResult Create()
        {
            Tender t = new Tender();
            return View(t);
        }

        // POST: Tender/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name, description, minPrice, maxPrice, participantIds, propertyIds")]Tender tender)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Tender.Add(tender);
                    db.SaveChanges();
                    // set default
                    foreach (int participantId in tender.participantIds)
                    {
                        Bid bid = new Bid();
                        bid.tenderId = tender.id;
                        bid.participantId = participantId;
                        bid.defaultValue = 0;
                        db.Bid.Add(bid);
                    }
                    db.SaveChanges();

                    foreach (int propertyId in tender.propertyIds)
                    {
                        Property property = Property.byId(propertyId).Clone();
                        //db.SaveChanges();
                        foreach (int participantId in tender.participantIds)
                        {
                            Bid bid = new Bid();
                            bid.tenderId = tender.id;
                            bid.participantId = participantId;
                            bid.propertyId = property.id;
                            bid.defaultValue = 0;
                            db.Bid.Add(bid);
                        }
                    }
                    db.SaveChanges();
                    return RedirectToAction("Edit", new { id = tender.id });
                }
            }
            catch (RetryLimitExceededException)// dex)
            {
                
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Произошла ошибка при сохранении записи. При повторении ошибки обратитесь к администратору");
            }
            return View(tender);
        }

        // GET: Tender/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tender Tender = db.Tender.Find(id);
            if (Tender == null)
            {
                return HttpNotFound();
            }
            return View(Tender);
        }

        // POST: Tender/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tender TenderToUpdate = db.Tender.Find(id);
            List<int> oldParticipants = TenderToUpdate.participantIds;
            List<int> oldProperties = TenderToUpdate.propertyIds;
            if (TryUpdateModel(TenderToUpdate, "", new string[] { "name", "description", "minPrice", "maxPrice", "participantIds", "propertyIds" }))
            {
                try
                {
                    db.SaveChanges();
                    List<int> propToRm = oldProperties.Except(TenderToUpdate.propertyIds).ToList();
                    List<int> propToAdd = TenderToUpdate.propertyIds.Except(oldProperties).ToList();
                    List<int> partToRm = oldParticipants.Except(TenderToUpdate.participantIds).ToList();
                    List<int> partToAdd = TenderToUpdate.participantIds.Except(oldParticipants).ToList();

                    List<Bid> bidsToRm = db.Bid.Where(b => partToRm.Contains(b.participantId)
                                                            || propToRm.Contains(b.propertyId ?? 0)).ToList();
                    db.Bid.RemoveRange(bidsToRm);
                    db.SaveChanges();

                    for (int cnt = 0; cnt < propToAdd.Count(); ++cnt )
                    {
                        propToAdd[cnt] = Property.byId(propToAdd[cnt]).Clone().id;
                    }

                    propToAdd = propToAdd.Concat(oldProperties.Where(op => !propToRm.Contains(op))).Distinct().ToList();
                    partToAdd = partToAdd.Concat(oldParticipants.Where(op => !partToRm.Contains(op))).Distinct().ToList();

                    foreach (int pyId in propToAdd)
                    {
                        foreach (int ptId in partToAdd)
                        {
                            if (db.Bid.FirstOrDefault(b => b.participantId == ptId &&
                                                    b.propertyId == pyId &&
                                                    b.tenderId == TenderToUpdate.id) == null)
                            {
                                Bid bid = new Bid();
                                bid.tenderId = TenderToUpdate.id;
                                bid.participantId = ptId;
                                bid.propertyId = pyId;
                                bid.defaultValue = 0;
                                db.Bid.Add(bid);
                            }
                        }
                    }
                    db.SaveChanges();
                    return RedirectToAction("Edit", new { id = id });
                }
                catch (RetryLimitExceededException)// dex )
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Произошла ошибка при изменении записи. При повторении ошибки обратитесь к администратору");
                }
            }
            return View(TenderToUpdate);
        }

        // GET: Tender/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tender Tender = db.Tender.Find(id);
            if (Tender == null)
            {
                return HttpNotFound();
            }
            return View(Tender);
        }


        // GET: Tender/Result/5
        public ActionResult Result(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tender Tender = db.Tender.Find(id);
            if (Tender == null)
            {
                return HttpNotFound();
            }
            return View(Tender);
        }

        // GET: Tender/Delete/5
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
            Tender Tender = db.Tender.Find(id);
            if (Tender == null)
            {
                return HttpNotFound();
            }
            return View(Tender);
        }

        // POST: Tender/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Tender Tender = db.Tender.Find(id);
                db.Tender.Remove(Tender);
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
