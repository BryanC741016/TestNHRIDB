using NHRIDB.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace NHRIDB.Controllers
{
    public class ProjectSetController : BasicController
    {
        private string _path = "~/Setting/Setting.xml";
       [HttpGet]
        public ActionResult Index()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath(_path));
            XmlNode root = xmlDoc.SelectSingleNode("set");
            string startDateXML = root.SelectSingleNode("startDate").InnerText;
            string endDateXML = root.SelectSingleNode("endDate").InnerText;
            ProjectSetViewModel model = new ProjectSetViewModel();
            model.endDate = DateTime.Parse(endDateXML);
            model.startDate = DateTime.Parse(startDateXML);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ProjectSetViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
            if (model.startDate.Value > model.endDate.Value) {
                ModelState.AddModelError(string.Empty, "開始時間不得大於結束時間");
                return View(model);
            }
            XmlDocument xmlDoc = new XmlDocument();
            string xml = "<set><startDate>"+
                 model.startDate.Value.ToString("yyyy/MM/dd")
                +"</startDate>"
                +"<endDate>" +
                      model.endDate.Value.ToString("yyyy/MM/dd")
               + "</endDate></set>";
            xmlDoc.LoadXml(xml);
            xmlDoc.Save(Server.MapPath(_path));
            return View(model);
        }
    }
}