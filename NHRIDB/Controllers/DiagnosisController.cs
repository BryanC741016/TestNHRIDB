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
    public class DiagnosisController : BasicController
    {
        private DiagnosisDA _diagnosisDA;
        private RegionDA _regionDA;
        private List<SelectListItem> _rlist;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _diagnosisDA = new DiagnosisDA();
            _regionDA = new RegionDA();
            List<Region>  rlist = _regionDA.GetQuery();
            _rlist = rlist.Select(e => new SelectListItem
            {
                Value=e.regionKey,
                Text=e.name_en
            }).ToList();
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: User
        public ActionResult Index(int pageNumber = 1, string sortColumn = "", string sortType = "")
        {
            DiagnosisModel model = new DiagnosisModel();
            if (TempData["Diagnosis"] != null)
                model = TempData["Diagnosis"] as DiagnosisModel;
            model.setData(pageNumber, sortColumn, sortType);
            return Index(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index(DiagnosisModel model)
        {

            var items = _diagnosisDA.GetQuery(model.dKey, model.name_en, model.name_tw);
                 
            model.items = model.GetSortColumnList(items, "diagnosisKey");
            TempData["Diagnosis"] = model;
            return View(model);
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [HttpGet]
        public ActionResult Create()
        {
            DiagnosisCreateModel model = new DiagnosisCreateModel();

            model.rList = _rlist;
            model.type = "Create";
            return View(model);
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DiagnosisCreateModel model)
        {
            model.rList = _rlist;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string msg = "";
            if (_diagnosisDA.HasAny("",model.dKey,model.name_en,model.name_tw,out msg)) {
                ModelState.AddModelError(string.Empty, msg);
                return View(model);
            }
            _diagnosisDA.Create(model.dKey, model.name_en, model.name_tw,model.checks);
          
            return RedirectToAction("Index");
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [HttpGet]
        public ActionResult Edit(string dkey)
        {
            DiagnosisCreateModel model = new DiagnosisCreateModel();
            Diagnosis data = _diagnosisDA.GetDiagnosis(dkey);
            model.name_en = data.dname_en;
            model.name_tw = data.dname_tw;

            model.dKey = data.diagnosisKey;
            model.type = "Edit";
            model.checks = data.Region.Select(e => e.regionKey).ToList();
            model.rList = GetChecks( model.checks);
            return View("Create", model);

        } 

      

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DiagnosisCreateModel model)
        {
            model.rList = GetChecks(model.checks);
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }
            string msg = "";
            if (_diagnosisDA.HasAny( model.dKey,"", model.name_en, model.name_tw, out msg))
            {
                ModelState.AddModelError(string.Empty, msg);
                return View("Create", model);
            }

            _diagnosisDA.Edit(model.dKey, model.name_en, model.name_tw,model.checks);
          
          
            return RedirectToAction("Index");
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [AjaxValidateAntiForgeryToken]
        public JsonResult Delete(string dkey)
        {
            Rs rs = new Rs();
            rs.message = "刪除失敗";
            rs.isSuccess = _diagnosisDA.Delete(dkey);

            return Json(rs);
        }

        private List<SelectListItem> GetChecks( List<string> checks)
        {
            if (checks == null) {
                return _rlist;
            }
            List<SelectListItem> selectListItems = _rlist.Select(e => new SelectListItem
            {
                Value = e.Value,
                Text = e.Text,
                Selected = checks.Contains(e.Value)
            }).ToList();
            return selectListItems;

        }


    }
}