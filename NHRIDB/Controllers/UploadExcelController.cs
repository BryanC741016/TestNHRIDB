﻿using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class UploadExcelController : BasicController
    {
        private HospitalDA _hospitalDA;
        private string _path = "";
        private string _dateFormat = "yyyyMMddHHmmss";
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _path = Server.MapPath("~/Upload/Excel");
            _hospitalDA = new HospitalDA(_db);
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index(string msg="")
        {
          
            if (!Directory.Exists(_path))
            {
                Directory.CreateDirectory(_path);
            }
            ImportViewModel model = new ImportViewModel();
            model.template = _template;
            model.msg = msg;
            return View(model);
        }

        [HttpPost]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Index(HttpPostedFileBase upload)
        {
            string ex = upload == null ? null : Path.GetExtension(upload.FileName).Replace(".", "");
            if (string.IsNullOrEmpty(ex))
            {
               
                return Index("請選擇檔案");
            }
            else
            {
                string[] allow = new string[] { "xlsx", "csv" };
                if (!allow.Contains(ex))
                {
                 
                    return Index("不支援此格式上傳");
                }
            }

            //==========上傳excel檔
            string now = DateTime.Now.ToString(_dateFormat);
            string hkey = _hospitalDA.GetHospital(_hos).hKey;
            string fileName = hkey + now + "." + ex;
            string path = Path.Combine(_path, fileName);
            upload.SaveAs(path);//為了方便csv讀取

            return Index("上傳完成，待管理者審核");
        }
  }
}