using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class RegionDA : DataAccess
    {
       /* public RegionDA(NHRIDBEntitiesDB db) : base(db)
        {
        }*/

        public List<Region> GetQuery(string rkey="",string name_en="",string name_tw="") {
            IQueryable<Region> qu = _db.Region;
            if(!string.IsNullOrEmpty(rkey))
                qu= qu.Where(e => e.regionKey.Contains(rkey));

            if (!string.IsNullOrEmpty(name_en))
                qu= qu.Where(e => e.name_en.Contains(name_en));

            if (!string.IsNullOrEmpty(name_tw))
                qu= qu.Where(e => e.name_tw.Contains(name_tw));
             
            return qu.ToList();
        }

        public DataTable GetDataTable()
        {
            DataTable table = new DataTable();
            string[] columns = new string[] { "部位編號", "部位名稱" };
            foreach (var item in columns)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = item;
                column.Caption = item;
                column.AllowDBNull = true;
                table.Columns.Add(column);
            }
            IQueryable<Region> qu = _db.Region;
            foreach (var item in qu.ToList())
            {
                DataRow row = table.NewRow();
                row["部位編號"] = item.regionKey;
                row["部位名稱"] = item.name_en;
                table.Rows.Add(row);
            }

            return table;
        }

        public bool HasAny(string rkey,string newkey,string name_en,string name_tw,out string msg) {
            IQueryable<Region> qu = _db.Region;
            msg = "";
            if (!string.IsNullOrEmpty(rkey)) {
                qu= qu.Where(e => !e.regionKey.Equals(rkey));
            }
            bool commit=false;
            if (!string.IsNullOrEmpty(newkey))
            {
                commit = qu.Any(e => e.regionKey.Equals(newkey));
                if (commit)
                {
                    msg = "編號已被使用";
                    return commit;
                }
            }
          

            commit = qu.Any(e => e.name_en.Equals(name_en));
            if (commit) {
                msg = "英文名稱重複";
                return commit;
            }

            commit = qu.Any(e => e.name_tw.Equals(name_tw));
            if (commit)
            {
                msg = "中文名稱重複";
            }
            return commit;
        }

        public Region GetRegion(string rkey) {
            IQueryable<Region> qu = _db.Region;
            
           return qu.Where(e => e.regionKey.Equals(rkey)).SingleOrDefault();
           
        }

        public void Create(string id,string en_name,string tw_name) {
            if (GetRegion(id) != null) {
                return;
            }
            Region add = new Region();
            add.regionKey = id;
            add.name_en = en_name;
            add.name_tw = tw_name;
            _db.Region.Add(add);
            _db.SaveChanges();
        }
 


        public void Edit(string rkey, string en_name, string tw_name)
        {
            Region data= GetRegion(rkey);
            data.name_en = en_name;
            data.name_tw = tw_name;
            _db.SaveChanges();
        }

        public bool Delete(string rkey)
        {
            Region data = GetRegion(rkey);
            if (data == null) {
                return false;
            }
            _db.Region.Remove(data);
            _db.SaveChanges();
            return true;
        }

        
         
    }
}
