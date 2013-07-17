//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace MyExtensions.ActionFilters
//{
//    public class UserNameFilter : ActionFilterAttribute
//    {
//        public override void OnActionExecuting(ActionExecutingContext filterContext)
//        {
//            const string Key = "userName";

//            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
//            {
//                var user = filterContext.HttpContext.User;
//                //var service = new ImpersonatingService(filterContext.HttpContext);
//                var userName = user.Identity.Name;

//                if (filterContext.ActionParameters.ContainsKey(Key))
//                {
//                    filterContext.ActionParameters[Key] = userName;
//                }
//                else if (filterContext.Controller is WokController)
//                {
//                    (filterContext.Controller as WokController).UserName = userName;
//                }
//            }
//            base.OnActionExecuting(filterContext);


//            //const string Key = "userName";

//            //if (filterContext.HttpContext.User.Identity.IsAuthenticated)
//            //{
//            //    var user = filterContext.HttpContext.User;
//            //    var service = new ImpersonatingService(filterContext.HttpContext);
//            //    var userName = service.IsImpersonating() ? service.ImpersonatingUserName() : user.Identity.Name;

//            //    if (filterContext.ActionParameters.ContainsKey(Key))
//            //    {
//            //        filterContext.ActionParameters[Key] = userName;
//            //    }
//            //    else if (filterContext.Controller is WokController)
//            //    {
//            //        (filterContext.Controller as WokController).UserName = userName;
//            //    }
//            //}
//            //base.OnActionExecuting(filterContext);
//        }
//    }
//}