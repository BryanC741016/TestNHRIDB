using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
    public class ErrorLogDA : DataAccess
    {
        public ErrorLogDA(NHRIDBEntitiesDB db) : base(db)
        {

        }

        public void Create(string id, string controller,string action, string message, string stacktrace)
        {
            ErrorLog _ErrorLog = new ErrorLog();
            _ErrorLog.id = id;
            _ErrorLog.createtime =DateTime.Now;
            _ErrorLog.controller = controller;
            _ErrorLog.action = action;
            _ErrorLog.message = message;
            _ErrorLog.stacktrace = stacktrace;

            _db.ErrorLog.Add(_ErrorLog);
            _db.SaveChanges();
        }
    }
}
