using ClassLibrary;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.IO;
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
        private NHRIDBEntitiesDB _db;
        private SysLogDA _SysLogDA;
        private string _ip;
        
        private ProjectSet _set { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _ip = GetIp();
            _db = new NHRIDBEntitiesDB();
            _SysLogDA = new SysLogDA(_db);

            _set = new ProjectSet(Server.MapPath("~/Setting/Setting.xml"));           
        }

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
             
            model.endDate = _set.endDate;
            model.startDate = _set.startDate;

            if (!System.IO.Directory.Exists(Server.MapPath("~/Logs"))){
                System.IO.Directory.CreateDirectory(Server.MapPath("~/Logs"));
            }
           
            if (!System.IO.Directory.Exists(Server.MapPath("~/Logs/ActionLog")))
            {
                System.IO.Directory.CreateDirectory(Server.MapPath("~/Logs/ActionLog"));
            }
             
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.message = "請確實填寫資料";
                return View(model);
            }

            if (string.IsNullOrEmpty(model.tonken) || string.IsNullOrEmpty(model.textshowCode))
            {
                model.message = "請輸入正確的驗證碼";
                return View(model);
            }

            string tonken = model.tonken.Substring(1, model.tonken.Length - 1).Replace(",", "");
            if (model.tonken.IndexOf(",") < 0 || !tonken.Equals(model.textshowCode))
            {
                model.message = "請輸入正確的驗證碼";

                return View(model);
            }

            NHRIDBEntitiesDB db = new NHRIDBEntitiesDB();
            UserDA uda = new UserDA(db);
            LogLoginDA logLoginDA = new LogLoginDA(db);

            if (logLoginDA.HasLock(_set.errorOutCount,model.userName))
            {
                model.message = "錯誤次數過多，已被鎖住";
                model.isLock = true;
                return View(model);
            }
           
            User user = uda.HasQuery(model.userName, model.passwd);

            if (user == null)
            {
                logLoginDA.Create(model.userName, _ip, false);             
            }
            else{

                logLoginDA.Create(model.userName, _ip, true);

                bool isstart = user.isstart.HasValue ? user.isstart.Value : false;

                if (!isstart)
                {
                    model.message = "帳號未啟用";

                    return View(model);
                }

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
                   .Where(e =>  list.Any(x => x.parentMenu == e.menuId && ((int)x.purview) >= 1))
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

                if (user.GroupUser.alwaysOpen || (now >= model.startDate && now <= model.endDate))
                {
                    _SysLogDA.Create(evettype: "使用者登入", ip: _ip, userName: Convert.ToString(Session["name"]));

                    FormsAuthentication.RedirectFromLoginPage(user.userId.ToString(), false);

                    return RedirectToAction("Index", "Bar");                
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