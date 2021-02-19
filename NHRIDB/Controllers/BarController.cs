using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class BarController : BasicController
    {
        private GetTotalDA _totalDA;
        private HospitalDA _hospitalDA;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _totalDA = new GetTotalDA(_db);
            _hospitalDA = new HospitalDA(_db);
        }

        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Bar
        public ActionResult Index(Nullable<Guid> treeId=null, Nullable<Guid> hosId=null)
        {
            BarViewModel model = new BarViewModel();
            model.treeId = treeId;
            model.leapProject = _leapProject;
            model.selfHos = _hospitalDA.GetHospital(_hos);
            model.hospitalSelect = new SelectList(_hospitalDA.GetQuery().ToList(), "id", "name_tw");
            if (!_leapProject)
            {
                hosId = _hos;
            }
            
            // model.items = _totalDA.GetQuery(treeId, hosId);
            

           

            return View(model);
        }
    }
}