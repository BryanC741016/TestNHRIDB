using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NHRIDB.Controllers
{
    [LogonAuthorizeFilter]
    public class BasicController : Controller
    {
        protected Guid _uid { get; set; }
 
        protected Guid _hos { get; set; }
        protected string _name { get; set; }
         protected bool _leapProject { get; set; }

        protected string _exName { get; set; }
        protected List<PurviewModel> _funcList { get; set; }
        protected NHRIDBEntitiesDB _db { get; set; }

        protected string _imgDirPath { get; set; }
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            SetInitialData();
        }

        private void SetInitialData() {
            try
            {
                _imgDirPath = Server.MapPath("~/images/HospitalLog");
                _uid = Guid.Parse(Session["uid"].ToString());
                _hos = Guid.Parse(Session["hos"].ToString());
                _name = Session["name"].ToString();
                _exName = Session["ex"]==null ?"": Session["ex"].ToString();
                _leapProject = Boolean.Parse(Session["leapProject"].ToString());
                Session["imgPath"] = GetImgPath(_hos,_exName);

                _funcList = Session["funcList"] as List<PurviewModel>;
            }
            catch { 
            
            }
         
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _db = new NHRIDBEntitiesDB();
        }

        protected private string GetImgPath(Guid hosId,string exName) {
            string path = Path.Combine(_imgDirPath, hosId.ToString() + "." + exName);
            if (System.IO.File.Exists(path)) {
                return path.Replace((Server.MapPath("~/")), "/").Replace("\\", "/");
            }

            return "";
        }
    }
}