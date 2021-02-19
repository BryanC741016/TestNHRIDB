using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class DiagnosisDA : DataAccess
    {
        //public DiagnosisDA(NHRIDBEntitiesDB db) : base(db)
        //{
        //}

        public List<Diagnosis> GetQuery(string dkey="",string name_en="",string name_tw="") {
            IQueryable<Diagnosis> qu = _db.Diagnosis;
            if(!string.IsNullOrEmpty(dkey))
                qu= qu.Where(e => e.diagnosisKey.Contains(dkey));

            if (!string.IsNullOrEmpty(name_en))
                qu= qu.Where(e => e.dname_en.Contains(name_en));

            if (!string.IsNullOrEmpty(name_tw))
                qu= qu.Where(e => e.dname_tw.Contains(name_tw));
             
            return qu.ToList();
        }

        public bool HasAny(string dkey,string newkey,string name_en,string name_tw,out string msg) {
            IQueryable<Diagnosis> qu = _db.Diagnosis;
            msg = "";
            if (!string.IsNullOrEmpty(dkey)) {
                qu= qu.Where(e => !e.diagnosisKey.Equals(dkey));
            }
            bool commit=false;
            if (!string.IsNullOrEmpty(newkey))
            {
                commit = qu.Any(e => e.diagnosisKey.Equals(newkey));
                if (commit)
                {
                    msg = "編號已被使用";
                    return commit;
                }
            }
          

            commit = qu.Any(e => e.dname_en.Equals(name_en));
            if (commit) {
                msg = "英文名稱重複";
                return commit;
            }

            commit = qu.Any(e => e.dname_tw.Equals(name_tw));
            if (commit)
            {
                msg = "中文名稱重複";
            }
            return commit;
        }

        public Diagnosis GetDiagnosis(string rkey) {
            IQueryable<Diagnosis> qu = _db.Diagnosis;
            
           return qu.Where(e => e.diagnosisKey.Equals(rkey)).SingleOrDefault();
           
        }

        public void Create(string id,string en_name,string tw_name,List<string> rlist) {
            if (GetDiagnosis(id) != null) {
                return;
            }
            Diagnosis add = new Diagnosis();
            add.diagnosisKey = id;
            add.dname_en = en_name;
            add.dname_tw = tw_name;

            CreateR(add, rlist);

            _db.Diagnosis.Add(add);

            
            _db.SaveChanges();
        }


     
        

        public void Edit(string rkey, string en_name, string tw_name,List<string> rlist)
        {
            Diagnosis data= GetDiagnosis(rkey);
            data.dname_en = en_name;
            data.dname_tw = tw_name;
            DeleteAllR(data);
            CreateR(data, rlist);
            _db.SaveChanges();
        }

        public bool Delete(string rkey)
        {
            Diagnosis data = GetDiagnosis(rkey);
            if (data == null) {
                return false;
            }
            _db.Diagnosis.Remove(data);
            _db.SaveChanges();
            return true;
        }

        private void CreateR(Diagnosis diagnosis, List<string> rlist)
        {
            if (rlist == null)
            {
                return;
            }
            foreach (string rkey in rlist)
            {
                Region re = _db.Region.Where(e => e.regionKey.Equals(rkey)).SingleOrDefault();
                if (re != null)
                {
                    diagnosis.Region.Add(re);
                }
            }
        }

        private void DeleteAllR(Diagnosis data) {
            List<Region> list = _db.Region.Where(e=>e.Diagnosis.Any(x=>x.diagnosisKey.Equals(data.diagnosisKey))).ToList();
            foreach (Region rg in list) {
                data.Region.Remove(rg);
            }

            
        }

         
    }
}
