using ClassLibrary;
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
using System.Xml;

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

        protected string _path = "~/Setting/Setting.xml";
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            string action = requestContext.RouteData.Values["Action"].ToString();
            string controller = requestContext.RouteData.Values["Controller"].ToString();
            Logs.WriteLog(Server.MapPath("~/Logs/ActionLog"),  Session["uid"].ToString()+"|"+ controller+"/"+ action);
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

        protected string GetImgPath(Guid hosId,string exName) {
            string path = Path.Combine(_imgDirPath, hosId.ToString() + "." + exName);
            if (System.IO.File.Exists(path)) {
                return path.Replace((Server.MapPath("~/")), "/").Replace("\\", "/");
            }

            return "";
        }

        protected ProjectSetViewModel GetProjSet() {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath(_path));
            XmlNode root = xmlDoc.SelectSingleNode("set");
            string startDateXML = root.SelectSingleNode("startDate").InnerText;
            string endDateXML = root.SelectSingleNode("endDate").InnerText;
            string regex = root.SelectSingleNode("regex").InnerText;
            string regexMsg = root.SelectSingleNode("regexMsg").InnerText;
            string errorOutCount = root.SelectSingleNode("errorOutCount").InnerText;
            ProjectSetViewModel model = new ProjectSetViewModel();
            model.endDate = DateTime.Parse(endDateXML);
            model.startDate = DateTime.Parse(startDateXML);
            model.regex = regex;
            model.errorOutCount = string.IsNullOrEmpty(errorOutCount) ? 0 : int.Parse(errorOutCount);
            model.regexMsg = regexMsg;
            return model;
        }
    }
}