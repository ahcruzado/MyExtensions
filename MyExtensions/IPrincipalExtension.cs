using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Principal;
using System.Web.Mvc;

namespace MyExtensions
{
    public static class IPrincipalExtension
    {
        public static bool IsInRole(this IPrincipal user, Role role)
        {
            bool result = false;
            var roles = EnumExtension.SplitFlags(role);
            foreach (var r in roles)
            {
                if (user.IsInRole(r.ToString()))
                    result = true;
            }
            return result;
        }
    }
}