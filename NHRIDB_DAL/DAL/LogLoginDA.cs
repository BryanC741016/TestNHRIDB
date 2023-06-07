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

        public void Delete(string userName,bool isLogin,DateTime lognDate)
        {
            IQueryable<LogLogin> _IQLogLogin = _db.LogLogin;
            LogLogin _logLogin = _IQLogLogin.Where(m => m.userName.Equals(userName) && m.isLogin.Equals(isLogin) && m.lognDate.Equals(lognDate)).FirstOrDefault();

            if(_logLogin!=null && !string.IsNullOrEmpty(_logLogin.userName))
            {
                _db.LogLogin.Remove(_logLogin);
                _db.SaveChanges();
            }           
        }

        public List<GetLockUser_Result> GetLockList(int count) {
          List<GetLockUser_Result> datas=   _db.GetLockUser(count).ToList();
            return datas;
        }

        public bool HasLock(int count, string userName) {
            List<GetLockUser_Result> datas = GetLockList(count);
            return datas.Any(e => e.userName.ToUpper().Equals(userName.ToUpper()));
        }

        public void UnLock(string userName) {
            _db.SetUnLockUser(userName);
        }

        public List<LogLogin> Query(string userName)
        {
            List<LogLogin> datas = _db.LogLogin.ToList();

            datas = datas.Where(m=>m.userName.Equals(userName)).ToList();

            return datas;
        }
    }
}

/* GetLockList
GetLockList:
內
rowNum,userName,isLogin,co,isUnlock
1     ,b        ,0     ,1 ,1
2     ,b        ,1     ,0 ,1
3     ,b        ,0     ,1 ,1
1     ,c        ,0     ,1 ,1
2     ,c        ,1     ,0 ,1
3     ,c        ,0     ,1 ,1
外
rowNum,userName,co,isUnlock
1     ,b       ,1 ,1
2     ,b       ,0 ,1
3     ,b       ,1 ,1
1     ,c       ,1 ,1
2     ,c       ,0 ,1
3     ,c       ,1 ,1
外sum
(lg.rowNum<=?:最後一天起 ~ 到前 ? 天止)(sum(lg.co)=? and sum(lg.isUnlock)=?:錯誤=未解鎖次)
userName,co,isUnlock
b       ,2 ,3
c       ,2 ,3
總結
?=2,代表上述資料只抓rowNum:1~2,sum(lg.co)=2 and sum(lg.isUnlock)=2,連續2次登入錯誤+2次都未解鎖
 */
