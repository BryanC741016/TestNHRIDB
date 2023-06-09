using ClassLibrary;
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
        private UserLogDA _UserLogDA;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            _userDA = new UserDA(_db);
            _UserLogDA = new UserLogDA(_db);
        }

        //舊網址跳轉
        public ActionResult Login() 
        {
            return RedirectToAction("Index", "Home");
        }

        public ActionResult NoRight() 
        {
            return View();
        }

        public ActionResult Change(string msg = "") 
        {
            AccountViewModel model = new AccountViewModel();
            User user = _userDA.GetUser(_uid);
            model.email = user.email;
            model.name = user.name;
            model.msg = msg;
            return View(model);
        }

        public ActionResult ChangePasswd(ChangePasswd model) 
        {
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

            if (!Regex3TimePasswd(_uid, model.newpasswd))
            {
                TempData["msg"] = "不可以與前三次使用過之密碼相同。";

                return RedirectToAction("Change", new { msg = "不可以與前三次使用過之密碼相同。" });
            }

            string msg = "";
            if (!RegexPasswd(model.newpasswd, out msg))
            {
                TempData["msg"] = msg;

                return RedirectToAction("Change", new { msg = msg });
            }

            //Regex reg = new Regex(@_set.regex);
            //if (!reg.IsMatch(model.newpasswd))
            //{
            //   string msg = string.IsNullOrEmpty(_set.regexMsg) ? "密碼強度不夠，請重新輸入" : _set.regexMsg;
            //   return RedirectToAction("Change", new { msg = msg });
            //} 

            User user = _userDA.HasQuery(_name,model.passwd);
            if (user == null) {

                return RedirectToAction("Change", new { msg = "舊密碼輸入錯誤" });
            }
            _userDA.ChagePasswd(_uid, model.newpasswd);

            #region send email
            UserDA uda = new UserDA(_db);

            List<User> _LitSendUser = new List<User>();
            List<User> _ListUsers = uda.GetQuery().ToList();

            User userLock = uda.GetUser(user.userId);

            GroupDA _GroupDA = new GroupDA(_db);
            List<GroupUser> _LitGroupUser = _GroupDA.GetQuery(gName: "主管理者").ToList();
            Guid groupId = _LitGroupUser.Count > 0 ? _LitGroupUser[0].groupId : Guid.NewGuid();

            if (userLock != null)
            {
                //Session["name"] = userLock.userName;

                foreach (User _User in _ListUsers)
                {
                    if (!userLock.userId.Equals(_User.userId) && userLock.id_Hospital.Equals(_User.id_Hospital) && _User.groupId.Equals(groupId))
                    {
                        _LitSendUser.Add(_User);
                    }
                }
            }

            #region 國衛院系統管理者mail
            foreach (SysEmpid _SysEmpid in _set._LitSysEmpid)
            {
                User _UserSys = new User();// 國衛院系統管理者mail
                _UserSys.email = _SysEmpid.email;
                _UserSys.userName = _SysEmpid.username;

                _LitSendUser.Add(_UserSys);
            }
            #endregion

            MailData mailData = new MailData();
            SendMailer sendMailer = new SendMailer();

            foreach (User _User in _LitSendUser)
            {
                if (!string.IsNullOrEmpty(_User.email))
                {
                    mailData.Set_StrSubject("密碼修改,帳號為:" + userLock.userName + ",醫院:" + userLock.Hospital.name_tw);
                    mailData.Set_StrBody("密碼修改,帳號為:" + userLock.userName + ",醫院:" + userLock.Hospital.name_tw);
                    mailData.Set_StrMail(_User.email);// 被寄的Email,email
                    mailData.Set_StrUsr(_User.userName);// 被寄的人員
                    mailData.Set_StrFromMail("nbctdata@nhri.edu.tw");// 寄的Email,emailUserName->參數多"EmailFromAddr"
                    mailData.Set_StrFromUsr("nbctdata");// 寄的人員,""

                    sendMailer.Set_MailData(mailData);
                    sendMailer.MailSender("sender.nhri.edu.tw", "nbctdata@nhri.edu.tw", string.Empty, 25);
                }
            }
            #endregion

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

        private bool Regex3TimePasswd(Guid userId, string passwd)
        {
            IQueryable<UserLog> _IQUserLog = _UserLogDA.GetUserLog(userId: userId);
            int IntCount = 0;
            bool isRegex3TimePasswd = true;
            CryptoSHA512 crypto = new CryptoSHA512();
            string SHA512_passwd = crypto.CryptoString(passwd);
            _IQUserLog = _IQUserLog.OrderByDescending(m => m.createtime);

            foreach (UserLog _UserLog in _IQUserLog)
            {
                IntCount++;

                if (IntCount > 3)
                {
                    return isRegex3TimePasswd;
                }

                if (SHA512_passwd.Equals(_UserLog.password))
                {
                    isRegex3TimePasswd = false;

                    return isRegex3TimePasswd;
                }
            }

            return isRegex3TimePasswd;
        }

        private bool RegexPasswd(string passwd, out string msg)
        {
            msg = "";
            Regex reg = new Regex(@_set.regex);
            //reg = new Regex(@"^(?=.*\d)(?=.*[A-Za-z]).{10,30}$");

            if (!reg.IsMatch(passwd))
            {
                msg = "密碼強度不夠，請重新輸入，長度至少為10字元以上應包含英文、數字";

                return false;
            }

            //if (!reg.IsMatch(passwd))
            //{
            //    msg = string.IsNullOrEmpty(_set.regexMsg) ? "密碼強度不夠，請重新輸入" : _set.regexMsg;

            //    return false;
            //}

            //if (passwd.Length<10)
            //{
            //    msg = "長度至少為10字元";

            //    return false;
            //}

            return true;
        }
    }
}