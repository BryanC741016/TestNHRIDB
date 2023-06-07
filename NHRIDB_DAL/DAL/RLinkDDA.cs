using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class RLinkDDA : DataAccess
    {
        public IQueryable<RLinkD> GetQuery(string dkey="",string rKey="",string dName="",string rName="") {
            IQueryable<RLinkD> qu = _db.RLinkD;

            if (!string.IsNullOrEmpty(dName)) {
                qu = qu.Where(e => e.dName.Contains(dName) );
            }

            if (!string.IsNullOrEmpty(rName))
            {
                qu = qu.Where(e => e.rName.Contains(rName));
            }

            if (!string.IsNullOrEmpty(dkey))
            {
                qu = qu.Where(e => e.diagnosisKey.Equals(dkey));
            }

            if (!string.IsNullOrEmpty(rKey))
            {
                qu = qu.Where(e => e.regionKey.Equals(rKey));
            }

            
            return qu;
        }
        public bool HasAny(string dkey = "", string rkey = "", string dName = "", string rName = "", string noRkey = "", string noDkey = "")
        {
            IQueryable<RLinkD> qu = _db.RLinkD;

            if (!string.IsNullOrEmpty(dName))
            {
                qu = qu.Where(e => e.dName.Equals(dName));
            }

            if (!string.IsNullOrEmpty(rName))
            {
                qu = qu.Where(e => e.rName.Equals(rName));
            }

            if (!string.IsNullOrEmpty(dkey))
            {
                qu = qu.Where(e => e.diagnosisKey.Equals(dkey));
            }

            if (!string.IsNullOrEmpty(rkey))
            {
                qu = qu.Where(e => e.regionKey.Equals(rkey));
            }

            if (!string.IsNullOrEmpty(noRkey))
            {
                qu = qu.Where(e => !e.regionKey.Equals(noRkey));
            }

            if (!string.IsNullOrEmpty(noDkey))
            {
                qu = qu.Where(e => !e.diagnosisKey.Equals(noDkey));
            }
            return qu.Count() > 1;
        }

        public RLinkD GetRD(string rkey,string dkey) {
            return _db.RLinkD.Where(e => e.regionKey.Equals(rkey) && e.diagnosisKey.Equals(dkey)).SingleOrDefault();
        }

        public void Create(string regionKey, string diagnosisKey, string rName, string dName) {

            RLinkD add = new RLinkD();
           
            add.regionKey = regionKey;
            add.diagnosisKey = diagnosisKey;
            add.rName = rName;
            add.dName = dName;

            _db.RLinkD.Add(add);
            _db.SaveChanges();

      
        }

        public bool Edit(string regionKey, string diagnosisKey, string rName, string dName,out string msg) {
            RLinkD edit = GetRD(regionKey, diagnosisKey);
            msg = "";
            if (edit == null) {
                msg = "查無此資料";
                return false;
            }
            
            if (HasAny(rName: rName, dName: dName, noDkey: diagnosisKey, noRkey: regionKey))
            {
                msg = "部位與診斷名稱已被使用";
                return false;
            }
          
            edit.rName = rName;
            edit.dName = dName;
            _db.SaveChanges();
            return true;
        }

        public void Delete(string regionKey, string diagnosisKey)
        {
            RLinkD edit = GetRD(regionKey, diagnosisKey);
            _db.RLinkD.Remove(edit);
            _db.SaveChanges();
        }

        public DataTable GetDataTable()
        {
            DataTable table = new DataTable();
            string[] columns = new string[] { "部位編號", "部位名稱" ,"診斷編號", "診斷名稱" };
            foreach (var item in columns)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = item;
                column.Caption = item;
                column.AllowDBNull = true;
                table.Columns.Add(column);
            }
            IQueryable<RLinkD> qu = _db.RLinkD;
            foreach (var item in qu.ToList())
            {
                DataRow row = table.NewRow();
                row[columns[0]] = item.regionKey;
                row[columns[1]] = item.rName;
                row[columns[2]] = item.diagnosisKey;
                row[columns[3]] = item.dName;
                table.Rows.Add(row);
            }

            return table;
        }
      
        public bool CheckDLinkR(DataTable table, out string msg)
        {
            msg = string.Empty;
            bool isSuccess = true;
            var datas = table.AsEnumerable().Select(e => new { regionKey = e.Field<string>("器官/部位代碼"), diagnosisKey = e.Field<string>("診斷代碼") })
                 .Distinct().ToList();
            IQueryable<RLinkD> qu = GetQuery();

            foreach (var data in datas)
            {
                bool commit = qu.Where(e => e.diagnosisKey.Equals(data.diagnosisKey) && e.regionKey.Equals(data.regionKey))
                      .Any();

                if (!commit)
                {
                    msg = msg+ data.diagnosisKey + "(診斷代碼)與" + data.regionKey + "(部位編號)查無相關資料" + Environment.NewLine;
                    isSuccess= false;
                }
            }

            return isSuccess;
        }

        public bool CheckDLinkR(DataTable table, ref List<DataRow> row)
        {
            bool isSuccess = true;
            var datas = table.AsEnumerable().Select(e => new { regionKey = e.Field<string>("器官/部位代碼"), diagnosisKey = e.Field<string>("診斷代碼") })
                 .Distinct().ToList();
            IQueryable<RLinkD> qu = GetQuery();

            foreach (var data in datas)
            {
                bool commit = qu.Where(e => e.diagnosisKey.Equals(data.diagnosisKey) && e.regionKey.Equals(data.regionKey))
                      .Any();

                if (!commit)
                {
                    DataRow drNew = table.NewRow();
                    drNew[0] = data.diagnosisKey + "(診斷代碼)與" + data.regionKey + "(部位編號)查無相關資料" + Environment.NewLine;

                    row.Add(drNew);


                    row.AddRange(
                                table.AsEnumerable()
                                    .Where(e =>
                                        (e["器官/部位代碼"] != DBNull.Value && e.Field<string>("器官/部位代碼").Equals(data.regionKey)) &&
                                        (e["診斷代碼"] != DBNull.Value && e.Field<string>("診斷代碼").Equals(data.diagnosisKey)))
                                    .ToList()
                        );                    
                    isSuccess = false;
                }
            }

            return isSuccess;
        }
    }
}
