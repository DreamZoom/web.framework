using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace adminTest.Controllers
{
    public class HomeController : dz.web.Controller.AdminBase
    {
        public override void setBindModelType()
        {
            SetModelType("User");
        }

        public override dz.web.service.ServiceBase CreateService()
        {
            return new Models.UserService();
        }
       
        public ActionResult Index()
        {
            ViewBag.Message = "修改此模板以快速启动你的 ASP.NET MVC 应用程序。";

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "你的应用程序说明页。";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "你的联系方式页。";

            return View();
        }
    }
}
