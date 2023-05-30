using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace NHRIDB_DAL.DAL
{
    public class TubeFileUploadDataDA : DataAccess
    {
        public TubeFileUploadDataDA(NHRIDBEntitiesDB db) : base(db)
        {

        }

        public void Create(System.Data.DataTable datas, string hospitalId, string userId)
        {
            TubeFileUploadData _TubeFileUploadData = new TubeFileUploadData();
            _TubeFileUploadData.hospitalId = Guid.Parse(hospitalId);
            _TubeFileUploadData.count = datas.AsEnumerable().Count();
            _TubeFileUploadData.createUser = Guid.Parse(userId);
            _TubeFileUploadData.createDate = DateTime.Now;

            _db.TubeFileUploadData.Add(_TubeFileUploadData);
            _db.SaveChanges();
        }
    }
}
