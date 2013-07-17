using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;
using System.Text;

namespace MyExtensions.ActionFilters
{
    public class ActionOutputCacheAttribute : ActionFilterAttribute
    {
        public ActionOutputCacheAttribute(int cacheDuration)
        {
            this.cacheDuration = cacheDuration;
        }

        private int cacheDuration;
        private string cacheKey;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string url = filterContext.HttpContext.Request.Url.PathAndQuery;
            this.cacheKey = ComputeCacheKey(filterContext);

            if (filterContext.HttpContext.Cache[this.cacheKey] != null)
            {
                //Setting the result prevents the action itself to be executed
                filterContext.Result =
                (ActionResult)filterContext.HttpContext.Cache[this.cacheKey];
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Add the ActionResult to cache 
            filterContext.HttpContext.Cache.Add(this.cacheKey, filterContext.Result, null, DateTime.Now.AddSeconds(cacheDuration),
              System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);

            //Add a value in order to know the last time it was cached.
            filterContext.Controller.ViewData["CachedStamp"] = DateTime.Now;

            base.OnActionExecuted(filterContext);
        }

        private string ComputeCacheKey(ActionExecutingContext filterContext)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
            keyBuilder.Append(filterContext.ActionDescriptor.ActionName);

            foreach (var pair in filterContext.RouteData.Values)
            {
                if (pair.Value != null)
                    keyBuilder.AppendFormat("rd{0}_{1}_", pair.Key.GetHashCode(), pair.Value.GetHashCode());
            }
            return keyBuilder.ToString();
        }
    }
}