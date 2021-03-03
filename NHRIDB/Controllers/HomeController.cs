using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace NHRIDB.Controllers
{
    public class HomeController : Controller
    {
       
        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {

            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();

            base.OnResultExecuting(filterContext);
        }
        // GET: Account
        public ActionResult Index()
        {
            LoginViewModel model = new LoginViewModel();
            model.imgUrl = new List<string>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(Server.MapPath("~/Setting/Setting.xml"));
            XmlNode root = xmlDoc.SelectSingleNode("set");
            string startDateXML = root.SelectSingleNode("startDate").InnerText;
            string endDateXML = root.SelectSingleNode("endDate").InnerText;
            

            model.endDate = DateTime.Parse(endDateXML);
            model.startDate = DateTime.Parse(startDateXML);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.tonken) || string.IsNullOrEmpty(model.textshowCode)) {
                model.message = "請輸入正確的驗證碼";
                return View(model);
            }

            string tonken = model.tonken.Substring(1, model.tonken.Length - 1).Replace(",", "");
            if (model.tonken.IndexOf(",")< 0 || !tonken.Equals(model.textshowCode))
            {
                model.message = "請輸入正確的驗證碼";
                return View(model);
            }
            if (!ModelState.IsValid)
            {
                model.message = "請確實填寫資料";
                return View(model);
            }
             

            NHRIDBEntitiesDB db = new NHRIDBEntitiesDB();
            UserDA uda = new UserDA(db);
            LogLoginDA log = new LogLoginDA(db);
            string ip = GetIp();
            User user = uda.HasQuery(model.userName, model.passwd);
            if (user == null)
            {
                log.Create(model.userName, ip, false);
            }
            else{
                log.Create(user.userName, ip, true);
                Session["uid"] = user.userId;
                Session["hos"] = user.id_Hospital;
                Session["name"] = user.userName;
                Session["ex"] = user.Hospital.fileExtension;
                Session["leapProject"] = user.GroupUser.leapProject;
                List<PurviewModel> list= user.GroupUser.Permissions
                      .Where(e=>e.purview>=1)
                     .Select(e => new PurviewModel {
                       controllName=e.MenuName.controllerName,
                       menuId=e.menuId,
                       parentMenu=e.MenuName.parentMenu,
                       menuText=e.MenuName.menuText,
                       purview=(PermissionsKind)e.purview,
                       sortIndex=e.MenuName.sortIndex
                      })
                     .ToList();

                MenuDA menu = new MenuDA(db);
                List<PurviewModel> parentMenu =  menu.GetQuery(isNullParent: true)
                    .ToList()
                   .Where(e => list.Any(x => x.parentMenu == e.menuId && ((int)x.purview) >= 1))
                   .Select(e => new PurviewModel
                   {
                       controllName = e.controllerName,
                       menuId = e.menuId,
                       parentMenu = null,
                       menuText = e.menuText,
                       purview = PermissionsKind.Read,
                       sortIndex=e.sortIndex
                   }).ToList();

                List<PurviewModel> fun = new List<PurviewModel>();
                fun.AddRange(list);
                fun.AddRange(parentMenu);
                Session["funcList"] = fun;

                DateTime now = DateTime.Now;
                if (user.GroupUser.alwaysOpen)
                {
                    FormsAuthentication.RedirectFromLoginPage(user.userId.ToString(), false);
                    return RedirectToAction("Index", "Import");
                }
                else if (now >= model.startDate && now <= model.endDate) {
                    FormsAuthentication.RedirectFromLoginPage(user.userId.ToString(), false);
                    return RedirectToAction("Index", "Import");
                }
                model.message = "未開放或無權限";
                return View(model);
            }
            model.message = "帳號密碼錯誤請重新填寫";
            return View(model);
        }

        private string GetIp()
        {
            string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }


    }
}