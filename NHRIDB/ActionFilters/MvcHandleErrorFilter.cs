using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.ActionFilters
{
    public class MvcHandleErrorFilter : HandleErrorAttribute
    {
        private NHRIDBEntitiesDB _db;
        private ErrorLogDA _ErrorLogDA;

        public override void OnException(ExceptionContext filterContext)
        {
            _db = new NHRIDBEntitiesDB();
            _ErrorLogDA = new ErrorLogDA(_db);

            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();
            var exception = filterContext.Exception;            
            var message = $"例外：{exception.Message}";

            //var logger = LogManager.GetLogger($"{controllerName}.{actionName}");
            //logger.Error(exception, message);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
            
            string id = DateTime.Now.ToString("yyyyMMddHHmmss") + Convert.ToString(HttpContext.Current.Session["name"]);
            _ErrorLogDA.Create(id:id, controller: controllerName, action: actionName, message: exception.Message, stacktrace: filterContext.Exception.StackTrace);
            controllerName = id;

            var handler = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
            var viewData = new ViewDataDictionary<HandleErrorInfo>(handler);

            //viewData.Add("Request_Variable", "a");//傳入自訂的ViewData
            //var viewName = "/Views/Error/Error.cshtml";

            filterContext.Result = new ViewResult
            {
                ViewName = "/Views/Shared/Error.cshtml",
                //MasterName = this.Master,
                ViewData = viewData,
                TempData = filterContext.Controller.TempData
            };
        }
    }
}