using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class LogLoginDA : DataAccess
    {
        public LogLoginDA(NHRIDBEntitiesDB db) : base(db)
        {
        }
        public void Create(string userName,string ip,bool isLogin) {
            LogLogin logLogin = new LogLogin();
            logLogin.userName = userName;
            logLogin.ip = ip;
            logLogin.isLogin = isLogin;
            logLogin.lognDate = DateTime.Now;

            _db.LogLogin.Add(logLogin);
            _db.SaveChanges();
        }
       

        
    }
}
