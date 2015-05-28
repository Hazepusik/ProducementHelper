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
    public class PropertyController : Controller
    {
        private ProcurementEntities db = new ProcurementEntities();

        // GET: Property
        public ViewResult Index(string sortOrder, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            List<Property> properties = ContosoUniversity.Property.QueryAll().Where(p => p.isDefault??false).ToList();
            switch (sortOrder)
            {
                case "name_desc":
                    properties = properties.OrderByDescending(p => p.name).ToList();
                    break;
                default:  // Name ascending 
                    properties = properties.OrderBy(p => p.name).ToList();
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(properties.ToPagedList(pageNumber, pageSize));
        }


        // GET: Property/Create
        public ActionResult Create()
        {
            Property prop = new Property();
            prop.toMax = true;
            prop.importance = 100;
            return View(prop);
        }

        // POST: Property/Save
        [HttpPost]
        public ActionResult Save(Property newP)
        {
            try
            {
                Property oldP = db.Property.First(p => p.id == newP.id);
                oldP.functionId = newP.functionId;
                oldP.importance = newP.importance;
                oldP.maxValue = newP.maxValue;
                oldP.minValue = newP.minValue;
                oldP.step = newP.step;
                oldP.toMax = newP.toMax;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                ModelState.AddModelError("", "Произошла ошибка при сохранении записи. Убедитесь, что все оПри повторении ошибки обратитесь к администратору");
            }
            return View();
        }


        // POST: Property/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name, importance, isSubpropertyOf, minValue, maxValue, step, functionId, toMax, isDefault")]Property property)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    property.isDefault = true;
                    db.Property.Add(property);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (RetryLimitExceededException)
            {
                
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Произошла ошибка при сохранении записи. При повторении ошибки обратитесь к администратору");
            }
            catch 
            {
                ModelState.AddModelError("", "Произошла ошибка при сохранении записи. Убедитесь, что все оПри повторении ошибки обратитесь к администратору");
            }
            return View(property);
        }

        // GET: Property/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property Property = db.Property.Find(id);
            if (Property == null)
            {
                return HttpNotFound();
            }
            return View(Property);
        }

        // POST: Property/Edit/5
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property PropertyToUpdate = db.Property.Find(id);
            UpdateModel(PropertyToUpdate, "", new string[] { "name", "importance", "isSubpropertyOf", "minValue", "maxValue", "step", "functionId", "toMax" });
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e) //(RetryLimitExceededException)// dex )
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Произошла ошибка при изменении записи. При повторении ошибки обратитесь к администратору");
                }
            }
            return View(PropertyToUpdate);
        }

        // GET: Property/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property Property = db.Property.Find(id);
            if (Property == null)
            {
                return HttpNotFound();
            }
            return View(Property);
        }

        // GET: Property/Delete/5
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
            Property Property = db.Property.Find(id);
            if (Property == null)
            {
                return HttpNotFound();
            }
            return View(Property);
        }

        // POST: Property/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Property Property = db.Property.Find(id);
                List<Property> children = db.Property.Where(p => p.isSubpropertyOf == id).ToList();
                foreach (Property p in children)
                    p.isSubpropertyOf = null;
                db.SaveChanges();
                db.Property.Remove(Property);
                db.SaveChanges();
            }
            catch// (RetryLimitExceededException )// dex )
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
