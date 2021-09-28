using NHRIDB.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NHRIDB_DAL.DAL;

namespace NHRIDB.Controllers
{
    public class TemplateExcelController : BasicController
    {
        // GET: TemplateExcel
        public ActionResult Index()
        {
            TemplateExcelViewModel _TemplateExcelViewModel = new TemplateExcelViewModel();

            return View(_TemplateExcelViewModel);
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase upload)
        {
            TemplateExcelViewModel _TemplateExcelViewModel = new TemplateExcelViewModel();

            string ex = upload == null ? null : Path.GetExtension(upload.FileName).Replace(".", "");

            if (string.IsNullOrEmpty(ex))
            {
                _TemplateExcelViewModel.msg = "請選擇檔案";

                return View(_TemplateExcelViewModel);
            }
            else
            {
                string[] allow = new string[] { "xlsx"};
                if (!allow.Contains(ex))
                {
                    _TemplateExcelViewModel.msg = "不支援此格式上傳";

                    return View(_TemplateExcelViewModel);
                }
            }

            try
            {
                string StrFile = Server.MapPath("~/Template/") + "Template.xlsx";
                upload.SaveAs(StrFile);

                TemplateExcelUpDataRecordDA _TemplateExcelUpDataRecordDA = new TemplateExcelUpDataRecordDA();
                _TemplateExcelUpDataRecordDA.Create();
            }
            catch(Exception e)
            {
                _TemplateExcelViewModel.msg = e.Message;

                return View(_TemplateExcelViewModel);
            }

            _TemplateExcelViewModel.msg = "儲存成功";

            return View(_TemplateExcelViewModel);
        }
    }
}