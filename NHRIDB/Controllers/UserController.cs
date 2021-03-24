 using MakeHTML.Models;
using NHRIDB.Filter;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
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
     
        private UserDA _userDA;
        private List<Hospital> _hospitalSelect;
        private List<GroupUser> _groupSelect;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            HospitalDA _hospitalDA = new HospitalDA(_db);
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

            _userDA.Create(model.username, model.password,model.hospitalId,model.groupId, model.email,model.name);
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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult Edit(UserEdit model) {
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
            
             
            _userDA.Edit(model.uid,model.username, model.hospitalId,  model.groupId, model.email,model.name);
            return RedirectToAction("Index");

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [MvcAdminRightAuthorizeFilter(param = 'w')]
        public ActionResult ChangePasswd(ChangePasswd model,Guid uid)
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

            _userDA.ChagePasswd(uid, model.newpasswd);
            TempData["msg"] = "修改完畢";
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

            _userDA.Delete(id);
            rs.isSuccess = true;
            return Json(rs);
        }

        private bool RegexPasswd(string passwd, out string msg) {
            msg = "";
            
            Regex reg = new Regex(@_set.regex);
            if (!reg.IsMatch(passwd))
            {
                 msg = string.IsNullOrEmpty(_set.regexMsg) ? "密碼強度不夠，請重新輸入" : _set.regexMsg;
                return false;
            }

            return true;
        }

    }
}