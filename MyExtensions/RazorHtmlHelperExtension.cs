using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Linq.Expressions;
using System.Web.WebPages;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web;
using System.ComponentModel;
namespace MyExtensions
{
    public static class RazorHtmlHelperExtension
    {
        public static UrlHelper GetUrlHelper(this System.Web.WebPages.Html.HtmlHelper html)
        {
            return ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).Url;
        }

        public static UrlHelper GetUrlHelper(this HtmlHelper html)
        {
            return new UrlHelper(html.ViewContext.RequestContext);
        }

        public static HtmlHelper GetPageHelper(this System.Web.WebPages.Html.HtmlHelper html)
        {
            return ((System.Web.Mvc.WebViewPage)WebPageContext.Current.Page).Html;
        }

        public static HtmlHelper<TModel> GetPageHelper<TModel>(this System.Web.WebPages.Html.HtmlHelper html)
        {
            return ((System.Web.Mvc.WebViewPage<TModel>)WebPageContext.Current.Page).Html;
        }
    }
}