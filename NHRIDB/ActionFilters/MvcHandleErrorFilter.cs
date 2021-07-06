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
        public override void OnException(ExceptionContext filterContext)
        {
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();
            var exception = filterContext.Exception;
            //var logger = LogManager.GetLogger($"{controllerName}.{actionName}");
            var message = $"例外：{exception.Message}";
            //logger.Error(exception, message);

            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();

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