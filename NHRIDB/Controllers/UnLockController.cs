using MakeHTML.Models;
using NHRIDB.Filter;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class UnLockController : BasicController
    {
        private LogLoginDA _logLoginDA;


        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _logLoginDA = new LogLoginDA(_db);
        }

        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index()
        {
            List<GetLockUser_Result> model= _logLoginDA.GetLockList(_set.errorOutCount);
            return View(model);
        }

        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public JsonResult SetUnlock(string name) {
            Rs rs = new Rs();
            rs.isSuccess = true;
            _logLoginDA.UnLock(name);
            return Json(rs);
        }
    }
}