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
        private HospitalDA _hospitalDA;
        private TubeDataTotalDA _tubeTotal;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
          
            _hospitalDA = new HospitalDA(_db);
            _tubeTotal= new TubeDataTotalDA();
        }

        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: Bar
        public ActionResult Index(Nullable<Guid> hosId=null)
        {
            BarViewModel model = new BarViewModel();
     
            model.leapProject = _leapProject;
            model.selfHos = _hospitalDA.GetHospital(_hos);
            model.hospitalSelect = new SelectList(_hospitalDA.GetQuery().ToList(), "id", "name_tw");
            if (!_leapProject)
            {
                hosId = _hos;
            }

            model.datas = _tubeTotal.GetTotal(hosId);
            model.columns = _tubeTotal.GetColummns();

            return View(model);
        }
    }
}