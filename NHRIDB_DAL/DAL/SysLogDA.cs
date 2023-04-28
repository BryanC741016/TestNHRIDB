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

        public void DeleteSixMonths()
        {
            DateTime _DateTime = DateTime.Now.AddMonths(-6);
            IQueryable<SysLog> _IQSysLog = _db.SysLog.Where(m=>m.createtime.HasValue.Equals(false) || m.createtime<= _DateTime);

            foreach (SysLog _SysLog in _IQSysLog)
            {
                _db.SysLog.Remove(_SysLog);
            }

            _db.SaveChanges();
        }
    }
}
