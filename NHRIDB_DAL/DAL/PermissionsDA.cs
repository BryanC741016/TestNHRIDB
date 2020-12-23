using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class PermissionsDA : DataAccess
    {
        public PermissionsDA(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public IQueryable<Permissions> GetQuery(Guid? gid=null) {
            IQueryable<Permissions> qu = _db.Permissions;
            if (gid.HasValue) {
                qu = qu.Where(e => e.groupId == gid.Value);
            }
            return qu;
        }

        public void AddRange(List<Permissions> items) {
            _db.Permissions.AddRange(items);
            _db.SaveChanges();
        }
        public void UpdateRange(Guid gid,List<Permissions> items)
        {
            Delete(gid);
            _db.Permissions.AddRange(items);
            _db.SaveChanges();
        }
        public void Delete(Guid gid) {
            IQueryable<Permissions> qu = GetQuery(gid);
            _db.Permissions.RemoveRange(qu);
            _db.SaveChanges();
        }

    }
}
