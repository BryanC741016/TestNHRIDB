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
       
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
     
        }

        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Form
        public ActionResult Index()
        {
           
            return View( );
        }

       

        public ActionResult Upload(HttpPostedFileBase upload) {
            string ex = upload == null ? null : Path.GetExtension(upload.FileName).Replace(".", "");
            if (string.IsNullOrEmpty(ex))
            {
                TempData["msg"] = "請選擇檔案";
                return RedirectToAction("Index");
            }
            else{
                string[] allow = new string[] { "xlsx", "csv" };
                if (!allow.Contains(ex))
                {
                    TempData["msg"] = "不支援此格式上傳";
                    return RedirectToAction("Index");
                }
            }

            /***
             * 1.欄位名稱是否相符
             * 2.性別是否統一
             * 3.年齡是否統一
             * 4.部位與診斷代碼(dlinkR)代碼比對
             * **/
            EPPlusExcel epp = new EPPlusExcel();
            DataTable table= epp.GetDataTableFromExcel(upload.InputStream);
            string[] columns = { "識別ID", "部位代碼", "診斷代碼", "收案年份", "年齡", "性別" };
     
            bool commit =  hasColumns(table, columns);
            if (!commit) {
                TempData["msg"] = "欄位名稱不符合，請參照範本";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

        private bool hasColumns(System.Data.DataTable dataTable, string[] format)
        {
            string[] columnNames = dataTable.Columns.Cast<DataColumn>()
                                    .Select(x => x.ColumnName)
                                    .ToArray();
            return ((from item in format
                     where columnNames.Contains(item)
                     select item).Count() == format.Length);
        }
    }
}