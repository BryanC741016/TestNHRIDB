using MakeHTML.Models;
using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class RegionController : BasicController
    {
        private RegionDA _nodeDA;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _nodeDA = new RegionDA();
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: User
        public ActionResult Index(int pageNumber = 1, string sortColumn = "", string sortType = "")
        {
            RegionViewModel model = new RegionViewModel();
            if (TempData["Region"] != null)
                model = TempData["Region"] as RegionViewModel;
            model.setData(pageNumber, sortColumn, sortType);
            return Index(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index(RegionViewModel model)
        {

            var items = _nodeDA.GetQuery(model.rKey, model.name_en, model.name_tw);
                 
            model.items = model.GetSortColumnList(items, "regionKey");
            TempData["Region"] = model;
            return View(model);
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [HttpGet]
        public ActionResult Create()
        {
            RegionCreateModel model = new RegionCreateModel();
            
            model.type = "Create";
            return View(model);
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(RegionCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string msg = "";
            if (_nodeDA.HasAny("",model.rKey,model.name_en,model.name_tw,out msg)) {
                ModelState.AddModelError(string.Empty, msg);
                return View(model);
            }
            _nodeDA.Create(model.rKey, model.name_en, model.name_tw);
            return RedirectToAction("Index");
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [HttpGet]
        public ActionResult Edit(string rkey)
        {
            RegionCreateModel model = new RegionCreateModel();
            Region data = _nodeDA.GetRegion(rkey);
            model.name_en = data.name_en;
            model.name_tw = data.name_tw;

            model.rKey = data.regionKey;
            model.type = "Edit";
            return View("Create", model);

        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(RegionCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }
            string msg = "";
            if (_nodeDA.HasAny( model.rKey,"", model.name_en, model.name_tw, out msg))
            {
                ModelState.AddModelError(string.Empty, msg);
                return View("Create", model);
            }
            
            _nodeDA.Edit(model.rKey, model.name_en, model.name_tw);
            return RedirectToAction("Index");
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [AjaxValidateAntiForgeryToken]
        public JsonResult Delete(string rkey)
        {
            Rs rs = new Rs();
            rs.message = "刪除失敗";
            rs.isSuccess = _nodeDA.Delete(rkey);

            return Json(rs);
        }


    }
}