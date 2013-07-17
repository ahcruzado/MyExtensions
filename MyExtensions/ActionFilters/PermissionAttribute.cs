using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Authentication;
using System.Web.Security;

namespace MyExtensions.ActionFilters
{
    public class PermissionsAttribute : FilterAttribute, IAuthorizationFilter
    {
        private readonly Permissions required;

        public PermissionsAttribute(Permissions required)
        {
            this.required = required;
        }

        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //var userName = filterContext.HttpContext.User.Identity.Name;
            //var wokController = (filterContext.Controller as WokController);
            //if (wokController != null)
            //{
                
            //}

            //if (user == null)
            //{
            //    //send them off to the login page
            //    var url = new UrlHelper(filterContext.RequestContext);
            //    var loginUrl = url.Content("~/Home/Login");
            //    filterContext.HttpContext.Response.Redirect(loginUrl, true);
            //}
            //else
            //{
            //    if (!user.HasPermissions(required))
            //    {
            //        throw new AuthenticationException("You do not have the necessary permission to perform this action");
            //    }
            //}
        }
    }
}