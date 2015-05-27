﻿using System;
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
                        db.Bid.Add(bid);
                    }
                    db.SaveChanges();
                    foreach (int propertyId in tender.propertyIds)
                        foreach (int participantId in tender.participantIds)
                        {
                            Bid bid = new Bid();
                            bid.tenderId = tender.id;
                            bid.participantId = participantId;
                            bid.propertyId = propertyId;
                            db.Bid.Add(bid);
                        }
                    db.SaveChanges();
                    return RedirectToAction("Index");
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
            if (TryUpdateModel(TenderToUpdate, "", new string[] { "name", "description", "minPrice", "maxPrice", "participantIds", "propertyIds" }))
            {
                try
                {
                    db.SaveChanges();
                    // TODO: change tender bids
                    return RedirectToAction("Index");
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
