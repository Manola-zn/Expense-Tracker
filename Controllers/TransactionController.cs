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
    public class TransactionController : Controller
    {
        private ExpenseTrackerDBEntities db = new ExpenseTrackerDBEntities();

        // GET: Transaction
        public ActionResult Index()
        {
            // Call the method to populate categories
            PopulateCategories();

            var transactions = db.Transactions
                                .OrderBy(t => t.transactionDate.Day)
                                .ThenBy(t => t.transactionDate.Month)
                                .ToList();

            return View(transactions);
        }

        // GET: Transaction/AddOrEdit
        public ActionResult AddOrEdit(int id = 0)
        {
            
            PopulateCategories();

            if (id == 0)
            { 
                return View(new Transaction());
            }
            else
            {
                return View(db.Transactions.Find(id)); 
            }
        }

        // POST: Transaction/AddOrEdit
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEdit([Bind(Include = "transactionID,transactionNote,transactionAmount,transactionDate,categoryID")] Transaction transaction)
        {
            try 
            {
                if (ModelState.IsValid)
                {
                    if (transaction.transactionID == 0)
                    {
                        db.Transactions.Add(transaction);
                    }
                    else
                    {
                        db.Entry(transaction).State = EntityState.Modified;
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

            PopulateCategories();
            return View(transaction);
        }

        // GET: Transaction/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [NonAction]
        public void PopulateCategories()
        {
            //Retrieve categories from your data source (e.g. database)
            var categories = db.Categories.ToList();

            //Convert Category objects to SelectListItem objects with option groups
            var categoriesAsSelectList = new List<SelectListItem>();

            //Add a default SelectListItem for "Choose a Category"
            categoriesAsSelectList.Add(new SelectListItem { Value = "", Text = "Choose a Category" });

            //Group categories by type (income or expense)
            var groupedCategories = categories.GroupBy(c => c.categoryType);

            foreach (var group in groupedCategories)
            {
                //Add option group
                var groupSelectList = new SelectListGroup { Name = group.Key };

                //Add categories within the group
                foreach (var category in group)
                {
                    //Combine group name and category name for display
                    string categoryNameWithGroup = category.TitleWithIcon;

                    categoriesAsSelectList.Add(new SelectListItem
                    {
                        Value = category.categoryID.ToString(),
                        Text = categoryNameWithGroup,
                        Group = groupSelectList //Assign the category to its respective group
                    });
                }
            }

            //Assign the list of SelectListItem to ViewBag
            ViewBag.Categories = categoriesAsSelectList;
        }

    }
}
