using NHRIDB_DAL.DbModel;
using NHRIDBLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
    public class UserLogDA : DataAccess
    {
        public UserLogDA(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public void Create(Guid userId, string userName, string password)
        {
            CryptoSHA512 crypto = new CryptoSHA512();

            string passwd = crypto.CryptoString(password);

            UserLog create = new UserLog();

            create.Id = Guid.NewGuid();
            create.userId = userId;
            create.userName = userName;
            create.password = passwd;
            create.createtime = DateTime.Now;

            _db.UserLog.Add(create);
            _db.SaveChanges();
        }

        public IQueryable<UserLog> GetUserLog(Guid userId)
        {
            return _db.UserLog.Where(e => e.userId == userId);
        }
    }
}
