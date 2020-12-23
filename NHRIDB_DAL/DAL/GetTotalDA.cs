using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class GetTotalDA : DataAccess
    {
        public GetTotalDA(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public List<GetTotal_Result> GetQuery(Nullable<Guid> id, Nullable<Guid> hosId) {
            string level = id.HasValue ? id.Value.ToString() : "";
            string hos = hosId.HasValue ? hosId.ToString() : "";
            return _db.GetTotal(level, hos).ToList();
        }
    }
}
