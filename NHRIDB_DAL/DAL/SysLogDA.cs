using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
    public class SysLogDA : DataAccess
    {
        public SysLogDA(NHRIDBEntitiesDB db) : base(db)
        {

        }

        public void Create(string evettype, string ip, string userName)
        {
            SysLog _SysLog = new SysLog();
            Guid g = Guid.NewGuid();
            _SysLog.id = g.ToString();
            _SysLog.createtime = DateTime.Now;
            _SysLog.evettype = evettype;
            _SysLog.ip = ip;
            _SysLog.userName = userName;

            _db.SysLog.Add(_SysLog);
            _db.SaveChanges();
        }
    }
}
