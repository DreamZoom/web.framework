using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;


namespace dz.web.Html
{
    public static class PagerExistions
    {
        public static MvcHtmlString TableGrid(this HtmlHelper helper,TableListed tableListed)
        {
            //return MvcHtmlString.Create(TemplateHelpers.TemplateHelper(helper, helper.ViewData.ModelMetadata, String.Empty, "TableGrid", DataBoundControlMode.ReadOnly, null /* additionalViewData */));

            return new TableGrid(tableListed,helper).ReaderTable();
        }

        
    }
}
