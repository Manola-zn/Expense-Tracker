using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExpenseTrackerApp.Models;

namespace ExpenseTrackerApp.Controllers
{
    public class CategoryController : Controller
    {
        private ExpenseTrackerDBEntities db = new ExpenseTrackerDBEntities();

        // GET: Category
        public ActionResult Index()
        {
            return View(db.Categories.ToList());
        }
      
        // GET: Category/AddOrEdit
        public ActionResult AddOrEdit(int id=0)
        {
            if (id == 0)
            {
                return View(new Category());
            }
            else
            {
                return View(db.Categories.Find(id));
            }
        }

        // POST: Category/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit([Bind(Include = "categoryID,categoryTitle,categoryIcon,categoryType")] Category category)
        {

            try
            {
                    if (ModelState.IsValid)
                    {
                        if (category.categoryID == 0)
                        {
                            db.Categories.Add(category);
                        }
                        else
                        {
                            db.Entry(category).State = EntityState.Modified;
                        }
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }

            return View(category);
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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
