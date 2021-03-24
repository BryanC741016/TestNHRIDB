using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using NHRIDBLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NHRIDB.Controllers
{
    public class AccountController : BasicController
    {
        private UserDA _userDA;
        
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _userDA = new UserDA(_db);
        }

        //舊網址跳轉
        public ActionResult Login() {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult NoRight() {
            return View();
        }

        public ActionResult Change(string msg = "") {
            AccountViewModel model = new AccountViewModel();
            User user = _userDA.GetUser(_uid);
            model.email = user.email;
            model.name = user.name;
            model.msg = msg;
            return View(model);
        }



        public ActionResult ChangePasswd(ChangePasswd model) {
            if (!ModelState.IsValid) {
                List<string> errors = ModelState.Values.Where(E => E.Errors.Count > 0)
                 .SelectMany(E => E.Errors)
                 .Select(E => E.ErrorMessage)
                 .ToList();
                return RedirectToAction("Change", new { msg = string.Join(",", errors) });
            }

            if (!model.newpasswd.Equals(model.repasswd)) {
                return RedirectToAction("Change", new { msg = "確認密碼必須與新密碼相同" });
            }
            
            
            Regex reg = new Regex(@_set.regex);
            if (!reg.IsMatch(model.newpasswd))
            {
               string msg = string.IsNullOrEmpty(_set.regexMsg) ? "密碼強度不夠，請重新輸入" : _set.regexMsg;
               return RedirectToAction("Change", new { msg = msg });
            } 

            User user = _userDA.HasQuery(_name,model.passwd);
            if (user == null) {
                return RedirectToAction("Change", new { msg = "舊密碼輸入錯誤" });
            }
            _userDA.ChagePasswd(_uid, model.newpasswd);


            return RedirectToAction("Change", new { msg = "執行完畢" });
        }

        public ActionResult ChangeEmail(ChangeData model)
        {
            if (!ModelState.IsValid)
            {
               List<string> errors= ModelState.Values.Where(E => E.Errors.Count > 0)
                 .SelectMany(E => E.Errors)
                 .Select(E => E.ErrorMessage)
                 .ToList();
                return RedirectToAction("Change",new { msg = string.Join(",",errors) });
            }
            _userDA.Edit(_uid, name: model.name, email: model.email);
            return RedirectToAction("Change", new { msg = "執行完畢" });
        }

    }
}