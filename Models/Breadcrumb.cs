﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExpenseTrackerApp.Models
{
    public class Breadcrumb
    {
        public class BreadcrumbItem
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
    }
}