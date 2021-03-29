using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class HospitalDA : DataAccess
    {
        public HospitalDA(NHRIDBEntitiesDB db) : base(db)
        {
        }

        public IQueryable<Hospital> GetQuery(string searchText="",string name_en="",string name_tw="",Nullable<Guid> noID=null,string hkey="") {
            IQueryable<Hospital> qu = _db.Hospital;

            if (!string.IsNullOrEmpty(searchText)) {
                qu = qu.Where(e => e.name_en.Contains(searchText) || e.name_tw.Contains(searchText));
            }

            if (!string.IsNullOrEmpty(name_en))
            {
                qu = qu.Where(e => e.name_en.Equals(name_en));
            }
            if (!string.IsNullOrEmpty(hkey))
            {
                qu = qu.Where(e => e.name_en.Equals(hkey));
            }

            if (!string.IsNullOrEmpty(name_tw))
            {
                qu = qu.Where(e => e.name_tw.Equals(name_tw));
            }

            if (noID.HasValue) {
                qu = qu.Where(e => e.id!=noID.Value);
            }
            return qu;
        }

        public Hospital GetHospital(Guid id) {
            return _db.Hospital.Where(e => e.id == id).SingleOrDefault();
        }

        public Guid Create(string name_en, string name_tw,string ex,string hkey) {

            Hospital add = new Hospital();
            add.id = Guid.NewGuid();
            add.name_en = name_en;
            add.name_tw = name_tw;
            add.fileExtension = ex;
            add.hKey = hkey;
            _db.Hospital.Add(add);
            _db.SaveChanges();

            return add.id;
        }

        public void Edit(Guid id, string name_en, string name_tw, string ex,string hkey) {
            Hospital edit = GetHospital(id);
            edit.name_en = name_en;
            edit.name_tw = name_tw;
            edit.fileExtension = ex;
            edit.hKey = hkey;
            _db.SaveChanges();
        }

        public void Delete(Guid id)
        {
            Hospital del = GetHospital(id);
            _db.Hospital.Remove(del);
            _db.SaveChanges();
        }
    }
}
