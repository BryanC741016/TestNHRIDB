using Newtonsoft.Json;
using NHRIDB_DAL.DbModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHRIDB_DAL.DAL
{
   public class DataTubeDA : DataAccess
    {
       
        public List<string> GetColumns() {
       
            List<string> info = TypeDescriptor.GetProperties(typeof(TubeDataType))
                     .Cast<PropertyDescriptor>()
                      .Where(e => e.DisplayName != null)
                     .Select(e => e.DisplayName)
                     .ToList();

            return info;
        }

        public bool CheckRequired(DataTable table,out string columnName) {
            var info = TypeDescriptor.GetProperties(typeof(TubeDataType))
                      .Cast<PropertyDescriptor>()
                      .Where(p => p.Attributes.Cast<Attribute>().Any(a => a.GetType() == typeof(RequiredAttribute)))
                      .ToList();
            TubeDataType basic = new TubeDataType();
            foreach (var column in info) { 
           
                  int nullCount=  table.AsEnumerable().Where(e =>  e.Field<string>(column.DisplayName)==null).Count();

                    if (nullCount > 0) {
                        columnName = column.DisplayName;
                        return false;
                    }
                
            }

            columnName = "";
            return true;

        }

        public bool RepleData(DataTable table,out string msg) {
            msg = "";
         List<string> keys=  table.AsEnumerable().GroupBy(e => e.Field<string>("識別編號")).Where(e => e.Count() > 1)
                .Select(e => e.Key).ToList();
            foreach (string key in keys) {
                var datas = table.AsEnumerable().Where(e => e.Field<string>("識別編號").Trim().Equals(key.Trim()));
               int sexCount= datas.GroupBy(e => e.Field<string>("性別")).Count();

                if (sexCount > 1) {
                    msg = "識別編號:"+key + "性別欄位輸入錯誤";
                    return false;
                }

                double sings =0;
                foreach (var item in datas) {
                    double sing = double.Parse(item.Field<string>("收案年份")) - double.Parse(item.Field<string>("年齡"));
                    if (sings == 0) {
                        sings = sing;
                    }

                    if (sings != sing) {
                        msg = "識別編號:" + key + "年齡欄位輸入錯誤";
                        return false;
                    }

                }
            }//end foreach key
            return true;
        }

        public bool MatchKey(DataTable table,out string msg)
        {
            msg = "";
           var data= table.AsEnumerable().GroupBy(e => new { key1= e.Field<string>("識別編號"), key2=e.Field<string>("部位編號"), key3 = e.Field<string>("診斷編號"), key4 = e.Field<string>("收案年份") })
                .Where(e=>e.Count() > 1);
            if (data.Count() > 0) {
                msg = data.First().Key.key1 + "(識別編號)," + data.First().Key.key2 + "(部位編號)," + data.First().Key.key3 + "(診斷編號)," + data.First().Key.key4 + "(收案年份) 資料重複";
                return false;
            }
            return true;
        }

        public List<TubeData> GetDatasByDataTable(DataTable table,Guid _hkey, Guid _user) {
            List<DataRow> tt = table.AsEnumerable().ToList();
            var infos = TypeDescriptor.GetProperties(typeof(TubeDataType))
                  .Cast<PropertyDescriptor>()
                  .ToList();
            List<TubeData> datas = new List<TubeData>();
            foreach (DataRow dr in tt) {
                TubeData data = new TubeData();
                foreach (var info in infos) {
                    if(!string.IsNullOrEmpty(dr[info.DisplayName].ToString()))
                    data.GetType().GetProperty(info.Name).SetValue(data, Convert.ChangeType(dr[info.DisplayName], info.PropertyType), null);
                }
                data.hospitalId = _hkey;
                data.createUser = _user;
                data.createDate = DateTime.Now;
                datas.Add(data);
            }

            return datas;
        }

            public DataTable GetEmptyDataTable()
        {
            DataTable table = new DataTable();
            List<string> columns = GetColumns();
            foreach (var item in columns)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = item;
                column.Caption = item;
                column.AllowDBNull = true;
                table.Columns.Add(column);
            }
            

            return table;
        }

    }
}
