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
using System.Text;
namespace MyExtensions
{
    public static class HtmlHelperExtension
    {
        #region Html

        public static void EnablePartialViewValidation(this HtmlHelper helper)
        {
            if (helper.ViewContext.FormContext == null)
            {
                helper.ViewContext.FormContext = new FormContext();
            }
        }

        /// <summary>
        /// http://stackoverflow.com/questions/3460762/how-to-concatenate-several-mvchtmlstring-instances/3464634#3464634
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        private static MvcHtmlString Concat(params MvcHtmlString[] items)
        {
            var sb = new StringBuilder();
            foreach (var item in items.Where(i => i != null))
                sb.Append(item.ToHtmlString());
            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString ControlGroupFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText = null, HtmlString coreControl = null, string textToAppend = "")
        {
            var controlGroup = new TagBuilder("div");
            controlGroup.AddCssClass("control-group");

            var controls = new TagBuilder("div");
            controls.AddCssClass("controls");

            if (coreControl == null)
                coreControl = html.EditorFor(expression);

            if (!string.IsNullOrWhiteSpace(textToAppend))
                textToAppend = " " + textToAppend;

            controls.InnerHtml =
                coreControl.ToString() +
                textToAppend +
                html.ValidationMessageFor(expression, null, new { @class = "help-inline" });

            if (labelText == null)
                labelText = html.LabelFor(expression, new { @class = "control-label" }).ToString();
            else
                labelText = html.LabelFor(expression, labelText, new { @class = "control-label" }).ToString();

            controlGroup.InnerHtml =
                labelText +
                controls.ToString();
            return new MvcHtmlString(controlGroup.ToString());
        }

