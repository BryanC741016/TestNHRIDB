using MakeHTML.Models;
using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class HospitalController : BasicController
    {
        private HospitalDA _hospitalDA;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _hospitalDA = new HospitalDA(_db);
        }

        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Hospital
        public ActionResult Index(int pageNumber=1,string sortColumn="",string sortType="")
        {
            HospitalViewModel model = new HospitalViewModel();
            if (TempData["HSForm"] != null)
                model = TempData["HSForm"] as HospitalViewModel;

            model.setData(pageNumber, sortColumn, sortType);

            return Index(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index(HospitalViewModel model)
        {

            

            var items=  _hospitalDA.GetQuery(searchText: model.searchText)
                .Select(e=> new HospitalItem { 
                   name_en=e.name_en,
                   name_tw=e.name_tw,
                   id=e.id
                }).ToList();

           model.items=  model.GetSortColumnList(items,"name_tw");
            TempData["HSForm"] = model;
            return View(model);
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Detail(Nullable<Guid> id) {
            HospitalDetail model = new HospitalDetail();
            if (id.HasValue) //編輯
            {
                var hos = _hospitalDA.GetHospital(id.Value);
                   model.id = hos.id;
                model.name_en = hos.name_en;
                model.name_tw = hos.name_tw;
                model.imgUrl = GetImgPath(hos.id, hos.fileExtension);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Detail(HospitalDetail model)
        {
            if (!ModelState.IsValid) {
                return View(model);
            }
            string ex = model.img == null ? null : Path.GetExtension(model.img.FileName).Replace(".","");
            if (!string.IsNullOrEmpty(ex)) {
                string[] allow = new string[] { "jpg", "gif", "png" };
                if (!allow.Contains(ex)) {
                    model.errorMsg = "上傳的附檔名不對";
                    return View(model);
                }
            }
            Guid id;
            if (model.id.HasValue)//修改
            {
                 id = model.id.Value;
                if (_hospitalDA.GetQuery(name_en: model.name_en,noID:id).Count() > 0)
                {
                    model.errorMsg = "英文名稱已被使用";
                    return View(model);
                }
                if (_hospitalDA.GetQuery(name_tw: model.name_tw, noID: id).Count() > 0)
                {
                    model.errorMsg = "中文名稱已被使用";
                    return View(model);
                }
                Hospital hospital = _hospitalDA.GetHospital(id);
                if (hospital == null) {
                    model.errorMsg = "查無此醫院資料";
                    return View(model);
                }

                if (model.img != null) {
                    delImg(id, hospital.fileExtension);
                }
           
                _hospitalDA.Edit(id, model.name_en, model.name_tw, ex);
              
            }
            else { //新增
                if (_hospitalDA.GetQuery(name_en: model.name_en).Count() > 0)
                {
                    model.errorMsg = "英文名稱已被使用";
                    return View(model);
                }
                if (_hospitalDA.GetQuery(name_tw: model.name_tw).Count() > 0)
                {
                    model.errorMsg = "中文名稱已被使用";
                    return View(model);
                }

                 id=  _hospitalDA.Create(model.name_en, model.name_tw, ex);
                
            }
            createImg(id, ex, model.img);

            return RedirectToAction("Index");
        }

        [AjaxValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public JsonResult Delete(Guid id) {
            Rs rs = new Rs();
            rs.isSuccess = false;
            Hospital hos= _hospitalDA.GetHospital(id);
            if (hos == null) {
                rs.message = "查無此資料";
                return Json(rs);
            }
            delImg(id,hos.fileExtension);
            _hospitalDA.Delete(id);
            rs.isSuccess = true;
            return Json(rs);
        }

        private void delImg(Guid id,string oldex) {
          
            string path = Path.Combine(_imgDirPath, id.ToString() + "." + oldex);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }

        private void createImg(Guid id, string ex,HttpPostedFileBase file)
        {
            if (file != null) {
                string path = Path.Combine(_imgDirPath, id.ToString() + "." + ex);
                file.SaveAs(path);
            }
          
        }
    }
}