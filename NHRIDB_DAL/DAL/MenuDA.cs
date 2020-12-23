using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class MenuDA : DataAccess
    {
        public MenuDA(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public IQueryable<MenuName> GetQuery(bool? isNullParent=null,int? parentId=null) {
            IQueryable<MenuName> qu = _db.MenuName;
            if (isNullParent.HasValue && isNullParent.Value==true) {
                qu = qu.Where(e => e.parentMenu == null);
            }

            if (parentId.HasValue) {
                qu = qu.Where(e => e.parentMenu == parentId.Value);
            }

            return qu;
        }

        
    }
}