        public static MvcHtmlString GoogleAnalytics(this HtmlHelper helper, string account)
        {
            var builder = new TagBuilder("script");
            builder.MergeAttribute("type", "text/javascript");
            builder.InnerHtml = @"
                var _gaq = _gaq || [];
                _gaq.push(['_setAccount', '" + account + @"']);
                _gaq.push(['_trackPageview']);

                (function () {
                    var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true;
                    ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js';
                    var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s);
                })();";
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string src, string altText)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);

            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", url.Content(src));
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("title", altText);
            return new MvcHtmlString(builder.ToString());
        }

        public static MvcHtmlString ActionImage(this HtmlHelper html, string action, string imagePath, string alt, object routeValues = null, string controllerName = null)
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            // build the <img> tag
            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imagePath));
            imgBuilder.MergeAttribute("alt", alt);
            imgBuilder.MergeAttribute("title", alt);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);

            // build the <a> tag
            var anchorBuilder = new TagBuilder("a");

            var urlStr = url.Action(action, controllerName, routeValues);

            anchorBuilder.MergeAttribute("href", urlStr);
            anchorBuilder.InnerHtml = imgHtml; // include the <img> tag inside
            string anchorHtml = anchorBuilder.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(anchorHtml);
        }

        public static MvcHtmlString ActionLink(this HtmlHelper html, MvcHtmlString innerHtml, string href, string target = "", string name = "")
        {
            var a = new TagBuilder("a");
            a.MergeAttribute("href", href);
            a.MergeAttribute("target", target);
            a.MergeAttribute("name", name);
            a.InnerHtml = innerHtml.ToHtmlString();
            return MvcHtmlString.Create(a.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString AlternateRows<T>(this HtmlHelper html, IEnumerable<T> items, Func<T, IHtmlString> template)
        {
            int i = 0;
            var result = "";
            foreach (var item in items)
            {
                i++;
                var tr = new TagBuilder("tr");
                if (i % 2 == 0)
                    tr.MergeAttribute("class", "alternateRow");
                tr.InnerHtml = template(item).ToHtmlString();
                result += tr.ToString();
            }
            return MvcHtmlString.Create(result);
        }

        public static MvcHtmlString Grid<T>(this HtmlHelper html, IEnumerable<T> items, Func<T, MvcHtmlString> template, int numCol)
        {
            var itemsList = items.ToList();
            var url = html.GetUrlHelper();

            int i, j;
            int idx = 0;

            var table = new TagBuilder("table");
            do
            {
                var tr = new TagBuilder("tr");

                for (j = 0; j < numCol; j++)
                {
                    var td = new TagBuilder("td");
                    if (idx < itemsList.Count())
                    {
                        var item = itemsList[idx++];
                        td.InnerHtml = template(item).ToHtmlString();
                    }
                    tr.InnerHtml += td.ToString();
                }
                table.InnerHtml += tr.ToString();
            } while (idx < itemsList.Count());
            return MvcHtmlString.Create(table.ToString(TagRenderMode.Normal));
        }

        public static SelectList ToSelectList<TEnum>()
        {
            IEnumerable values = EnumExtension.ToList<TEnum>();
            return new SelectList(values, "Value", "Text");
        }

        /// <summary>
        /// Non credo supporti gli enum a flag
        /// </summary>
        public static HtmlString EnumDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> modelExpression, string firstElement)
        {
            var typeOfProperty = modelExpression.ReturnType;
            if (!typeOfProperty.IsEnum)
                throw new ArgumentException(string.Format("Type {0} is not an enum", typeOfProperty));

            var enumValues = new SelectList(Enum.GetValues(typeOfProperty));
            return htmlHelper.DropDownListFor(modelExpression, enumValues, firstElement);
        }

        public static SelectList ToSelectList<TEnum>(TEnum enumObj, bool useStringValue = false)
        {
            IEnumerable values = EnumExtension.ToList<TEnum>(useStringValue);

            Type enumType = EnumExtension.UnboxNullable<TEnum>();

            var value = "";
            if (enumObj != null)
            {
                var stringValue = enumObj.ToString();
                if (useStringValue)
                    value = stringValue;
                else
                    value = ((int)Enum.Parse(enumType, stringValue)).ToString();
            }

            return new SelectList(values, "Value", "Text", value);
        }
        #endregion
        
        public static MvcHtmlString MultiLineText(this HtmlHelper htmlHelper, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return MvcHtmlString.Create(string.Empty);
            }
            var encodedText = htmlHelper.Encode(text);
            encodedText = Regex.Replace(encodedText, Environment.NewLine, "<br />");
            encodedText = Regex.Replace(encodedText, "\n", "<br />");
            return MvcHtmlString.Create(encodedText);
        }

        public static MvcHtmlString DisplayForEnum<TEnum>(this HtmlHelper htmlHelper, TEnum value)
        {
            var text = EnumExtension.GetDisplay<TEnum>(value);
            return new MvcHtmlString(text);
        }


        #region
        /// <summary>
        /// http://blogs.msdn.com/b/stuartleeks/archive/2010/05/21/asp-net-mvc-creating-a-dropdownlist-helper-for-enums.aspx
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            IEnumerable<SelectListItem> items =
                from value in values
                select new SelectListItem
                {
                    Text = EnumExtension.GetDisplay(value),
                    Value = value.ToString(),
                    Selected = value.Equals(metadata.Model)
                };

            if (metadata.IsNullableValueType)
            {
                items = new[] { new SelectListItem { Text = "", Value = "" } }.Concat(items);
            }

            return htmlHelper.DropDownListFor(
                expression,
                items
                );
        }

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }
        #endregion

        #region http://haacked.com/archive/2008/10/23/model-binding-to-a-list.aspx
        static Regex _stripIndexerRegex = new Regex(@"\[(?<index>\d+)\]", RegexOptions.Compiled);

        public static string GetIndexerFieldName(this TemplateInfo templateInfo)
        {
            string fieldName = templateInfo.GetFullHtmlFieldName("Index");
            fieldName = _stripIndexerRegex.Replace(fieldName, string.Empty);
            if (fieldName.StartsWith("."))
            {
                fieldName = fieldName.Substring(1);
            }
            return fieldName;
        }

        public static int GetIndex(this TemplateInfo templateInfo)
        {
            string fieldName = templateInfo.GetFullHtmlFieldName("Index");
            var match = _stripIndexerRegex.Match(fieldName);
            if (match.Success)
            {
                return int.Parse(match.Groups["index"].Value);
            }
            return 0;
        }

        /// <summary>
        /// http://haacked.com/archive/2008/10.aspx
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString HiddenIndexerInputForModel<TModel>(this HtmlHelper<TModel> html)
        {
            string name = html.ViewData.TemplateInfo.GetIndexerFieldName();
            object value = html.ViewData.TemplateInfo.GetIndex();
            string markup = String.Format(@"<input type=""hidden"" name=""{0}"" value=""{1}"" />", name, value);
            return MvcHtmlString.Create(markup);
        }
        #endregion
        
        //public static MvcHtmlString InlineEditorFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, Expression<Func<TModel, TValue>> idExpression, string url, string defaultDisplay= null)
        //{
        //    var div = new TagBuilder("div");
        //    div.MergeAttribute("data-inlineedit", url);
        //    if(defaultDisplay!=null)
        //    div.MergeAttribute("data-inline-edit-default", defaultDisplay);

        //    var form = new TagBuilder("form");

        //    var divLable= new TagBuilder("div");
        //    divLable.MergeAttribute("data-inline-edit-label","");
        //    divLable.InnerHtml = "<div class='pull-right'>"+ html.Image(IconImage.Edit, "Modifica") +"</div>";

        //    var divDisplay = new TagBuilder("div");

        //    divDisplay.InnerHtml = html.MultiLineText(expression.Compile()().ToString();
        //                </div>    
        //                @Html.EditorFor(m => item.Note, "")
        //                <input type="hidden" name = "ID" value = "@item.ID" />
        //            }

        //     <div data-inline-edit='@Url.Action("EditPUserAsync")' data-inline-edit-default="Anonimo" style="width: 100%; height:100%">
        //                <div>
        //                    <form class="form-horizontal" action="">
        //                        <div data-inline-edit-label>
        //                            <div class="pull-right">
        //                        </div>
        //                        <div data-inline-edit-label-text>
        //                            @if (string.IsNullOrWhiteSpace(Model.PUser.Name))
        //                            {
        //                                <text>Anonimo</text>}
        //                            else
        //                            { @Model.PUser.Name}
        //                        </div>
        //                    </div>
        //                    <div style="display: none" data-inline-edit-editor>
        //                        @Html.EditorFor(m => Model.PUser.Name)
        //                    </div>
        //                    <span style="display: none" data-inline-edit-loading>loading..</span>
        //                    <input type="hidden" name = "ID" value = "@Model.ID" />
        //                </form>
        //            </div>
        //}


        #region Ajax
        public static MvcHtmlString ActionImage(this AjaxHelper helper, string src, string altText, string actionName, object routeValues, AjaxOptions ajaxOptions, string controllerName = null)
        {
            var url = new UrlHelper(helper.ViewContext.RequestContext);

            var builder = new TagBuilder("img");
            builder.MergeAttribute("src", url.Content(src));
            builder.MergeAttribute("alt", altText);
            builder.MergeAttribute("title", altText);

            var link = helper.ActionLink("[replaceme]", actionName, controllerName, routeValues, ajaxOptions);
            return new MvcHtmlString(link.ToString().Replace("[replaceme]", builder.ToString(TagRenderMode.SelfClosing)));
        }
        #endregion
    }
}