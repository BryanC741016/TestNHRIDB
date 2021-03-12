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
        private TubeDataTotalDA _TubeDataTotalDA;
        string _dPath;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _diagnosisDA = new DiagnosisDA();
            _regionDA= new RegionDA();
            _dataTubeDA= new DataTubeDA();
            _TubeDataTotalDA = new TubeDataTotalDA();
            _dPath = Server.MapPath("~/Upload/" + _hos.ToString());
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Form
        public ActionResult Index()
        {
           
            if (!Directory.Exists(_dPath))
            {
                Directory.CreateDirectory(_dPath);
            }
            //add cache, add datas
            //delete cache files
            //var files = Directory.GetFiles("C:\\temp", "*.*", SearchOption.AllDirectories)
            // .Where(s => s.EndsWith("*.csv") || s.EndsWith("*.jpg"));
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

            //==========上傳excel檔
            string now = DateTime.Now.ToString("yyyyMMddhhmmss");
            string fileName = _hos.ToString()+"_"+ now + "." + ex;
            string path = Path.Combine(_dPath,"Cache", fileName);
            upload.SaveAs(path);
            model.fileName = fileName;
            //紀錄檔名，若沒有點選儲存則刪除該檔；若有即移到至正式目錄
            return View("ViewDatas",model); //顯示匯入的資
        }

        /// <summary>
        /// ViewDatas介面傳送資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [ValidateAntiForgeryToken]
        public ActionResult SaveData(ViewDatasViewModel model) {
            _dataTubeDA.Create(model.datas,_hos,_uid);
            string path = Path.Combine(_dPath,"Cache", model.fileName);
          
         
            string now = DateTime.Now.ToString("yyyyMMddhhmmss");
            System.IO.File.Move(path, Path.Combine(_dPath,"Datas", now));
          
            TempData["msg"] = "儲存完畢";
            return RedirectToAction("Different");
        }

        public ActionResult Different() {
            List<GetDifferentTotal_Result> diff= _TubeDataTotalDA.GetDifferent(_hos);
            DiffViewModel model = new DiffViewModel();
            model.columns = _TubeDataTotalDA.GetColummns();
            model.datas = diff;
            return View(model);
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