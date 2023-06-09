using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFrmAppNHRIDB
{
    public class ClassSetting
    {
        public List<SysEmpid> _LitSysEmpid { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string FromMail { get; set; }

        public string FromUsr { get; set; }

        public string mailServer { get; set; }

        public string account { get; set; }

        public string password { get; set; }

        public int port { get; set; }
    }
}
