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
    public class PlanController : BasicController
    {
        private PlanDA _PlanDA;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _PlanDA = new PlanDA();

        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index(int pageNumber = 1, string sortColumn = "", string sortType = "")
        {
            PlanViewModel model = new PlanViewModel();

            if (TempData["PlanForm"] != null)
                model = TempData["PlanForm"] as PlanViewModel;

            model.setData(pageNumber, sortColumn, sortType);

            return Index(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index(PlanViewModel model)
        {
            var items = _PlanDA.GetQuery(planKey: model.planKey, planName: model.planName)
                .Select(e => new PlanItem
                {
                    planKey = e.planKey
                  ,
                    planName = e.planName
                  ,
                    Remark = e.Remark
                }).ToList();

            model.items = model.GetSortColumnList(items, "planKey");
            TempData["PlanForm"] = model;

            return View(model);
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Create()
        {
            PlanCreate model = new PlanCreate();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Create(PlanCreate model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (_PlanDA.HasAny(planKey: model.planKey))
            {
                ModelState.AddModelError(string.Empty, "此代碼重覆");

                return View(model);
            }

            if (model.planKey.Length!=4)
            {
                ModelState.AddModelError(string.Empty, "代碼需4碼");

                return View(model);
            }

            _PlanDA.Create(model.planKey.ToUpper(), model.planName, model.Remark);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Edit(string planKey)
        {
            Plan _Plan = _PlanDA.GetPlan(planKey);
            PlanEdit model = new PlanEdit();
            model.planKey = planKey;

            if (_Plan == null)
            {
                ModelState.AddModelError(string.Empty, "查無此資料");

                return View(model);
            }

            model.planName = _Plan.planName;
            model.Remark = _Plan.Remark;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Edit(PlanEdit model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Plan _Plan = _PlanDA.GetPlan(model.planKey);

            if (_Plan == null)
            {
                ModelState.AddModelError(string.Empty, "查無此資料");

                return View(model);
            }

            _PlanDA.Edit(model.planKey, model.planName, model.Remark);

            return RedirectToAction("Index");
        }

        [AjaxValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public JsonResult Delete(string planKey)
        {
            Rs rs = new Rs();
            rs.isSuccess = false;

            Plan _Plan = _PlanDA.GetPlan(planKey);

            if (_Plan == null)
            {
                rs.message = "查無此資料";
                return Json(rs);
            }

            _PlanDA.Delete(planKey);

            rs.isSuccess = true;
            return Json(rs);
        }
    }
}