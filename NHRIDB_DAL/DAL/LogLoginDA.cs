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
        public void Create(string userName, string ip, bool isLogin)
        {
            LogLogin logLogin = new LogLogin();
            logLogin.userName = userName;
            logLogin.ip = ip;
            logLogin.isLogin = isLogin;
            logLogin.lognDate = DateTime.Now;

            _db.LogLogin.Add(logLogin);
            _db.SaveChanges();
        }

        public List<GetLockUser_Result> GetLockList(int count) {
          List<GetLockUser_Result> datas=   _db.GetLockUser(count).ToList();
            return datas;
        }

        public bool HasLock(int count, string userName) {
            List<GetLockUser_Result> datas = GetLockList(count);
            return datas.Any(e => e.userName.Equals(userName));
        }

        public void UnLock(string userName) {
            _db.SetUnLockUser(userName);
        }

    }
}
