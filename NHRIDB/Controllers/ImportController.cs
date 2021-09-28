using ClassLibrary;
using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using NHRIDB_DAL.ViewModel;
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
            setRemoveSession();

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

            TemplateExcelUpDataRecordDA _TemplateExcelUpDataRecordDA = new TemplateExcelUpDataRecordDA();
            List<TemplateExcelUpDataRecord> old = _TemplateExcelUpDataRecordDA.getAllList();
            string templateTime = string.Empty;

            if(old!=null)
            {
                if(old.Count>0)
                {
                    templateTime = templateTime + old[0].UpDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                }
            }

            model.templateTime = templateTime;

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
            setRemoveSession();

            string ex = upload == null ? null : Path.GetExtension(upload.FileName).Replace(".", "");

            if (string.IsNullOrEmpty(ex))
            {
                return Index("請選擇檔案", hosId);
            }
            else
            {
                string[] allow = new string[] { "xlsx", "csv" };
                if (!allow.Contains(ex))
                {
                    return Index("不支援此格式上傳", hosId);
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
            string fileName = hosId.ToString() + now + "." + ex;
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
            DataTable table = new DataTable();

            try
            {
                table = epp.GetDataTable(path, upload.InputStream);

                List<TubeData> _LitTubeData = _dataTubeDA.getTubeData(hosId);
                Session["OldTubeDataCount"] = _LitTubeData != null ? _LitTubeData.Count : 0;

                if (table.Rows.Count >100000)
                {
                    Session["BatchTable"] = table;
                    Session["BatchTablePath"] = path;
                    Session["BatchType"] = 0;
                    Session["hosId"] = hosId.ToString();
                    Session["intBatchStartIndex"] =0;
                    Session["isBatchError"] = false;
                    Session["fileName"] = model.fileName;
                    BatchTableViewModel _BatchTableViewModel = new BatchTableViewModel();

                    return BatchTable(_BatchTableViewModel);
                }
            }
            catch (Exception e)
            {
                StrAllMsg = "檔案轉換失敗,請確定檔案格式是否正確";
                isSuccess = false;

                System.IO.File.Delete(path);
                return Index(StrAllMsg, hosId);
            }

            if (!_dataTubeDA.ImportCheck(table, out msg))
            {
                StrAllMsg = StrAllMsg + msg;
                isSuccess = false;
            }
            else if (!_rLinkDDA.CheckDLinkR(table, out msg)) //部位與診斷代碼(dlinkR)代碼比對
            {
                StrAllMsg = StrAllMsg + msg;
                isSuccess = false;
            }

            if (!isSuccess)
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

        /// <summary>
        /// View Batch Datas介面傳送資料
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        [ValidateAntiForgeryToken]
        public ActionResult SaveBatchData(ViewBatchDatasViewModel model)
        {
            DataSaveAns _DataSaveAns = _dataTubeDA.CreateBatch(model.datas, model.hId, _uid,model.isFirst);

            if(model.isFirst)
            {
                string path = Path.Combine(_cPath, model.fileName);
                //移動至正式目錄
                string dpath = Server.MapPath("~/Upload/" + model.hId.ToString());
                System.IO.File.Move(path, Path.Combine(dpath, model.fileName.Replace(model.hId.ToString(), "")));
            }            

            if(!model.isEnd)
            {
                BatchTableViewModel _BatchTableViewModel = new BatchTableViewModel();
                _BatchTableViewModel.isExeSetTimeOut = true;

                if (!_DataSaveAns.isSuccess)
                {
                    _BatchTableViewModel.isExeSetTimeOut = false;
                }

                return RedirectToAction("BatchTable", new { StrAnsError = _DataSaveAns.StrMsg, isExeSetTimeOut = _BatchTableViewModel.isExeSetTimeOut });
            }                
            else
            {
                if(!_DataSaveAns.isSuccess)
                {
                    BatchTableViewModel _BatchTableViewModel = new BatchTableViewModel();
                    _BatchTableViewModel.isExeSetTimeOut = false;

                    return RedirectToAction("BatchTable", new { StrAnsError = _DataSaveAns.StrMsg, isExeSetTimeOut = _BatchTableViewModel.isExeSetTimeOut });
                }
                else
                {
                    return RedirectToAction("Different", new { hId = model.hId, _DataSaveAns = _DataSaveAns });
                }
            } 
        }

        public ActionResult Different(Guid hId, DataSaveAns _DataSaveAns) 
        {
            ImportAnsViewModel _ImportAnsViewModel = new ImportAnsViewModel();
            _ImportAnsViewModel.OldCount = Convert.ToInt32(Session["OldTubeDataCount"]);
            List<TubeData> _LitTubeData=_dataTubeDA.getTubeData(hId);
            _ImportAnsViewModel.NewCount= _LitTubeData != null? _LitTubeData.Count:0;

            //List<GetDifferentTotal_Result> diff = _TubeDataTotalDA.GetDifferent(hId);
            //DiffViewModel model = new DiffViewModel();

            //model.columns = _TubeDataTotalDA.GetColummns();
            //model.datas = diff;
            //model.isSuccess = _DataSaveAns.isSuccess;
            //model.StrMsg = _DataSaveAns.StrMsg;
            //model.StackTrace = _DataSaveAns.StackTrace;
            //model.Test = "2021/7/7";

            //return View(model);
            return View(_ImportAnsViewModel);
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
                        BatchTableViewModel.StrBatchMsgNext = "下個檢查:欄位名稱是否相符";
                        BatchTableViewModel.StrCheckMsg = string.Empty;
                        BatchTableViewModel.isExeSetTimeOut = true;

                        Session["BatchType"] = 1;
                    }
                    break;
                case 1:
                    {
                        BatchTableViewModel.StrBatchMsg = "目前檢查:欄位名稱是否相符";
                        BatchTableViewModel.StrBatchMsgNext = "下個檢查:必填欄位沒有填";

                        if (!_dataTubeDA.HasColumns(Session["BatchTable"] as DataTable))
                        {
                            BatchTableViewModel.StrCheckMsg = BatchTableViewModel.StrCheckMsg + "欄位名稱不符合，請參照範本" + Environment.NewLine;
                            System.IO.File.Delete(Session["BatchTablePath"]as string);
                            BatchTableViewModel.isExeSetTimeOut = false;
                        }
                        else
                        {
                            Session["BatchType"] = 2;
                            BatchTableViewModel.isExeSetTimeOut = true;
                        }
                    }
                    break;
                case 2:
                    {
                        BatchTableViewModel.StrBatchMsg = "目前檢查:必填欄位沒有填";
                        BatchTableViewModel.StrBatchMsgNext = "下個檢查:性別是否統一、年齡是否統一";

                        if (!_dataTubeDA.CheckRequired(Session["BatchTable"] as DataTable,out BatchTableViewModel.StrCheckMsg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                            BatchTableViewModel.isExeSetTimeOut = false;
                        }
                        else
                        {
                            Session["BatchType"] = 3;
                            BatchTableViewModel.isExeSetTimeOut = true;
                        }
                    }
                    break;
                case 3:
                    {
                        DataTable table = (Session["BatchTable"] as DataTable);
                        List<string> keys = table.AsEnumerable().GroupBy(e => e.Field<string>("個案代碼")).Where(e => e.Count() > 1).Select(e => e.Key).ToList();
                        int intBatchStartIndex = (Session["intBatchStartIndex"] as int?).HasValue ? (Session["intBatchStartIndex"] as int?).Value : 0;
                        int IntBatchRunCount = 1;
                        string msg = string.Empty;

                        //分批跑,只跑 1百
                        for (int i = intBatchStartIndex; i < keys.Count; i++)
                        {
                            if (IntBatchRunCount > 100)
                            {
                                intBatchStartIndex = i-1;

                                break;
                            }

                            if((i+1).Equals(keys.Count))
                            {
                                intBatchStartIndex = i;
                            }

                            var datas = table.AsEnumerable().Where(e => e.Field<string>("個案代碼").Trim().Equals(keys[i].Trim()));
                            int sexCount = datas.GroupBy(e => e.Field<string>("性別")).Count();

                            if (sexCount > 1)
                            {
                                msg = msg + "個案代碼:" + keys[i] + "性別欄位輸入錯誤" + Environment.NewLine;
                            }

                            double sings = 0;
                            foreach (var item in datas)
                            {
                                double sing = double.Parse(item.Field<string>("收案年份 (西元年)")) - double.Parse(item.Field<string>("年齡 (歲)"));
                                if (sings == 0)
                                {
                                    sings = sing;
                                }

                                if ((sings - sing) > 1.5 || (sings - sing) < -1.5)
                                {
                                    msg = msg + "個案代碼:" + keys[i] + "年齡欄位輸入錯誤(誤差值大於+-1.5)" + Environment.NewLine;
                                }
                            }

                            IntBatchRunCount++;
                        }

                        BatchTableViewModel.StrCheckMsg = msg;
                        if (!string.IsNullOrEmpty(msg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                            Session["isBatchError"] = true;                            
                        }

                        if (!(intBatchStartIndex+1).Equals(keys.Count))
                        {
                            BatchTableViewModel.StrBatchMsg = "目前檢查:性別是否統一、年齡是否統一;總量:"+ keys.Count.ToString()+";目前執行到第幾 "+(intBatchStartIndex+1 ) +" 筆";
                            BatchTableViewModel.StrBatchMsgNext = "下個檢查:主key重複";                            

                            if(!(Session["isBatchError"] as bool?).Value)
                            {
                                BatchTableViewModel.isExeSetTimeOut = true;
                                Session["intBatchStartIndex"] = intBatchStartIndex + 1;
                            }
                            else
                            {
                                BatchTableViewModel.isExeSetTimeOut = false;
                            }
                        }
                        else
                        {
                            BatchTableViewModel.StrBatchMsg = "目前檢查:性別是否統一、年齡是否統一;總量:" + keys.Count.ToString() + ";目前執行到第幾 " + (intBatchStartIndex+1) + " 筆";
                            BatchTableViewModel.StrBatchMsgNext = "下個檢查:主key重複";                            

                            if (!(Session["isBatchError"] as bool?).Value)
                            {
                                Session["BatchType"] = 4;
                                BatchTableViewModel.isExeSetTimeOut = true;

                                // 歸零,重跑或給下一動
                                Session["intBatchStartIndex"] = 0;
                            }
                            else
                            {
                                BatchTableViewModel.isExeSetTimeOut = false;
                            }
                        }

                        #region Old
                        //if(!isCountAll)
                        //{
                        //    BatchTableViewModel.StrBatchMsg = "目前檢查(前半):性別是否統一、年齡是否統一";
                        //    BatchTableViewModel.StrBatchMsgNext = "下個檢查:主key重複";

                        //    if (!_dataTubeDA.BatchRepleData(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg, isCountAll))
                        //    {
                        //        System.IO.File.Delete(Session["BatchTablePath"] as string);
                        //    }
                        //    else
                        //    {
                        //        Session["isCountAll"] = true;
                        //    }
                        //}
                        //else
                        //{
                        //    BatchTableViewModel.StrBatchMsg = "目前檢查(後半):性別是否統一、年齡是否統一";
                        //    BatchTableViewModel.StrBatchMsgNext = "下個檢查:主key重複";

                        //    if (!_dataTubeDA.BatchRepleData(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg, isCountAll))
                        //    {
                        //        System.IO.File.Delete(Session["BatchTablePath"] as string);
                        //    }
                        //    else
                        //    {
                        //        Session["BatchType"] = 4;
                        //    }
                        //}       
                        #endregion
                    }
                    break;
                case 4:
                    {
                        BatchTableViewModel.StrBatchMsg = "目前檢查:主key重複";
                        BatchTableViewModel.StrBatchMsgNext = "下個檢查:各欄位的資料型別 f:m , 數字 , 0:1)";

                        if (!_dataTubeDA.MatchKey(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                            BatchTableViewModel.isExeSetTimeOut = false;
                        }
                        else
                        {
                            Session["BatchType"] = 5;
                            BatchTableViewModel.isExeSetTimeOut = true;
                        }
                    }
                    break;
                case 5:
                    {
                        BatchTableViewModel.StrBatchMsg = "目前檢查:各欄位的資料型別 f:m , 數字 , 0:1)";
                        BatchTableViewModel.StrBatchMsgNext = "下個檢查:部位與診斷代碼(dlinkR)代碼比對";

                        if (!_dataTubeDA.CheckType(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                            BatchTableViewModel.isExeSetTimeOut = false;
                        }
                        else
                        {
                            Session["BatchType"] = 6;
                            BatchTableViewModel.isExeSetTimeOut = true;
                        }
                    }
                    break;
                case 6:
                    {
                        DataTable table = (Session["BatchTable"] as DataTable);
                        var datas = table.AsEnumerable().Select(e => new { regionKey = e.Field<string>("器官/部位代碼"), diagnosisKey = e.Field<string>("診斷代碼") })
                                    .Distinct().ToList();
                        IQueryable<RLinkD> qu = new RLinkDDA().GetQuery();
                        int intBatchStartIndex = (Session["intBatchStartIndex"] as int?).HasValue ? (Session["intBatchStartIndex"] as int?).Value : 0;
                        int IntBatchRunCount = 1;
                        string msg = string.Empty;

                        //分批跑,只跑 1百
                        for (int i = intBatchStartIndex; i < datas.Count; i++)
                        {
                            if (IntBatchRunCount > 100)
                            {
                                intBatchStartIndex = i-1;

                                break;
                            }

                            if ((i + 1).Equals(datas.Count))
                            {
                                intBatchStartIndex = i;
                            }

                            var data = datas[i];

                            bool commit = qu.Where(e => e.diagnosisKey.Equals(data.diagnosisKey) && e.regionKey.Equals(data.regionKey)).Any();

                            if (!commit)
                            {
                                msg = msg + datas[i].diagnosisKey + "(診斷代碼)與" + datas[i].regionKey + "(部位編號)查無相關資料" + Environment.NewLine;
                            }

                            IntBatchRunCount++;
                        }

                        BatchTableViewModel.StrCheckMsg = msg;
                        if (!string.IsNullOrEmpty(msg))
                        {
                            System.IO.File.Delete(Session["BatchTablePath"] as string);
                            Session["isBatchError"] = true;
                        }

                        if(!(intBatchStartIndex+1).Equals(datas.Count))
                        {
                            BatchTableViewModel.StrBatchMsg = "目前檢查:部位與診斷代碼(dlinkR)代碼比對;總量:" + datas.Count.ToString() + ";目前執行到第幾 " + (intBatchStartIndex+1 ) + " 筆";
                            BatchTableViewModel.StrBatchMsgNext = "下個:顯示結果";                            

                            if (!(Session["isBatchError"] as bool?).Value)
                            {
                                BatchTableViewModel.isExeSetTimeOut = true;
                                Session["intBatchStartIndex"] = intBatchStartIndex + 1;
                            }
                            else
                            {
                                BatchTableViewModel.isExeSetTimeOut = false;
                            }
                        }
                        else
                        {
                            BatchTableViewModel.StrBatchMsg = "目前檢查:部位與診斷代碼(dlinkR)代碼比對;總量:" + datas.Count.ToString() + ";目前執行到第幾 " + (intBatchStartIndex+1) + " 筆";
                            BatchTableViewModel.StrBatchMsgNext = "下個:顯示結果";

                            if (!(Session["isBatchError"] as bool?).Value)
                            {
                                Session["BatchType"] = 7;
                                BatchTableViewModel.isExeSetTimeOut = true;

                                // 歸零,重跑或給下一動
                                Session["intBatchStartIndex"] = 0;
                            }
                            else
                            {
                                BatchTableViewModel.isExeSetTimeOut = false;
                            }
                        }

                        #region Old
                        //BatchTableViewModel.StrBatchMsg = "目前檢查:部位與診斷代碼(dlinkR)代碼比對";
                        //BatchTableViewModel.StrBatchMsgNext = "下個:顯示結果";

                        //if (!_rLinkDDA.CheckDLinkR(Session["BatchTable"] as DataTable, out BatchTableViewModel.StrCheckMsg))
                        //{
                        //    System.IO.File.Delete(Session["BatchTablePath"] as string);
                        //}
                        //else
                        //{
                        //    Session["BatchType"] = 7;
                        //}
                        #endregion
                    }
                    break;
                case 7:
                    {
                        ViewBatchDatasViewModel model = new ViewBatchDatasViewModel();
                        model.StrAnsError = BatchTableViewModel.StrAnsError;
                        model.isExeSetTimeOut = BatchTableViewModel.isExeSetTimeOut;
                        model.columns = _dataTubeDA.GetColummns();
                        model.hId = Guid.Parse((Session["hosId"] as string));
                        model.fileName = Session["fileName"] as string;

                        DataTable table = (Session["BatchTable"] as DataTable);
                        List<DataRow> AllDataRow = table.AsEnumerable().ToList();
                        
                        int intBatchStartIndex = (Session["intBatchStartIndex"] as int?).HasValue ? (Session["intBatchStartIndex"] as int?).Value : 0;
                        int IntBatchRunCount = 1;

                        List<TubeDataType> datas = new List<TubeDataType>();

                        if (intBatchStartIndex.Equals(0))
                            model.isFirst = true;

                        for (int i= intBatchStartIndex; i< AllDataRow.Count;i++)
                        {
                            if (IntBatchRunCount > 1000)
                            {
                                intBatchStartIndex = i - 1;

                                break;
                            }

                            if ((i + 1).Equals(AllDataRow.Count))
                            {
                                intBatchStartIndex = i;                                
                            }

                            TubeDataType data = new TubeDataType();
                            foreach (var info in model.columns)
                            {
                                if (info.PropertyType.Name.Equals("Boolean"))
                                {
                                    bool bo = AllDataRow[i][info.DisplayName].Equals("1") ? true : false;
                                    data.GetType().GetProperty(info.Name).SetValue(data, Convert.ChangeType(bo, info.PropertyType), null);
                                }
                                else
                                {
                                    data.GetType().GetProperty(info.Name).SetValue(data, Convert.ChangeType(AllDataRow[i][info.DisplayName], info.PropertyType), null);
                                }
                            }

                            datas.Add(data);

                            IntBatchRunCount++;
                        }

                        if(!(intBatchStartIndex+1).Equals(AllDataRow.Count))
                        {
                            Session["intBatchStartIndex"] = intBatchStartIndex + 1;
                        }
                        else
                        {                            
                            model.isEnd = true;
                            Session["intBatchStartIndex"] = 0;
                        }

                        model.StrBatchMsgNext = "目前儲存=>總量:" + AllDataRow.Count.ToString() + ";目前準備儲存到第幾 " + (intBatchStartIndex + 1) + " 筆";
                        model.datas = datas;

                        //紀錄檔名，若沒有點選儲存則刪除該檔；若有即移到至正式目錄
                        return View("ViewBatchDatas", model); //顯示匯入的資
                    }
            }

            return View("BatchTable",BatchTableViewModel);
        }

        private void setRemoveSession()
        {
            Session.Remove("OldTubeDataCount");
            Session.Remove("BatchTable");
            Session.Remove("BatchTablePath");
            Session.Remove("BatchType");
            Session.Remove("hosId");
            Session.Remove("intBatchStartIndex");
            Session.Remove("isBatchError");
            Session.Remove("fileName");
        }
    }
}