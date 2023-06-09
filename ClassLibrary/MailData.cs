using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
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
}
