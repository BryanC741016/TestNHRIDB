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
    public class RLinkDController : BasicController
    {
     
        private RLinkDDA _rLinkDDA;
       
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
             _rLinkDDA = new RLinkDDA();
           
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
 
        public ActionResult Index(int pageNumber = 1, string sortColumn = "", string sortType = "")
        {
            RLinkDViewModel model = new RLinkDViewModel();
            if (TempData["RLinkDForm"] != null)
                model = TempData["RLinkDForm"] as RLinkDViewModel;
            model.setData(pageNumber, sortColumn, sortType);
            return Index(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index(RLinkDViewModel model)
        {

            
            var items = _rLinkDDA.GetQuery(rKey:model.searchRkey,dkey:model.searchDkey,dName:model.searchDname,rName:model.searchRname)
                .Select(e => new RLinkDItem
                {
                  diagnosisKey=e.diagnosisKey
                  ,regionKey=e.regionKey
                  ,dName=e.dName
                  ,rName=e.rName
                }).ToList();

            model.items = model.GetSortColumnList(items, "regionKey");
            TempData["RLinkDForm"] = model;
            return View(model);
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Create() {
            RLinkDCreate model = new RLinkDCreate();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Create(RLinkDCreate model)
        {
             
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
           
            if (_rLinkDDA.HasAny(rName: model.rName, dName: model.dName)) {

                ModelState.AddModelError(string.Empty, "此部位與診斷名稱已新增過");
                return View(model);
            }

            
            if (_rLinkDDA.HasAny(dkey: model.diagnosisKey,rkey: model.regionKey)) {
                ModelState.AddModelError(string.Empty, "此部位與診斷編號重覆");
                return View(model);
            }

            _rLinkDDA.Create(model.regionKey,model.diagnosisKey,model.rName,model.dName);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Edit(string rKey,string dKey)
        {
            RLinkD rLinkD = _rLinkDDA.GetRD(rKey,dKey);
            RLinkDEdit model = new RLinkDEdit();
            model.diagnosisKey = dKey;
            model.regionKey = rKey;
            if (rLinkD == null) {
            
                ModelState.AddModelError(string.Empty, "查無此資料");
                return View(model);
            }

            model.rName = rLinkD.rName;
            model.dName = rLinkD.dName;
           

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Edit(RLinkDEdit model) {
           
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            RLinkD rLinkD = _rLinkDDA.GetRD(model.regionKey, model.diagnosisKey);

            if (rLinkD == null)
            {
               
                ModelState.AddModelError(string.Empty, "查無此資料");
                return View(model);
            }
            string msg = "";
           bool commit= _rLinkDDA.Edit(model.regionKey, model.diagnosisKey, model.rName, model.dName,out msg);
            if (!commit) {
                ModelState.AddModelError(string.Empty, msg);
                return View(model);
            }
            
            return RedirectToAction("Index");

        }

 

        [AjaxValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public JsonResult Delete(string rkey, string dkey)
        {
            Rs rs = new Rs();
            rs.isSuccess = false;
            RLinkD rLinkD = _rLinkDDA.GetRD(rkey, dkey);
            if (rLinkD == null)
            {
                rs.message = "查無此資料";
                return Json(rs);
            }

            _rLinkDDA.Delete(rkey, dkey);
            rs.isSuccess = true;
            return Json(rs);
        }

    }
}