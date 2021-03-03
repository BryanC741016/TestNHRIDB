using Newtonsoft.Json;
using NHRIDB_DAL.DbModel;
using NHRIDB_DAL.ViewModel;
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
        
            List<InfoColummns> _columns = TypeDescriptor.GetProperties(typeof(TubeDataType))
                     .Cast<PropertyDescriptor>()
                     .Select(e=> new InfoColummns { 
                       Name = e.Name,
                       DisplayName=e.DisplayName,
                       Required= e.Attributes.Cast<Attribute>().Any(a => a.GetType() == typeof(RequiredAttribute)),
                         PropertyType=e.PropertyType
                     })
                     .ToList();
        public List<InfoColummns> GetColummns() {
            return _columns;
        }
        /***
           * 1.欄位名稱是否相符
           * 必填欄位沒有填
           * 2.性別是否統一
           * 3.年齡是否統一
           * 主key重複
           * 資料型別(f:m , 數字 , 0:1)
           * **/
        public bool ImportCheck(DataTable table, out string msg) {
            msg = "";
            //1.欄位名稱是否相符
            if (!HasColumns(table))
            {
                msg= "欄位名稱不符合，請參照範本";
                return false;
            }

           
            //必填欄位沒有填
            if (!CheckRequired(table, out msg))
            {
                return false;
            }

            //性別是否統一、年齡是否統一
            if (!RepleData(table, out msg))
            {
                return false;
            }
            //主key重複
            if (!MatchKey(table, out msg))
            {
                return false;
            }
            //各欄位的資料型別 f:m , 數字 , 0:1)
            if (!CheckType(table, out msg)) {
                return false;
            }

            return true;
        }

        public bool CheckType(DataTable table, out string msg)
        {
            msg = "";
          
            foreach (var column in _columns)
            {
                var datas = table.AsEnumerable().Where(e => !e.Field<string>(column.DisplayName).Equals(""));
                int res;
                bool commit = true;
                switch (column.Name) {
                    case "patientKey":
                    case "regionKey":
                    case "diagnosisKey":
                        break;
                    case "endYear":
                        commit =!datas.Where(e => e.Field<string>(column.DisplayName).Length != 4 || !int.TryParse(e.Field<string>(column.DisplayName), out res)).Any();
                        break;
                    case "age": 
                        commit = !datas.Where(e => !int.TryParse(e.Field<string>(column.DisplayName), out res) || res < 0).Any();
                        break;
                    case "gender":     
                        commit = !datas.Where(e => !e.Field<string>(column.DisplayName).Equals("F") && !e.Field<string>(column.DisplayName).Equals("M")).Any();
                        break;

                    default:
                    
                        commit = !datas.Where(e => !e.Field<string>(column.DisplayName).Equals("0") && !e.Field<string>(column.DisplayName).Equals("1")).Any();
                        break;

                }

                if (!commit) {
                    msg = column.DisplayName + "型別不正確";
                    return commit;
                }

            }

            
            return true;

        }

        public bool HasColumns(DataTable dataTable)
        {
            string[] columnNames = dataTable.Columns.Cast<DataColumn>()
                                    .Select(x => x.ColumnName)
                                    .ToArray();
            string[] format = _columns.Select(e => e.DisplayName).ToArray();
            return ((from item in format
                     where columnNames.Contains(item)
                     select item).Count() == format.Length);
        }

        public bool CheckRequired(DataTable table,out string columnName) {
            var info = _columns
                        .Where(e=>e.Required==true)
                      .ToList();
         
            foreach (var column in info) { 
           
                  int nullCount=  table.AsEnumerable().Where(e =>  e.Field<string>(column.DisplayName).Equals("")).Count();

                    if (nullCount > 0) {
                        columnName = "必要欄位「" + column.DisplayName + "」未填寫" ;
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

        public List<TubeDataType> GetDatasByDataTable(DataTable table) {
            List<DataRow> tt = table.AsEnumerable().ToList();
            
            List<TubeDataType> datas = new List<TubeDataType>();
            foreach (DataRow dr in tt) {
                TubeDataType data = new TubeDataType();
                foreach (var info in _columns) {
                   
                        if (info.PropertyType.Name.Equals("Boolean"))
                        {
                            bool bo = dr[info.DisplayName].Equals("1") ? true : false;
                            data.GetType().GetProperty(info.Name).SetValue(data, Convert.ChangeType(bo, info.PropertyType), null);
                        }
                        else {
                            data.GetType().GetProperty(info.Name).SetValue(data, Convert.ChangeType(dr[info.DisplayName], info.PropertyType), null);
                        }
                   
                   
                }
               
                datas.Add(data);
            }

            return datas;
        }

        public void Create(List<TubeDataType> datas,Guid hkey,Guid uid) {
            List<TubeData> adds = new List<TubeData>();
            DateTime now = DateTime.Now;
            foreach (TubeDataType data in datas) {
                TubeData tube = new TubeData();
                foreach (var info in _columns)
                {
                    var value = data.GetType().GetProperty(info.Name).GetValue(data);
                    tube.GetType().GetProperty(info.Name).SetValue(tube, Convert.ChangeType(value, info.PropertyType), null);
                }
                tube.hospitalId = hkey;
                tube.createUser = uid;
                    tube.createDate = now;

                adds.Add(tube);
            }

            List<TubeData> old = _db.TubeData.Where(e => e.hospitalId == hkey).ToList();
            TubeDataToLog(old);
            _db.TubeData.RemoveRange(old);
            _db.TubeData.AddRange(adds);
            _db.SaveChanges();
        }

        private void TubeDataToLog(List<TubeData> datas)
        {
            List<TubeDataLog> adds = new List<TubeDataLog>();
         
            foreach (TubeData data in datas)
            {
                TubeDataLog tube = new TubeDataLog();
                foreach (var info in _columns)
                {
                    var value = data.GetType().GetProperty(info.Name).GetValue(data);
                    tube.GetType().GetProperty(info.Name).SetValue(tube, Convert.ChangeType(value, info.PropertyType), null);
                }
                 

                adds.Add(tube);
            }

            _db.TubeDataLog.AddRange(adds);
            
        }

        public DataTable GetEmptyDataTable()
        {
            DataTable table = new DataTable();
             
            foreach (var item in _columns)
            {
                DataColumn column = new DataColumn();
                column.ColumnName = item.DisplayName;
                column.Caption = item.DisplayName;
                column.AllowDBNull = item.Required;
                table.Columns.Add(column);
            }
            

            return table;
        }

    }
}
