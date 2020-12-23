using NHRIDB_DAL.DbModel;
using NHRIDBLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class UserDA : DataAccess
    {
        public UserDA(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public IQueryable<User> GetQuery(Nullable<Guid> hosID=null,string email="",string userName="",Nullable<Guid> noID=null) {
            IQueryable<User> qu = _db.User;
            if (hosID.HasValue) {
                qu = qu.Where(e => e.id_Hospital == hosID);
            }
            if (!string.IsNullOrEmpty(email)) {
                qu = qu.Where(e => e.email.Equals(email));
            }
            if (!string.IsNullOrEmpty(userName))
            {
                qu = qu.Where(e => e.userName.Equals(userName));
            }

            if (noID.HasValue)
            {
                qu = qu.Where(e => e.userId != noID);
            }

            qu = qu.Where(e => e.del == false);
            return qu;
        }

        public User HasQuery(string username,string password) {
            CryptoSHA512 crypto = new CryptoSHA512();
          
            string passwd = crypto.CryptoString(password);

            return _db.User.Where(e => e.userName.Equals(username) && e.password.Equals(passwd) && e.del==false)
                 .SingleOrDefault();
        }

        public User GetUser(Guid id) {
            return _db.User.Where(e => e.userId==id)
                   .SingleOrDefault();
        }

        public void Create(string userName, string password,Guid hosId,Guid groupId,string email,string name) {
            CryptoSHA512 crypto = new CryptoSHA512();

            string passwd = crypto.CryptoString(password);

            User create = new User();
            create.userName = userName;
            create.password = passwd;
            create.id_Hospital = hosId;
            create.groupId = groupId;
            create.email = email;
            create.name = name;
            _db.User.Add(create);
            _db.SaveChanges();
        }

        public void Edit(Guid id,string userName="", Guid? hosId=null, Guid? groupId=null, string email="",string name="") {
            User user = GetUser(id);
            if(!string.IsNullOrEmpty(userName))
             user.userName = userName;

           
           

            if (hosId.HasValue)
            user.id_Hospital = hosId.Value;

            if(groupId.HasValue)
            user.groupId = groupId.Value;


            user.email = email;
            user.name = name;
            _db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            User edit = GetUser(id);
            // _db.User.Remove(del);
            edit.del = true;
            _db.SaveChanges();
        }

        public void ChagePasswd(Guid id,string password) {
            User user = GetUser(id);
            if (!string.IsNullOrEmpty(password))
            {
                CryptoSHA512 crypto = new CryptoSHA512();
                string passwd = crypto.CryptoString(password);
                user.password = passwd;
            }
            _db.SaveChanges();
        }
    }
}
