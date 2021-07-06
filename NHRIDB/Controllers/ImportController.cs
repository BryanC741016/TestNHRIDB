using ClassLibrary;
using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
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
        public ActionResult Index(string msg="", Guid? hosId=null)
        {
            Session.Remove("BatchTable");
            Session.Remove("BatchTablePath");
            Session.Remove("BatchType");
            Session.Remove("hosId");

            if (!Directory.Exists(_cPath))
            {
                Directory.CreateDirectory(_cPath);
            }

            ImportViewModel model = new ImportViewModel();
            if (hosId.HasValue)
            {
                model.hospitalSelect = new SelectList(_hospitalDA.GetQuery().ToList(), "id", "name_tw",hosId.Value);
            }
            else {
                model.hospitalSelect = new SelectList(_hospitalDA.GetQuery().ToList(), "id", "name_tw");
            }
         
            model.template = _template;
            model.msg = msg;
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
        public ActionResult Index(HttpPostedFileBase upload,Guid hosId) 
        {
            Session.Remove("BatchTable");
            Session.Remove("BatchTablePath");
            Session.Remove("BatchType");
            Session.Remove("hosId");

            string ex = upload == null ? null : Path.GetExtension(upload.FileName).Replace(".", "");

            if (string.IsNullOrEmpty(ex))
            {               
                return Index("請選擇檔案",hosId);
            }
            else{
                string[] allow = new string[] { "xlsx", "csv" };
                if (!allow.Contains(ex))
                {                   
                    return Index("不支援此格式上傳",hosId);
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
            string msg = string.Empty;
            string StrAllMsg = string.Empty;
            bool isSuccess = true;
            EPPlusExcel epp = new EPPlusExcel();
            DataTable table=new DataTable();

            try
            {
                table = epp.GetDataTable(path, upload.InputStream);

                if(table.Rows.Count>100000)
                {
                    Session["BatchTable"] = table;
                    Session["BatchTablePath"] = path;
                    Session["BatchType"] = 0;
                    Session["hosId"] = hosId.ToString();
                    BatchTableViewModel _BatchTableViewModel = new BatchTableViewModel();                    

                    return BatchTable(_BatchTableViewModel);
                }
            }
            catch(Exception e)
            {
                StrAllMsg = "檔案轉換失敗,請確定檔案格式是否正確";
                isSuccess = false;                

                System.IO.File.Delete(path);
                return Index(StrAllMsg, hosId);
            }

            if (!_dataTubeDA.ImportCheck(table, out msg)) 
            {
                StrAllMsg = StrAllMsg+ msg;
                isSuccess = false;
            }
            else if (!_rLinkDDA.CheckDLinkR(table,out msg)) //部位與診斷代碼(dlinkR)代碼比對
            {
                StrAllMsg = StrAllMsg + msg;
                isSuccess = false;
            }

            if(!isSuccess)
            {
                System.IO.File.Delete(path);
                return Index(StrAllMsg, hosId);
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
        public ActionResult SaveData(ViewDatasViewModel model) 
        {
            DataSaveAns _DataSaveAns = _dataTubeDA.Create(model.datas,model.hId,_uid);
            string path = Path.Combine(_cPath, model.fileName);
            //移動至正式目錄
            string dpath = Server.MapPath("~/Upload/" + model.hId.ToString());
            System.IO.File.Move(path, Path.Combine(dpath, model.fileName.Replace(model.hId.ToString(),"")));
          
           
            return RedirectToAction("Different",new { hId=model.hId, _DataSaveAns = _DataSaveAns });
        }

        public ActionResult Different(Guid hId, DataSaveAns _DataSaveAns) 
        {
            List<GetDifferentTotal_Result> diff= _TubeDataTotalDA.GetDifferent(hId);
            DiffViewModel model = new DiffViewModel();

            model.columns = _TubeDataTotalDA.GetColummns();
            model.datas = diff;
            model.isSuccess = _DataSaveAns.isSuccess;
            model.StrMsg = _DataSaveAns.StrMsg;
            model.StackTrace = _DataSaveAns.StackTrace;

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

        /// <summary>
        /// 大量資料匯入-批次檢查處理
        /// </summary>
        /// <param name="BatchTableViewModel"></param>
        /// <returns></returns>
        public ActionResult BatchTable(BatchTableViewModel BatchTableViewModel)
        {
            if (string.IsNullOrEmpty(BatchTableViewModel.StrBatchMsg))
                BatchTableViewModel.StrBatchMsg = string.Empty;

            if (string.IsNullOrEmpty(BatchTableViewModel.StrCheckMsg))
                BatchTableViewModel.StrCheckMsg = string.Empty;

            switch (Session["BatchType"])
            {
                case 0:
                    {
                        BatchTableViewModel.StrBatchMsg = "因大量資料故採取批次分量檢查";
                        BatchTableViewModel.StrCheckMsg = string.Empty;

                        Session["BatchType"] = 1;
                    }
                    break;
                case 1:
                    {
                        BatchTableViewModel.StrBatchMsg = "檢查:欄位名稱是否相符";

                        if (!_dataTubeDA.HasColumns(Session["BatchTable"] as DataTable))
                        {
                            BatchTableViewModel.StrCheckMsg = BatchTableViewModel.StrCheckMsg + "欄位名稱不符合，請參照範本" + Environment.NewLine;
                            System.IO.File.Delete(Session["BatchTablePath"]as string);
                        }
                        else
                        {
                            Session["BatchType"] = 2;
                        }
                    }
                    break;
                case 2:
                    {
                        BatchTableViewModel.StrBatchMsg = "檢查:必填欄位沒有填";

                        if (!_dataTubeDA.CheckRequired(Session["BatchTable"] as DataTable,out BatchTableViewModel.StrCheckMsg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                        }
                        else
                        {
                            Session["BatchType"] = 3;
                        }
                    }
                    break;
                case 3:
                    {
                        BatchTableViewModel.StrBatchMsg = "檢查:性別是否統一、年齡是否統一";

                        if (!_dataTubeDA.RepleData(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                        }
                        else
                        {
                            Session["BatchType"] = 4;
                        }
                    }
                    break;
                case 4:
                    {
                        BatchTableViewModel.StrBatchMsg = "檢查:主key重複";

                        if (!_dataTubeDA.MatchKey(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                        }
                        else
                        {
                            Session["BatchType"] = 5;
                        }
                    }
                    break;
                case 5:
                    {
                        BatchTableViewModel.StrBatchMsg = "檢查:各欄位的資料型別 f:m , 數字 , 0:1)";

                        if (!_dataTubeDA.CheckType(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                        }
                        else
                        {
                            Session["BatchType"] = 6;
                        }
                    }
                    break;
                case 6:
                    {
                        BatchTableViewModel.StrBatchMsg = "檢查:部位與診斷代碼(dlinkR)代碼比對";

                        if (!_rLinkDDA.CheckDLinkR(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                        }
                        else
                        {
                            Session["BatchType"] = 7;
                        }
                    }
                    break;
                case 7:
                    {
                        ViewDatasViewModel model = new ViewDatasViewModel();
                        model.datas = _dataTubeDA.GetDatasByDataTable(Session["BatchTable"] as DataTable);
                        model.columns = _dataTubeDA.GetColummns();
                        model.hId = Guid.Parse(( Session["hosId"] as string));

                        //紀錄檔名，若沒有點選儲存則刪除該檔；若有即移到至正式目錄
                        return View("ViewDatas", model); //顯示匯入的資
                    }
            }

            return View("BatchTable",BatchTableViewModel);
        }
    }
}