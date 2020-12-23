using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class GroupDA : DataAccess
    {
        public GroupDA(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public IQueryable<GroupUser> GetQuery(string gName="",Guid? noID=null) {
            IQueryable<GroupUser> qu = _db.GroupUser;
            if (!string.IsNullOrEmpty(gName)) {
                qu = qu.Where(e => e.gName.Equals(gName));
            }
            if (noID.HasValue) {
                qu = qu.Where(e => e.groupId!=noID.Value);
            }
            return qu;
        }

        public GroupUser GetGroupUser(Guid id) {
            return _db.GroupUser.Where(e => e.groupId == id).SingleOrDefault();
        }

        public Guid Create(string gname,int leapProject,int alwaysOpen) {

            GroupUser add = new GroupUser();
            add.groupId = Guid.NewGuid();
            add.gName = gname;
            add.leapProject = leapProject==1;
            add.alwaysOpen = alwaysOpen==1;
            _db.GroupUser.Add(add);
            _db.SaveChanges();

            return add.groupId;
        }

        public void Edit(Guid id, string gname, int leapProject, int alwaysOpen) {
            GroupUser edit = GetGroupUser(id);
            edit.gName = gname;
            edit.leapProject = leapProject==1;
            edit.alwaysOpen = alwaysOpen==1;
            _db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            GroupUser del = GetGroupUser(id);
            _db.GroupUser.Remove(del);
            _db.SaveChanges();
        }
    }
}
