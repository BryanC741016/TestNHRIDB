﻿using ClassLibrary;
using MakeHTML.Models;
using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using NHRIDBLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace NHRIDB.Controllers
{
    public class UserController : BasicController
    {
        private SysLogDA _SysLogDA;
        private UserLogDA _UserLogDA;
        private UserDA _userDA;
        private List<Hospital> _hospitalSelect;
        private List<GroupUser> _groupSelect;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            HospitalDA _hospitalDA = new HospitalDA(_db);
            _SysLogDA = new SysLogDA(_db);
            _UserLogDA = new UserLogDA(_db);
            _userDA = new UserDA(_db);
            _hospitalSelect = _hospitalDA.GetQuery().ToList();
            GroupDA groupDA = new GroupDA(_db);
            _groupSelect = groupDA.GetQuery().ToList();
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        // GET: User
        public ActionResult Index(int pageNumber = 1, string sortColumn = "", string sortType = "")
        {
            UserModelView model = new UserModelView();
            if (TempData["USForm"] != null)
                model = TempData["USForm"] as UserModelView;
            model.setData(pageNumber, sortColumn, sortType);
            return Index(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'r')]
        public ActionResult Index(UserModelView model)
        {

            model.hospitalSelect = new SelectList(_hospitalSelect,"id","name_tw",model.searchHospitalID);
            var items = _userDA.GetQuery(hosID: model.searchHospitalID,email:model.searchEmail,userName:model.searchUserName)
                .Select(e => new UserItem
                {
                  email=e.email,
                  userName=e.userName,
                  id=e.userId,
                  hosName=e.Hospital.name_tw
                }).ToList();

            model.items = model.GetSortColumnList(items, "userName");
            TempData["USForm"] = model;
            return View(model);
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Create() {
            UserCreate model = new UserCreate();
           model.hospitalSelect = new SelectList(_hospitalSelect, "id", "name_tw");
            model.msSelect = new SelectList(_groupSelect, "groupId", "gName");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Create(UserCreate model)
        {
            model.msSelect = new SelectList(_groupSelect, "groupId", "gName");
            model.hospitalSelect = new SelectList(_hospitalSelect, "id", "name_tw");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string msg = "";
            if (!RegexPasswd(model.password,out msg)) {
                ModelState.AddModelError(string.Empty, msg);
                return View(model);
            }
           
            if (!model.password.Equals(model.repassword)) {
             
                ModelState.AddModelError(string.Empty, "密碼與確認密碼不相同");
                return View(model);
            }

            int count=   _userDA.GetQuery(userName: model.username).Count();
            if (count > 0) {

                ModelState.AddModelError(string.Empty, "此帳號已被使用");
                return View(model);
            }

            _SysLogDA.Create(evettype: "新增使用者帳號:" + model.username, ip: this.GetIp(), userName: Convert.ToString(Session["name"]));
            Guid userId = Guid.NewGuid();
            _userDA.Create(userId, model.username, model.password,model.hospitalId,model.groupId, model.email,model.name, isstart:model.isstart);
            _UserLogDA.Create(userId: userId,userName: model.username,password: model.password);

            return RedirectToAction("Index");
        }

        [HttpGet]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Edit(Guid id)
        {
            User user = _userDA.GetUser(id);
            UserEdit model = new UserEdit();

            if (user == null) {
            
                ModelState.AddModelError(string.Empty, "查無此資料");
                return View(model);
            }
          
            model.uid = id;
            model.hospitalId = user.id_Hospital;
            model.groupId = user.groupId;
            model.hospitalSelect = new SelectList(_hospitalSelect, "id", "name_tw",user.id_Hospital);
            model.msSelect = new SelectList(_groupSelect, "groupId", "gName");
            model.username = user.userName;
            model.email = user.email;
            model.name = user.name;
            model.isstart = user.isstart.HasValue? user.isstart.Value:false;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Edit(UserEdit model) 
        {
            model.hospitalSelect = new SelectList(_hospitalSelect, "id", "name_tw", model.hospitalId);
            model.msSelect = new SelectList(_groupSelect, "groupId", "gName", model.groupId);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = _userDA.GetUser(model.uid);
         
            if (user == null)
            {
               
                ModelState.AddModelError(string.Empty, "查無此資料");
                return View(model);
            }
           
            int count = _userDA.GetQuery(userName: model.username,noID:model.uid).Count();

            if (count > 0)
            {
                
                ModelState.AddModelError(string.Empty, "此帳號已被使用");
                return View(model);
            }

            _SysLogDA.Create(evettype: "修改使用者:" + model.username+",啟用狀態:"+ model.isstart, ip: this.GetIp(), userName: Convert.ToString(Session["name"]));
            _userDA.Edit(model.uid,model.username, model.hospitalId,  model.groupId, model.email,model.name, isstart:model.isstart);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult ChangePasswd(ChangePasswd model,Guid uid)// ChangePasswdController.cs / AccountController.cs
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "請填寫資料";
                return RedirectToAction("Edit", new { id = uid });
            }

            string msg = "";
            if (!RegexPasswd(model.newpasswd, out msg))
            {
                TempData["msg"] = msg;
                return RedirectToAction("Edit", new { id = uid });
            }

            if (!model.newpasswd.Equals(model.repasswd))
            {
                TempData["msg"] = "密碼與確認密碼不相同";
             
                return RedirectToAction("Edit", new { id = uid });
            }

            if (!Regex3TimePasswd(uid, model.newpasswd))
            {
                TempData["msg"] = "不可以與前三次使用過之密碼相同。";

                return RedirectToAction("Edit", new { id = uid });
            }

            _SysLogDA.Create(evettype: "修改使用者密碼", ip: this.GetIp(), userName: Convert.ToString(Session["name"]));
            _userDA.ChagePasswd(uid, model.newpasswd);
            _UserLogDA.Create(userId: uid, userName: Convert.ToString(Session["name"]), password: model.newpasswd);
            TempData["msg"] = "修改完畢";

            #region send email
            UserDA uda = new UserDA(_db);

            List<User> _LitSendUser = new List<User>();
            List<User> _ListUsers = uda.GetQuery().ToList();

            User userLock = uda.GetUser(uid);

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

            return RedirectToAction("Edit",new { id= uid });
        }

        [AjaxValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public JsonResult Delete(Guid id)
        {
            Rs rs = new Rs();
            rs.isSuccess = false;
            User hos = _userDA.GetUser(id);

            if (hos == null)
            {
                rs.message = "查無此資料";
                return Json(rs);
            }

            _SysLogDA.Create(evettype: "刪除使用者", ip: this.GetIp(), userName: Convert.ToString(Session["name"]));
            _userDA.Delete(id);
            rs.isSuccess = true;

            return Json(rs);
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

        private bool Regex3TimePasswd(Guid userId,string passwd)
        {
            IQueryable <UserLog> _IQUserLog = _UserLogDA.GetUserLog(userId: userId);
            int IntCount = 0;
            bool isRegex3TimePasswd = true;
            CryptoSHA512 crypto = new CryptoSHA512();
            string SHA512_passwd = crypto.CryptoString(passwd);
            _IQUserLog = _IQUserLog.OrderByDescending(m=>m.createtime);

            foreach (UserLog _UserLog in _IQUserLog)
            {
                IntCount++;

                if(IntCount>3)
                {
                    return isRegex3TimePasswd;
                }

                if(SHA512_passwd.Equals(_UserLog.password))
                {
                    isRegex3TimePasswd = false;

                    return isRegex3TimePasswd;
                }
            }

            return isRegex3TimePasswd;
        }
    }
}