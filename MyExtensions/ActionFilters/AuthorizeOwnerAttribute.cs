using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyExtensions.ActionFilters
{
    public class AuthorizeOwnerAttribute : FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            string ownerName = filterContext.HttpContext.User.Identity.Name;
            filterContext.Result = new ViewResult() { ViewName = "UnautorizedAction" };
        }
    }
}