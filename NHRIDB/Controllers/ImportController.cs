using ClassLibrary;
using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using NHRIDB_DAL.ViewModel;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class ImportController : BasicController
    {
        private DiagnosisDA _diagnosisDA;
        private RegionDA _regionDA;
        private DataTubeDA _dataTubeDA;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _diagnosisDA = new DiagnosisDA();
            _regionDA= new RegionDA();
            _dataTubeDA= new DataTubeDA();
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Form
        public ActionResult Index()
        {
           
            return View( );
        }

        [HttpPost]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Index(HttpPostedFileBase upload) {
            string ex = upload == null ? null : Path.GetExtension(upload.FileName).Replace(".", "");
            if (string.IsNullOrEmpty(ex))
            {
                TempData["msg"] = "請選擇檔案";
                return View();
            }
            else{
                string[] allow = new string[] { "xlsx", "csv" };
                if (!allow.Contains(ex))
                {
                    TempData["msg"] = "不支援此格式上傳";
                    return View();
                }
            }

            /***
             * 1.欄位名稱是否相符
             * 必填欄位沒有填
             * 2.性別是否統一
             * 3.年齡是否統一
             * 主key重複
             * 4.部位與診斷代碼(dlinkR)代碼比對
             * 資料型別(f:m , 數字 , 0:1)
             * **/
            EPPlusExcel epp = new EPPlusExcel();
            DataTable table= epp.GetDataTable(upload.InputStream);

            string msg = "";
            if (!_dataTubeDA.ImportCheck(table, out msg)) {
                TempData["msg"] = msg;
                return View();
            }

            //部位與診斷代碼(dlinkR)代碼比對
            if (!_diagnosisDA.CheckDLinkR(table,out msg)) {
                TempData["msg"] = msg;
                return View();
            }

            ViewDatasViewModel model = new ViewDatasViewModel();
            model.datas = _dataTubeDA.GetDatasByDataTable(table);
            model.columns = _dataTubeDA.GetColummns();
            return View("ViewDatas",model);
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [ValidateAntiForgeryToken]
        public ActionResult SaveData(ViewDatasViewModel model) {
            _dataTubeDA.Create(model.datas,_hos,_uid);
            return RedirectToAction("Index");
        }


        /// <summary>
        /// 匯出範本
        /// </summary>
        /// <returns></returns>
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult ExportData() {
            EPPlusExcel epp = new EPPlusExcel();
            List<DataTable> table = new List<DataTable>();
            DataTable tb = _dataTubeDA.GetEmptyDataTable();
            table.Add(tb);
        
            DataTable tb2 = _regionDA.GetDataTable();
            table.Add(tb2);
            DataTable tb1 = _diagnosisDA.GetDataTable();
            table.Add(tb1);

            string[] names = new string[] { "檢體資料", "部位編號", "診斷編號" };

            MemoryStream stream = epp.ExportSample(names, table);

            string fileName = "sample"+DateTime.Now.ToString("yyyyMMddhhmmss")+".xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            stream.Position = 0;
            return File(stream, contentType, fileName);
             
        }

      
    }
}