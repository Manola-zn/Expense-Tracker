using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ExpenseTrackerApp.Helpers
{
    public static class HtmlHelpers
    {
        public static bool IsCurrentAction(this HtmlHelper helper, string actionName, string controllerName)
        {
            var routeValues = new RouteValueDictionary(helper.ViewContext.RouteData.Values);
            var currentAction = routeValues["action"] as string;
            var currentController = routeValues["controller"] as string;

            return string.Equals(currentAction, actionName, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(currentController, controllerName, StringComparison.OrdinalIgnoreCase);
        }
    }
}