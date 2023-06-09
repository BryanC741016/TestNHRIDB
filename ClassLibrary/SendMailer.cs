using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
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
            #region Old
            //SmtpClient smtp = new SmtpClient(mailServer, port);

            //if (!string.IsNullOrEmpty(account) && !string.IsNullOrEmpty(password))
            //{
            //    smtp.Credentials = new NetworkCredential(account, password);
            //}
            //smtp.Port = port;
            //MailMessage Msg = new MailMessage();
            //System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            //System.Text.Encoding headerEncoding = System.Text.Encoding.BigEndianUnicode;

            //MailAddress mailFrom = new MailAddress(this._maildata.Get_StrFromMail(), this._maildata.Get_StrFromUsr(), encoding);

            //Msg.From = mailFrom;

            //try
            //{
            //    MailAddress mailTo = new MailAddress(this._maildata.Get_StrMail(), this._maildata.Get_StrUsr(), headerEncoding);

            //    // 內容使用html
            //    Msg.IsBodyHtml = true;
            //    Msg.BodyEncoding = System.Text.Encoding.UTF8;
            //    // 前面是發信email後面是顯示的名稱
            //    // Msg.From = New MailAddress("system@mail.com", "system")
            //    // 收信者email 
            //    Msg.To.Add(mailTo);

            //    // CC
            //    if (_maildata.Get_StrCC().Length > 0)
            //        Msg.CC.Add(_maildata.Get_StrCC());
            //    // 設定優先權
            //    Msg.Priority = MailPriority.Normal;
            //    // 標題
            //    Msg.Subject = _maildata.Get_StrSubject();
            //    // 內容
            //    Msg.Body = _maildata.Get_StrBody();
            //    smtp.EnableSsl = false;
            //    smtp.Send(Msg);
            //    Msg.Dispose();
            //}
            //catch (Exception ex)
            //{

            //}
            #endregion

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
    }
}
