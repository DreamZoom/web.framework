using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace dz.web.Controller
{
    /* *****
     * wxl
     * 后台管理通用控制器
     * 2015-03-04
     * *****/

    public class AdminBase : System.Web.Mvc.Controller
    {

        
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (!ModelBinders.Binders.ContainsKey(typeof(model.ModelBase)))
            {
                ModelBinders.Binders.Add(typeof(model.ModelBase), new ModelBaseBinder());
            }

            _service = CreateService();

            setBindModelType();
            initActionLinks();
            initBatchActionLinks();
        }

        public virtual string ControllerName
        {
            get
            {
                return ControllerContext.RouteData.Values["controller"].ToString();
            }
        }

        public virtual void initActionLinks()
        {
            
            _service.AddActionLinks(new KeyValuePair<string, string>("编辑", "/" + ControllerName + "/Edit/{ID}"));
            _service.AddActionLinks(new KeyValuePair<string, string>("删除", "/" + ControllerName + "/Delete/{ID}"));
            _service.AddActionLinks(new KeyValuePair<string, string>("详情", "/" + ControllerName + "/Details/{ID}"));
        }

        public virtual void initBatchActionLinks()
        {
            _service.AddBatchActionLinks(new KeyValuePair<string, string>("创建", "/" + ControllerName + "/Create"));
            _service.AddBatchActionLinks(new KeyValuePair<string, string>("删除", "/" + ControllerName + "/Delete/{ID}"));
        }

        public virtual void setBindModelType()
        {
            ViewData["BindModelType"] = this.ControllerContext.RouteData.Values["controller"].ToString();           
        }

        public virtual void SetModelType(string typeName)
        {
            ViewData["BindModelType"] = typeName ?? this.ControllerContext.RouteData.Values["controller"].ToString();
        }

        public virtual int PageSize {
            get
            {
                return 20;
            }
        }

        private service.ServiceBase _service = null;
        /// <summary>
        /// 获取服务处理程序
        /// </summary>
        public virtual service.ServiceBase Service
        {
            get {
                if (_service == null)
                {
                    _service = CreateService();
                }
                return _service;
            }
        }

        /// <summary>
        /// 创建一个处理服务
        /// </summary>
        /// <returns></returns>
        public virtual service.ServiceBase CreateService()
        {
            return new service.ServiceBase();
        }


        #region 创建对象
        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult Create(model.ModelBase model)
        {
            if (Service.Insert(model))
            {
                return Success();
            }

            return View(model);
        }
        #endregion

        #region 编辑对象

        public virtual ActionResult Edit(object ID)
        {
            var model = Service.GetModel("ID="+ID);
            if (model == null) return Error("未找到数据，请检查。");
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Edit(model.ModelBase model)
        {
            if (Service.Update(model))
            {
                return Success();
            }

            return View(model);
        }

        #endregion

        #region 删除对象

        public virtual ActionResult Delete(object ID)
        {
            var model = Service.GetModel("ID=" + ID);
            if (model == null) return Error("未找到数据，请检查。");
            return View(model);
        }

        [HttpPost]
        public virtual ActionResult Delete(object ID,model.ModelBase d)
        {
            var model = Service.GetModel("ID=" + ID);
            if (model == null) return Error("未找到数据，请检查。");

            if (Service.Delete(model))
            {
                return Success();
            }
            return View(model);
        }

        #endregion

        #region 查看对象
        public virtual ActionResult Details(object ID)
        {
            var model = Service.GetModel("ID=" + ID);
            if (model == null) return Error("未找到数据，请检查。");
            return View(model);
        }
        #endregion

        #region 分页列表

        public string Where { get; set; }

        public virtual ActionResult List(int page,string order)
        {
            Html.TableListed listed = Service.GetModelList(Where, order, page, PageSize);
            return View(listed);
        }
        #endregion


        #region 成功失败提示 

        public virtual ActionResult Success(string message="操作成功")
        {
            return View();
        }


        public virtual ActionResult Error(string message="操作失败")
        {
            return View();
        }
        #endregion
    }
}
