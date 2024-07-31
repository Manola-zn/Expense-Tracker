using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExpenseTrackerApp.Models
{
    public class TransactionViewModel
    {
        public int transactionID { get; set; }
        public DateTime transactionDate { get; set; }
        public decimal transactionAmount { get; set; }
        public int categoryID { get; set; }
        public string categoryTitle { get; set; }
        public string categoryIcon { get; set; }
        public string categoryType { get; set; }
        
        public List<Category> Category { get; set; }

        [NotMapped]
        public string TitleWithIcon
        {
            get { return this.categoryIcon + " " + this.categoryTitle; }
        }
    }
}