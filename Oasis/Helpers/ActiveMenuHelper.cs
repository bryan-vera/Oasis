namespace Helpers.Helpers
{ 

    using System;
    using System.Web.Mvc;
    using System.Collections.Generic;
    using System.Linq;

    public static class ActiveMenuHelper
    {
        public static string IsSelected(this HtmlHelper htmlHelper, string controllers, string actions, string cssClass = "active nav-link")
        {
        string currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
        string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

        IEnumerable<string> acceptedActions = (actions ?? currentAction).Split(',');
        IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(',');

        return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController) ?
            cssClass : "nav-link";
        }

        public static string DropdownIsSelected(this HtmlHelper htmlHelper, string controllers, string cssClass = "nav-item menu-is-opening menu-open")
        {
            string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(',');

            return acceptedControllers.Contains(currentController) ?
                cssClass : "nav-item";
        }

        public static string LabelDropdownIsSelected(this HtmlHelper htmlHelper, string controllers, string cssClass = "active nav-link")
        {
            string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;
            IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(',');
            return acceptedControllers.Contains(currentController) ?
                cssClass : "nav-link";
        }


    }

}