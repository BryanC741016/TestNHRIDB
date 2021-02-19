using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class DataAccess
    {
        protected NHRIDBEntitiesDB _db { get; set; }
        public DataAccess(NHRIDBEntitiesDB db) {
            this._db = db;
        }

        public DataAccess()
        {
            this._db = new NHRIDBEntitiesDB();
        }
    }
}
