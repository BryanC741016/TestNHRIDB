using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class UploadListController : BasicController
    {
        private SysLogDA _SysLogDA;
        private HospitalDA _hospitalDA;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            _hospitalDA = new HospitalDA(_db);
            _SysLogDA = new SysLogDA(_db);
        }
        // GET: UploadList
        public ActionResult Index()
        {
            HospitalUploadViewModel model = new HospitalUploadViewModel();
            model.items =
                _db.Hospital
                    .Select(e => new HospitalUploadlist
                    {
                        HospitalId = e.id,
                        name_tw = e.name_tw,
                        name_en = e.name_en,
                        Count = _db.TubeData
                                .Where(f => f.hospitalId.Equals(e.id)).Count(),
                        HasRow = _db.TubeData
                                .Where(f => f.hospitalId.Equals(e.id)).Count() > 0 ? true : false,
                        LastDate = _db.TubeData
                                .Where(f => f.hospitalId.Equals(e.id))
                                .OrderByDescending(f => f.createDate)
                                .Select(f => f.createDate)
                                .FirstOrDefault()
                    })
                    .OrderByDescending(e => e.LastDate)
                    .ToList();
            ViewData.Model = model;
            return View();
        }
    }
}