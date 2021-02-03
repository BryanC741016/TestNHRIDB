using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class RegionDA : DataAccess
    {
        public RegionDA(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public IQueryable<Region> GetChild(Nullable<Guid> parentId=null) {
            IQueryable<Region> qu = _db.Region;
  
                qu = qu.Where(e => e.parentId== parentId);
               qu = qu.OrderBy(e => e.sort);
            return qu;
        }

        
    }
}
