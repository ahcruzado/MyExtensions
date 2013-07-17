using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security;
using System.Net;

namespace MyExtensions.ActionFilters
{
    public class MyHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            //if (filterContext.Exception is HostProtectionException)
            //{
            //    filterContext.Exception = new HttpException((int)HttpStatusCode.Unauthorized, filterContext.Exception.Message);
            //}

            var url = filterContext.RequestContext.HttpContext.Request.Url.ToString();
            //Logger.Instance.LogErrorPage(url, filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}