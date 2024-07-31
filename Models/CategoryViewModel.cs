using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExpenseTrackerApp.Models
{
    public class CategoryViewModel
    {
        public int categoryID { get; set; }
        public string categoryTitle { get; set; }
        public string categoryIcon { get; set; }
        public string categoryType { get; set; }
        public string TitleWithIcon { get; set; }
        public bool IsExpense { get; set; }
    }
}