using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class NodeDataDa : DataAccess
    {
        public NodeDataDa(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public IQueryable<NodeData> GetChild(Nullable<Guid> parentId=null) {
            IQueryable<NodeData> qu = _db.NodeData;
  
                qu = qu.Where(e => e.parentId== parentId);
               qu = qu.OrderBy(e => e.sort);
            return qu;
        }

        
    }
}
