using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyExtensions.ActionFilters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorize : AuthorizeAttribute
    {
        public RoleAuthorize() : base() { }
        public RoleAuthorize(Role roles)
            : base()
        {
            Roles = roles;
        }

        public new Role Roles { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Roles != 0)
                base.Roles = Roles.ToString();

            base.OnAuthorization(filterContext);
        }
    }
}