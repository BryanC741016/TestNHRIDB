using ClassLibrary;
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
        public bool ImportCheck(DataTable table, out string msg) 
        {
            msg = string.Empty;

            //1.欄位名稱是否相符
            if (!HasColumns(table))
            {
                msg = msg + "欄位名稱不符合，請參照範本" + Environment.NewLine;
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
            if (!CheckType(table, out msg)) 
            {
                return false;
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

            bool isSuccess = true;
            columnName = string.Empty;

            foreach (var column in info) 
            {            
                int nullCount=  table.AsEnumerable().Where(e =>  e.Field<string>(column.DisplayName).Equals("")).Count();

                if (nullCount > 0) 
                {
                    columnName = columnName + "必要欄位「" + column.DisplayName + "」未填寫" + Environment.NewLine;
                    isSuccess= false;
                }                
            }

            return isSuccess;
        }

        public bool RepleData(DataTable table,out string msg) 
        {
            msg = string.Empty;
            List<string> keys=  table.AsEnumerable().GroupBy(e => e.Field<string>("個案代碼")).Where(e => e.Count() > 1)
                .Select(e => e.Key).ToList();
            bool isSuccess = true;

            foreach (string key in keys) {
                var datas = table.AsEnumerable().Where(e => e.Field<string>("個案代碼").Trim().Equals(key.Trim()));
                int sexCount= datas.GroupBy(e => e.Field<string>("性別")).Count();

                if (sexCount > 1) {
                    msg = msg+ "個案代碼:" +key + "性別欄位輸入錯誤" + Environment.NewLine;
                    isSuccess= false;
                }

                double sings =0;
                foreach (var item in datas) {
                    double sing = double.Parse(item.Field<string>("收案年份 (西元年)")) - double.Parse(item.Field<string>("年齡 (歲)"));
                    if (sings == 0) {
                        sings = sing;
                    }

                    if ((sings - sing)>1.5 || (sings - sing)<-1.5) {
                        msg = msg+ "個案代碼:" + key + "年齡欄位輸入錯誤(誤差值大於+-1.5)" + Environment.NewLine;
                        isSuccess= false;
                    }
                }
            }//end foreach key

            return isSuccess;
        }

        public bool BatchRepleData(DataTable table, out string msg,int intBatchStartIndex)
        {
            msg = string.Empty;            
            List<string> keys = table.AsEnumerable().GroupBy(e => e.Field<string>("個案代碼")).Where(e => e.Count() > 1)
                .Select(e => e.Key).ToList();
            bool isSuccess = true;
            int IntBatchRunCount = 1;

            //分批跑,只跑 1萬
            for (int i= intBatchStartIndex; i< keys.Count; i++)
            {
                if (IntBatchRunCount > 10000)
                {
                    intBatchStartIndex = i;

                    break;
                }

                var datas = table.AsEnumerable().Where(e => e.Field<string>("個案代碼").Trim().Equals(keys[i].Trim()));
                int sexCount = datas.GroupBy(e => e.Field<string>("性別")).Count();

                if (sexCount > 1)
                {
                    msg = msg + "個案代碼:" + keys[i] + "性別欄位輸入錯誤" + Environment.NewLine;
                    isSuccess = false;
                }

                double sings = 0;
                foreach (var item in datas)
                {
                    double sing = double.Parse(item.Field<string>("收案年份 (西元年)")) - double.Parse(item.Field<string>("年齡 (歲)"));
                    if (sings == 0)
                    {
                        sings = sing;
                    }

                    if ((sings - sing) > 1.5 || (sings - sing) < -1.5)
                    {
                        msg = msg + "個案代碼:" + keys[i] + "年齡欄位輸入錯誤(誤差值大於+-1.5)" + Environment.NewLine;
                        isSuccess = false;
                    }
                }

                IntBatchRunCount++;
            }

            //foreach (string key in keys)
            //{
            //    var datas = table.AsEnumerable().Where(e => e.Field<string>("個案代碼").Trim().Equals(key.Trim()));
            //    int sexCount = datas.GroupBy(e => e.Field<string>("性別")).Count();

            //    if (sexCount > 1)
            //    {
            //        msg = msg + "個案代碼:" + key + "性別欄位輸入錯誤" + Environment.NewLine;
            //        isSuccess = false;
            //    }

            //    double sings = 0;
            //    foreach (var item in datas)
            //    {
            //        double sing = double.Parse(item.Field<string>("收案年份 (西元年)")) - double.Parse(item.Field<string>("年齡 (歲)"));
            //        if (sings == 0)
            //        {
            //            sings = sing;
            //        }

            //        if ((sings - sing) > 1.5 || (sings - sing) < -1.5)
            //        {
            //            msg = msg + "個案代碼:" + key + "年齡欄位輸入錯誤(誤差值大於+-1.5)" + Environment.NewLine;
            //            isSuccess = false;
            //        }
            //    }
            //}//end foreach key

            return isSuccess;
        }

        public bool MatchKey(DataTable table,out string msg)
        {
            msg = string.Empty;
            var data= table.AsEnumerable().GroupBy(e => new { key1= e.Field<string>("個案代碼"), key2=e.Field<string>("器官/部位代碼"), key3 = e.Field<string>("診斷代碼"), key4 = e.Field<string>("收案年份 (西元年)"), key5 = e.Field<string>("年齡 (歲)") })
                .Where(e=>e.Count() > 1);

            if (data.Count() > 0) {
                
                foreach(var Item in data)
                {
                    msg = msg
                        + Item.Key.key1 + "(個案代碼)," 
                        + Item.Key.key2 + "(器官/部位代碼)," 
                        + Item.Key.key3 + "(診斷代碼)," 
                        + Item.Key.key4 + "(收案年份 (西元年)),"
                        + Item.Key.key5 + "(年齡 (歲)) 資料重複"
                        + Environment.NewLine;
                }
                
                return false;
            }

            return true;
        }

        public bool CheckType(DataTable table, out string msg)
        {
            msg = string.Empty;
            bool isSuccess = true;

            foreach (var column in _columns)
            {
                var datas = table.AsEnumerable().Where(e => !e.Field<string>(column.DisplayName).Equals(""));
                decimal age;
                int endYear;
                bool commit = true;
                switch (column.Name)
                {
                    case "patientKey":
                    case "regionKey":
                    case "diagnosisKey":
                        break;
                    case "endYear":
                        commit = !datas.Where(e => e.Field<string>(column.DisplayName).Length != 4 || !int.TryParse(e.Field<string>(column.DisplayName), out endYear)).Any();
                        break;
                    case "age":
                        commit = !datas.Where(e => !decimal.TryParse(e.Field<string>(column.DisplayName), out age) || age < 0).Any();
                        break;
                    case "gender":
                        commit = !datas.Where(e => !e.Field<string>(column.DisplayName).Equals("F") && !e.Field<string>(column.DisplayName).Equals("M")).Any();
                        break;
                    default:
                        commit = !datas.Where(e => !e.Field<string>(column.DisplayName).Equals("0") && !e.Field<string>(column.DisplayName).Equals("1")).Any();
                        break;
                }

                if (!commit)
                {
                    msg = msg + column.DisplayName + "型別不正確" + Environment.NewLine;
                    isSuccess = false;
                }
            }

            return isSuccess;
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

        public DataSaveAns Create(List<TubeDataType> datas,Guid hkey,Guid uid) 
        {
            DataSaveAns _DataSaveAns = new DataSaveAns();

            try
            {
                List<TubeData> adds = new List<TubeData>();
                DateTime now = DateTime.Now;
                foreach (TubeDataType data in datas)
                {
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
                TubeDataToLog(old, hkey, uid);
                _db.TubeData.RemoveRange(old);
                _db.TubeData.AddRange(adds);
                _db.SaveChanges();

                _DataSaveAns.isSuccess = true;
            }
            catch(Exception e)
            {
                _DataSaveAns.StrMsg = e.Message;
                _DataSaveAns.StackTrace= e.StackTrace;
                _DataSaveAns.isSuccess = false;
            }

            return _DataSaveAns;
        }

        public DataSaveAns CreateBatch(List<TubeDataType> datas, Guid hkey, Guid uid,bool isFirst)
        {
            DataSaveAns _DataSaveAns = new DataSaveAns();

            try
            {
                List<TubeData> adds = new List<TubeData>();
                DateTime now = DateTime.Now;
                foreach (TubeDataType data in datas)
                {
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

                if (isFirst)
                    TubeDataToLog(old, hkey, uid);

                if(isFirst)
                    _db.TubeData.RemoveRange(old);

                _db.TubeData.AddRange(adds);
                _db.SaveChanges();

                _DataSaveAns.isSuccess = true;
            }
            catch (Exception e)
            {
                _DataSaveAns.StrMsg = e.Message;
                _DataSaveAns.StackTrace = e.StackTrace;
                _DataSaveAns.isSuccess = false;
            }

            return _DataSaveAns;
        }

        private void TubeDataToLog(List<TubeData> datas,Guid hoid,Guid uId)
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

                tube.hospitalId = hoid;
                tube.createUser = data.createUser;
                tube.createDate = data.createDate;
                tube.age = data.age;
                tube.LogDate = DateTime.Now;

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
