using NHRIDB.Filter;
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
       
       [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index()
        {
            ProjectSetViewModel model = new ProjectSetViewModel(_path);


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Index(ProjectSetViewModel model) {
            if (!ModelState.IsValid) {
                return View(model);
            }
         
            if (model.startDate.Value > model.endDate.Value) {
                ModelState.AddModelError(string.Empty, "開始時間不得大於結束時間");
                return View(model);
            }
            if (model.errorOutCount < 0) {
                ModelState.AddModelError(string.Empty, "次數必須大於0");
                return View(model);
            }
            XmlDocument xmlDoc = new XmlDocument();
            string xml = "<set>" +
                "<startDate>" +
                 model.startDate
                +"</startDate>"
                +"<endDate>" +
                      model.endDate +
               "</endDate>"+
                "<regex>" +
                      model.regex +
               "</regex>" +
                 "<regexMsg>" +
                      model.regexMsg +
               "</regexMsg>" +
                 "<errorOutCount>" +
                      model.errorOutCount.ToString() +
               "</errorOutCount>" +
               "</set>";
            xmlDoc.LoadXml(xml);
            xmlDoc.Save(_path);
            ModelState.AddModelError(string.Empty, "修改完成");
            return View(model);
        }
    }
}