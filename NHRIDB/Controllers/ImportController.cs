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
        private RLinkDDA _rLinkDDA;
        private DataTubeDA _dataTubeDA;
        private TubeDataTotalDA _TubeDataTotalDA;
        private HospitalDA _hospitalDA;
        private string _cPath;
        private string _dateFormat="yyyyMMddHHmmss";
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _rLinkDDA = new RLinkDDA();
            _dataTubeDA= new DataTubeDA();
            _TubeDataTotalDA = new TubeDataTotalDA();
         //   _dPath = Server.MapPath("~/Upload/" + _hos.ToString());
            _cPath = Server.MapPath("~/Upload/Cache");
            _hospitalDA = new HospitalDA(_db);
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Form
        public ActionResult Index()
        {
           
          
            if (!Directory.Exists(_cPath))
            {
                Directory.CreateDirectory(_cPath);
            }

            ImportViewModel model = new ImportViewModel();
            model.hospitalSelect = new SelectList(_hospitalDA.GetQuery().ToList(), "id", "name_tw");
            return View(model);
        }

        private void DeleteFiles(string path) {
            DateTime now = DateTime.Now;
             string deltime = now.AddHours(-2).ToString(_dateFormat);
            foreach (var file in Directory.GetFiles(_cPath)) {
                string fileName = Path.GetFileNameWithoutExtension(file).Substring(36,14);
                 
                if (deltime.CompareTo(fileName) > 0) {
                     System.IO.File.Delete(file);
                }
            }

            //=========datas
            string delyear = now.AddYears(-2).ToString(_dateFormat);
            foreach (var file in Directory.GetFiles(path))
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (delyear.CompareTo(fileName) > 0)
                {
                    System.IO.File.Delete(file);
                }
            }
        }

        [HttpPost]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Index(HttpPostedFileBase upload,Guid hosId) {
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
            //============建立目錄
            string dpath = Server.MapPath("~/Upload/" + hosId.ToString());
            

            if (!Directory.Exists(dpath))
            {
                Directory.CreateDirectory(dpath);
            }
            //=================delete old cache files=============
            DeleteFiles(dpath);
            //==========上傳excel檔
            string now = DateTime.Now.ToString(_dateFormat);
            string fileName = hosId.ToString()+ now + "." + ex;
            string path = Path.Combine(_cPath, fileName);
            upload.SaveAs(path);//為了方便csv讀取
            ViewDatasViewModel model = new ViewDatasViewModel();
            model.fileName = fileName;
           
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
            DataTable table= epp.GetDataTable(path, upload.InputStream);

            string msg = "";
            if (!_dataTubeDA.ImportCheck(table, out msg)) {
                TempData["msg"] = msg;
                System.IO.File.Delete(path);
                return View("Index",new { hosId = hosId });
            }

            //部位與診斷代碼(dlinkR)代碼比對
            if (!_rLinkDDA.CheckDLinkR(table,out msg)) {
                TempData["msg"] = msg;
                System.IO.File.Delete(path);
                return View("Index", new { hosId = hosId });
            }

          
            model.datas = _dataTubeDA.GetDatasByDataTable(table);
            model.columns = _dataTubeDA.GetColummns();
            model.hId = hosId;

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
            _dataTubeDA.Create(model.datas,model.hId,_uid);
            string path = Path.Combine(_cPath, model.fileName);
            //移動至正式目錄
           string dpath = Server.MapPath("~/Upload/" + model.hId.ToString());
            System.IO.File.Move(path, Path.Combine(dpath, model.fileName.Replace(model.hId.ToString(),"")));
          
            TempData["msg"] = "儲存完畢";
            return RedirectToAction("Different",new { hId=model.hId});
        }

        public ActionResult Different(Guid hId) {
            List<GetDifferentTotal_Result> diff= _TubeDataTotalDA.GetDifferent(hId);
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
        
            DataTable tb2 = _rLinkDDA.GetDataTable();
            table.Add(tb2);
           

            string[] names = new string[] { "檢體資料", "部位與診斷編號" };

            MemoryStream stream = epp.ExportSample(names, table);

            string fileName = "sample"+DateTime.Now.ToString("yyyyMMddhhmmss")+".xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            stream.Position = 0;
            return File(stream, contentType, fileName);
             
        }

      
    }
}