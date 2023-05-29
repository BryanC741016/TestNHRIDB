using ClassLibrary;
using NHRIDB.Models.ViewModels;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
            #region Send Email Test
            //MailData mailData = new MailData();
            //mailData.Set_StrSubject("Test");
            //mailData.Set_StrBody("TestA");
            //mailData.Set_StrMail("Bryanc@e-tec.com.tw");// 被寄的Email,email
            //mailData.Set_StrUsr("Bryanc");// 被寄的人員
            ////mailData.Set_StrFromMail("Bryanc@e-tec.com.tw");// 寄的Email,emailUserName->參數多"EmailFromAddr"
            //mailData.Set_StrFromMail("noreply@nhri.edu.tw");// 寄的Email,emailUserName->參數多"EmailFromAddr"
            //mailData.Set_StrFromUsr("");// 寄的人員,""

            //SendMailer sendMailer = new SendMailer();
            //sendMailer.Set_MailData(mailData);
            ////sendMailer.MailSender("abba.e-tec.com.tw", "Bryanc@e-tec.com.tw", "Hfi6@1016", 25);
            //sendMailer.MailSender("sender.nhri.edu.tw", "noreply@nhri.edu.tw", string.Empty, 25);
            //// mailHost:RTE置換排程 發送E-MAIL主機IP abba.e-tec.com.tw
            //// emailUserName:RTE置換排程 發送E-MAIL主機帳號 sdservice
            //// emailPassword:RTE置換排程 發送E-MAIL主機帳號密碼 Rh#T53f
            //// port:25 RTE置換排程 發送E-MAIL主機port 25
            #endregion

            LoginViewModel model = new LoginViewModel();
            model.imgUrl = new List<string>();

            model.endDate = _set.endDate;
            model.startDate = _set.startDate;

            if (!System.IO.Directory.Exists(Server.MapPath("~/Logs")))
            {
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
                /*
                 * model.userName->找同醫院 + 等級為主管級 + 不是自已的->迴圈寄信
                 */
                #region Get Send Emails And Send Email
                List<User> _LitSendUser = new List<User>();
                List<User> _ListUsers = uda.GetQuery().ToList();
                User userLock = uda.HasQuery(model.userName, model.passwd);

                GroupDA _GroupDA = new GroupDA(db);
                List<GroupUser> _LitGroupUser = _GroupDA.GetQuery(gName: "主管理者").ToList();
                Guid groupId = _LitGroupUser.Count > 0 ? _LitGroupUser[0].groupId : new Guid();

                if (userLock !=null)
                {
                    foreach(User _User in _ListUsers)
                    {
                        if(!userLock.userId.Equals(_User.userId) && userLock.id_Hospital.Equals(_User.id_Hospital) && userLock.groupId.Equals(groupId))
                        {
                            _LitSendUser.Add(_User);
                        }
                    }
                }

                MailData mailData = new MailData();
                SendMailer sendMailer = new SendMailer();                             

                foreach (User _User in _LitSendUser)
                {
                    if(!string.IsNullOrEmpty(_User.email))
                    {
                        mailData.Set_StrSubject("國衛院登入帳號已被鎖住,帳號為:" + userLock.userName + ",醫院:" + userLock.Hospital.name_tw);
                        mailData.Set_StrBody("國衛院登入帳號已被鎖住,帳號為:"+ userLock.userName+",醫院:"+ userLock.Hospital.name_tw);
                        mailData.Set_StrMail(_User.email);// 被寄的Email,email
                        mailData.Set_StrUsr(_User.userName);// 被寄的人員
                        mailData.Set_StrFromMail("noreply@nhri.edu.tw");// 寄的Email,emailUserName->參數多"EmailFromAddr"
                        mailData.Set_StrFromUsr("noreply");// 寄的人員,""

                        sendMailer.Set_MailData(mailData);
                        sendMailer.MailSender("sender.nhri.edu.tw", "noreply@nhri.edu.tw", string.Empty, 25);
                    }
                }  
                #endregion

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

                List<LogLogin> _LitLogLogin = logLoginDA.Query(userName:model.userName);
                _LitLogLogin = _LitLogLogin.Where(m=>m.isLogin.Equals(true)).ToList();
                _LitLogLogin = _LitLogLogin.OrderByDescending(m=>m.lognDate).ToList();
                Session["name"] = user.userName;
                Session["uid"] = user.userId;

                if (_LitLogLogin.Count<2 || _LitLogLogin[0].lognDate<=DateTime.Now.AddMonths(-6) || !user.passwordtime.HasValue || user.passwordtime.Value<= DateTime.Now.AddMonths(-6))
                {
                    if (_LitLogLogin.Count < 2)
                        logLoginDA.Delete(userName: user.userName, isLogin:true, lognDate: _LitLogLogin[0].lognDate);

                    Session.Remove("hos");
                    Session.Remove("ex");
                    Session.Remove("leapProject");
                    Session.Remove("funcList");

                    return RedirectToAction("Edit", "ChangePasswd",new { id = user.userId });
                }
                
                Session["hos"] = user.id_Hospital;                
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
                    _SysLogDA.DeleteSixMonths();
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

    public class MailData
    {
        private string _strFromUsr = "";
        private string _strFromMail = "";
        private string _strUsr = "";
        private string _strMail = "";
        private string _strBody = "";
        private string _strSubject = "";
        private string _strCC = "";

        // PURPOSE :設定變數值
        // PARAM1 : STRING 要設定的變數之值
        public void Set_StrFromUsr(string strVal)
        {
            _strFromUsr = strVal;
        }
        public void Set_StrFromMail(string strVal)
        {
            _strFromMail = strVal;
        }
        public void Set_StrUsr(string strVal)
        {
            _strUsr = strVal;
        }
        public void Set_StrMail(string strVal)
        {
            _strMail = strVal;
        }
        public void Set_StrBody(string strVal)
        {
            _strBody = strVal;
        }
        public void Set_StrSubject(string strVal)
        {
            _strSubject = strVal;
        }
        public void Set_StrCC(string strVal)
        {
            _strCC = strVal;
        }


        // PURPOSE :取得變數值
        // RETURN : STRING
        public string Get_StrFromUsr()
        {
            return _strFromUsr;
        }
        public string Get_StrFromMail()
        {
            return _strFromMail;
        }
        public string Get_StrUsr()
        {
            return _strUsr;
        }
        public string Get_StrMail()
        {
            return _strMail;
        }
        public string Get_StrBody()
        {
            return _strBody;
        }
        public string Get_StrSubject()
        {
            return _strSubject;
        }
        public string Get_StrCC()
        {
            return _strCC;
        }
    }

    public class SendMailer
    {
        private MailData _maildata = new MailData();

        // PURPOSE :設定變數值
        public void Set_MailData(MailData MailData)
        {
            _maildata = MailData;
        }

        // PURPOSE :email 處理
        public void MailSender(string mailServer, string account, string password, int port)
        {
            SmtpClient smtp = new SmtpClient(mailServer, port);

            if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password))
            {
                smtp.Credentials = new NetworkCredential(account, password);
            }
            smtp.Port = port;
            MailMessage Msg = new MailMessage();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            System.Text.Encoding headerEncoding = System.Text.Encoding.BigEndianUnicode;

            MailAddress mailFrom = new MailAddress(this._maildata.Get_StrFromMail(), this._maildata.Get_StrFromUsr(), encoding);

            Msg.From = mailFrom;

            try
            {
                MailAddress mailTo = new MailAddress(this._maildata.Get_StrMail(), this._maildata.Get_StrUsr(), headerEncoding);

                // 內容使用html
                Msg.IsBodyHtml = true;
                Msg.BodyEncoding = System.Text.Encoding.UTF8;
                // 前面是發信email後面是顯示的名稱
                // Msg.From = New MailAddress("system@mail.com", "system")
                // 收信者email 
                Msg.To.Add(mailTo);

                // CC
                if (_maildata.Get_StrCC().Length > 0)
                    Msg.CC.Add(_maildata.Get_StrCC());
                // 設定優先權
                Msg.Priority = MailPriority.Normal;
                // 標題
                Msg.Subject = _maildata.Get_StrSubject();
                // 內容
                Msg.Body = _maildata.Get_StrBody();
                smtp.EnableSsl = false;
                smtp.Send(Msg);
                Msg.Dispose();
            }
            catch (Exception ex)
            {

            }
        }
    }
}