using ClassLibrary;
using NHRIDB_DAL.DAL;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace WinFrmAppNHRIDB
{
    public partial class Form1 : Form
    {
        bool isClose;

        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string[] args)
        {
            InitializeComponent();

            if (args != null && args.Length.Equals(1) && args[0].Equals("SendMult"))
            {
                SendMultEditPassword();
            }
            else if(args != null && !args.Length.Equals(1))
            {
                isClose = true;
            }
            else if (args != null && args.Length.Equals(1) && !args[0].Equals("SendMult"))
            {
                isClose = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (isClose)
            {
                this.Close();
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            SendSing();
        }

        private void BtnEditPassword_Click(object sender, EventArgs e)
        {
            SendMultEditPassword();
        }

        private void SendSing()
        {
            TxtANS.Text = string.Empty;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    MailData mailData = new MailData();
                    SendMailer sendMailer = new SendMailer();

                    mailData.Set_StrSubject(TxtSubject.Text);
                    mailData.Set_StrBody(TxtBody.Text);
                    mailData.Set_StrMail(TxtMail.Text);// 被寄的Email,email
                    mailData.Set_StrUsr(TxtUsr.Text);// 被寄的人員
                    mailData.Set_StrFromMail(TxtFromMail.Text);// 寄的Email,emailUserName->參數多"EmailFromAddr"
                    mailData.Set_StrFromUsr(TxtFromUsr.Text);// 寄的人員,""

                    sendMailer.Set_MailData(mailData);
                    sendMailer.MailSender(TxtmailServer.Text, Txtaccount.Text, Txtpassword.Text, Convert.ToInt32(Txtport.Text));
                    
                    this.TxtANS.BeginInvoke(new Action(() =>
                    {
                        TxtANS.Text = TxtANS.Text + "Success!!";
                    }));
                }
                catch (Exception ex)
                {
                    this.TxtANS.BeginInvoke(new Action(() =>
                    {
                        TxtANS.Text = TxtANS.Text + ex.Message + Environment.NewLine;
                        TxtANS.Text = TxtANS.Text + "------------->" + Environment.NewLine;
                        TxtANS.Text = TxtANS.Text + ex.StackTrace + Environment.NewLine;
                        TxtANS.Text = TxtANS.Text + "------------------------------------------->" + Environment.NewLine;
                    }));                    
                }
            });            
        }

        private void SendMultEditPassword()
        {
            TxtANS.Text = string.Empty;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    NHRIDBEntitiesDB db = new NHRIDBEntitiesDB();
                    UserDA uda = new UserDA(db);
                    
                    List<User> _ListUsers = uda.GetQuery().ToList();

                    GroupDA _GroupDA = new GroupDA(db);

                    List<GroupUser> _LitGroupUserSys = _GroupDA.GetQuery(gName: "主管理者").ToList();
                    Guid groupIdSys = _LitGroupUserSys.Count > 0 ? _LitGroupUserSys[0].groupId : Guid.NewGuid();

                    List<GroupUser> _LitGroupUserSetData = _GroupDA.GetQuery(gName: "填單填寫者").ToList();
                    Guid groupIdSetData = _LitGroupUserSetData.Count > 0 ? _LitGroupUserSetData[0].groupId : Guid.NewGuid();

                    ClassSetting _ClassSetting = ProjectSet(ConfigurationManager.AppSettings["setaddr"]);                    

                    MailData mailData = new MailData();
                    SendMailer sendMailer = new SendMailer();

                    foreach (User _UserChagePasswd in _ListUsers)
                    {
                        if (_UserChagePasswd.groupId.Equals(groupIdSetData))
                        {
                            Guid _Guid = Guid.NewGuid();
                            string StrPaswword = _Guid.ToString();
                            StrPaswword = StrPaswword.Replace("-", "");
                            StrPaswword = StrPaswword.Substring(0, 10);

                            uda.ChagePasswd(_UserChagePasswd.userId, StrPaswword);

                            List<User> _LitSendUser = new List<User>();

                            #region 國衛院系統管理者mail
                            foreach (SysEmpid _SysEmpid in _ClassSetting._LitSysEmpid)
                            {
                                User _UserSys = new User();// 國衛院系統管理者mail
                                _UserSys.email = _SysEmpid.email;
                                _UserSys.userName = _SysEmpid.username;

                                _LitSendUser.Add(_UserSys);
                            }
                            #endregion

                            foreach (User _User in _ListUsers)
                            {
                                if (!_UserChagePasswd.userId.Equals(_User.userId) && _UserChagePasswd.id_Hospital.Equals(_User.id_Hospital) && _User.groupId.Equals(groupIdSys))
                                {
                                    _LitSendUser.Add(_User);
                                }
                            }

                            foreach (User _User in _LitSendUser)
                            {
                                if (!string.IsNullOrEmpty(_User.email))
                                {
                                    mailData.Set_StrSubject(_ClassSetting.Subject);
                                    mailData.Set_StrBody(_ClassSetting.Body
                                        +","+"帳號:"+ _UserChagePasswd.userName+","+"醫院:"+ _UserChagePasswd.Hospital.name_tw+","+"密碼:"+ StrPaswword);
                                    mailData.Set_StrMail(_User.email);// 被寄的Email,email
                                    mailData.Set_StrUsr(_User.userName);// 被寄的人員
                                    mailData.Set_StrFromMail(_ClassSetting.FromMail);// 寄的Email,emailUserName->參數多"EmailFromAddr"
                                    mailData.Set_StrFromUsr(_ClassSetting.FromUsr);// 寄的人員,""

                                    sendMailer.Set_MailData(mailData);
                                    sendMailer.MailSender(_ClassSetting.mailServer, _ClassSetting.account, _ClassSetting.password, _ClassSetting.port);

                                    this.TxtANS.BeginInvoke(new Action(() =>
                                    {
                                        TxtANS.Text = TxtANS.Text + "寄信:" + _User.email + ",人員:" + _User.userName + Environment.NewLine;
                                    }));
                                }
                            }
                        }
                    }                    

                    this.TxtANS.BeginInvoke(new Action(() =>
                    {
                        TxtANS.Text = TxtANS.Text + "Success!!";
                    }));
                }
                catch (Exception ex)
                {
                    NHRIDBEntitiesDB db = new NHRIDBEntitiesDB();
                    ErrorLogDA _ErrorLogDA = new ErrorLogDA(db);
                    string id = DateTime.Now.ToString("yyyyMMddHHmmss") + "system";
                    _ErrorLogDA.Create(id: id, controller: "WinFrmAppNHRIDB", action: "SendMult", message: ex.Message, stacktrace: ex.StackTrace);

                    this.TxtANS.BeginInvoke(new Action(() =>
                    {
                        TxtANS.Text = TxtANS.Text + ex.Message + Environment.NewLine;
                        TxtANS.Text = TxtANS.Text + "------------->" + Environment.NewLine;
                        TxtANS.Text = TxtANS.Text + ex.StackTrace + Environment.NewLine;
                        TxtANS.Text = TxtANS.Text + "------------------------------------------->" + Environment.NewLine;
                    }));
                }
            });            
        }

        public ClassSetting ProjectSet(string path)
        {
            ClassSetting _ClassSetting = new ClassSetting();
            _ClassSetting._LitSysEmpid = new List<SysEmpid>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);
            XmlNode root = xmlDoc.SelectSingleNode("set");
            _ClassSetting.Subject = root.SelectSingleNode("SysEmailSet").Attributes["Subject"].Value;
            _ClassSetting.Body = root.SelectSingleNode("SysEmailSet").Attributes["Body"].Value;
            _ClassSetting.FromMail = root.SelectSingleNode("SysEmailSet").Attributes["FromMail"].Value;
            _ClassSetting.FromUsr = root.SelectSingleNode("SysEmailSet").Attributes["FromUsr"].Value;
            _ClassSetting.mailServer = root.SelectSingleNode("SysEmailSet").Attributes["mailServer"].Value;
            _ClassSetting.account = root.SelectSingleNode("SysEmailSet").Attributes["account"].Value;
            _ClassSetting.password = root.SelectSingleNode("SysEmailSet").Attributes["password"].Value;
            _ClassSetting.port = Convert.ToInt32(root.SelectSingleNode("SysEmailSet").Attributes["port"].Value);

            var itemNodeList = root.SelectNodes("SysEmpid");

            foreach (XmlNode item in itemNodeList)
            {
                SysEmpid _SysEmpid = new SysEmpid();
                _SysEmpid.username = item.Attributes["username"].Value;
                _SysEmpid.email = item.Attributes["email"].Value;

                _ClassSetting._LitSysEmpid.Add(_SysEmpid);
            }

            return _ClassSetting;
        }

        private void ChkEditAllPassword_CheckedChanged(object sender, EventArgs e)
        {
            if(ChkEditAllPassword.Checked)
            {
                BtnEditPassword.Visible = true;
            }
            else
            {
                BtnEditPassword.Visible = false;
            }
        }        
    }
}
