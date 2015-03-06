using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web;
using System.IO;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.Mvc.Html;

namespace dz.web.Html
{
    public class TableGrid
    {
        /// <summary>
        /// 数据列表
        /// </summary>
        private TableListed TableListed { get; set; }

        /// <summary>
        /// 当前全局唯一id
        /// </summary>
        public string TableID = Guid.NewGuid().ToString();


        public HtmlHelper Html { get; set; }

        public TableGrid(TableListed tableListed,HtmlHelper html)
        {
            this.TableListed = tableListed;
            this.Html = html;
            this.ParamName = "page";

            this.PageIndex = tableListed.PageIndex;
            this.RecordCount = tableListed.RecordCount;
            this.PageSize = tableListed.PageSize;
        }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PageCount
        {
            get
            {
                return RecordCount / PageSize + (RecordCount % PageSize == 0 ? 0 : 1);
            }
        }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int RecordCount { get; set; }


        /// <summary>
        /// 最大 数字分页链接个数
        /// </summary>
        public int NumberLinkCount { get; set; }


        /// <summary>
        /// 当前页变量名
        /// </summary>
        public string ParamName { set; get; }


        private string getPageUrl(string url, string paramName, int page)
        {

            return url+"?"+ UpdateUrlParamters(Html.ViewContext.RequestContext.HttpContext.Request.QueryString, paramName, page.ToString());
            
        }

        /// <summary>
        /// 呈现分页条
        /// </summary>
        /// <returns></returns>
        public string ReaderPager(string url)
        {
            List<string> listNumber = new List<string>();

            //first
            listNumber.Add(string.Format("<a href=\"{0}\">首页</a>", getPageUrl(url, ParamName, 1)));
            //prev
            listNumber.Add(string.Format("<a href=\"{0}\">上一页</a>", getPageUrl(url, ParamName, PageIndex - 1)));

            #region Number Links
            int loopcount = PageCount > NumberLinkCount ? NumberLinkCount : PageCount;

            int startpage = 1;

            if (PageCount > NumberLinkCount)
            {
                if (PageIndex > 3 && PageIndex < PageCount - 3) startpage = 1;
                if (PageIndex >= PageCount - 3) startpage = PageCount - loopcount;
            }

            string currentClass = "";
            for (int i = startpage; i < startpage + loopcount; i++)
            {
                if (i == PageIndex) currentClass = "active";
                listNumber.Add(string.Format("<a href=\"{0}\" class=\"{2}\">{1}</a>", getPageUrl(url, ParamName, i), i, currentClass));
            }

            #endregion

            //next
            listNumber.Add(string.Format("<a href=\"{0}\">下一页</a>", getPageUrl(url, ParamName, PageIndex + 1)));
            //prev
            listNumber.Add(string.Format("<a href=\"{0}\">尾页</a>", getPageUrl(url, ParamName, PageCount)));
            return string.Join("", listNumber.ToArray());
        }


        public MvcHtmlString ReaderHead()
        {
            var propertys = this.TableListed.DataType.GetProperties();
            List<string> thList = new List<string>();
            thList.Add("<tr>");
            foreach (var p in propertys)
            {
                var displayname = ((DisplayAttribute[])p.GetCustomAttributes(typeof(DisplayAttribute), false)).FirstOrDefault();
                thList.Add(string.Format("<th>{0}</th>", displayname == null ? p.Name : displayname.Name));
            }
            thList.Add(string.Format("<th>操作</th>"));
            thList.Add("</tr>");
            return new MvcHtmlString(string.Join("", thList.ToArray()));
        }

        private string Formatter(string tempString, object model)
        {
            var propertys = model.GetType().GetProperties();
            Regex r = new Regex("{[a-z|0-9]*}");
            return r.Replace(tempString.ToLower(), (m) =>
            {
                foreach (var p in propertys)
                {
                    if ("{" + p.Name.ToLower() + "}" == m.Value)
                    {
                        var o = p.GetValue(model, null);

                        return o == null ? "0" : o.ToString();
                    }
                }
                return "0";
            });
        }

        private MvcHtmlString ReaderRow(object model)
        {
            var propertys = model.GetType().GetProperties();
            List<string> thList = new List<string>();
            thList.Add("<tr>");
            foreach (var p in propertys)
            {
                thList.Add(string.Format("<td>{0}</td>", p.GetValue(model, null)));
            }

            List<string> links = new List<string>();
            foreach (var kv in TableListed.ActionLinks)
            {
                links.Add(string.Format("<a href=\"{1}\">{0}</a>", kv.Key, Formatter(kv.Value, model)));
            }
            thList.Add(string.Format("<td>{0}</td>", string.Join("|", links.ToArray())));
            thList.Add("</tr>");
            return new MvcHtmlString(string.Join("", thList.ToArray()));
        }

        private MvcHtmlString ReaderBody()
        {

            List<string> thList = new List<string>();
            foreach (var d in TableListed)
            {
                thList.Add(ReaderRow(d).ToHtmlString());
            }
            return new MvcHtmlString(string.Join("", thList.ToArray()));
        }

        public MvcHtmlString ReaderTable()
        {

            UrlHelper UrlHelper = new UrlHelper(Html.ViewContext.RequestContext,Html.RouteCollection);
            string controllerName = Html.ViewContext.RouteData.Values["controller"].ToString();
            string actionName = Html.ViewContext.RouteData.Values["action"].ToString();

            List<string> thList = new List<string>();
            thList.Add("<div>");
            thList.Add(string.Format("  <table id=\"{0}\" >",TableID));
            thList.Add(ReaderHead().ToHtmlString());
            thList.Add(ReaderBody().ToHtmlString());
            thList.Add("</table>");
            thList.Add(string.Format("<div>{0}</div>", ReaderPager(UrlHelper.Action(actionName))));
            thList.Add("<div>");

            
            return new MvcHtmlString(string.Join("", thList.ToArray()));
        }

        private string UpdateUrlParamters(NameValueCollection QueryString,string name,string value)
        {
            NameValueCollection queryString = new NameValueCollection(QueryString);
            queryString.Set(name,value);

            List<string> list = new List<string>();
            foreach (var key in queryString.Keys)
            {
                list.Add(string.Format("{0}={1}",key.ToString(), queryString[key.ToString()]));
            }
            return string.Join("", list.ToArray());
        }

       
    }
    internal abstract class ActionCacheItem
    {
        public abstract string Execute(HtmlHelper html, ViewDataDictionary viewData);
    }
    internal class ActionCacheViewItem : ActionCacheItem
    {
        public string ViewName { get; set; }

        public override string Execute(HtmlHelper html, ViewDataDictionary viewData)
        {
            ViewEngineResult viewEngineResult = ViewEngines.Engines.FindPartialView(html.ViewContext, ViewName);
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            viewEngineResult.View.Render(new ViewContext(html.ViewContext, viewEngineResult.View, viewData, html.ViewContext.TempData, writer), writer);
            return writer.ToString();
        }
    }
}
